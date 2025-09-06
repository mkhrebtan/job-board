using Application.Abstractions.Messaging;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.IdentityContext.IDs;
using Domain.Contexts.JobPostingContext.Aggregates;
using Domain.Contexts.JobPostingContext.ValueObjects;
using Domain.Repos.Users;
using Domain.Repos.Vacancies;
using Domain.Services;
using Domain.Shared.ErrorHandling;
using Domain.Shared.ValueObjects;

namespace Application.Commands.Vacancies.Create;

internal class CreateVacancyCommandHandler : ICommandHandler<CreateVacancyCommand, CreateVacancyCommandResponse>
{
    private readonly VacancyService _vacancyService;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMarkdownParser _markdownParser;
    private readonly IVacancyRepository _vacancyRepository;

    public CreateVacancyCommandHandler(
        IUnitOfWork unitOfWork,
        VacancyService vacancyService,
        IUserRepository userRepository,
        IMarkdownParser markdownParser,
        IVacancyRepository vacancyRepository)
    {
        _unitOfWork = unitOfWork;
        _vacancyService = vacancyService;
        _userRepository = userRepository;
        _markdownParser = markdownParser;
        _vacancyRepository = vacancyRepository;
    }

    public async Task<Result<CreateVacancyCommandResponse>> Handle(CreateVacancyCommand command, CancellationToken cancellationToken = default)
    {
        var employer = await _userRepository.GetByIdAsync(new UserId(command.EmployerId), cancellationToken);
        if (employer is null)
        {
            return Result<CreateVacancyCommandResponse>.Failure(Error.NotFound("CreateVacancyCommandHandler.UserNotFound", "The specified employer user was not found."));
        }

        var vacancyTitleResult = VacancyTitle.Create(command.Title);
        if (vacancyTitleResult.IsFailure)
        {
            return Result<CreateVacancyCommandResponse>.Failure(vacancyTitleResult.Error);
        }

        var descriptionResult = RichTextContent.Create(command.DescriptionMarkdown, _markdownParser);
        if (descriptionResult.IsFailure)
        {
            return Result<CreateVacancyCommandResponse>.Failure(descriptionResult.Error);
        }

        Result<Salary> salaryResult = default!;
        if (command.MinSalary == 0 && (command.MaxSalary is null || command.MaxSalary == 0))
        {
            salaryResult = Salary.None();
        }
        else if (command.MinSalary > 0 && (command.MaxSalary is null || command.MaxSalary == command.MinSalary))
        {
            salaryResult = Salary.Fixed(command.MinSalary, command.SalaryCurrency);
        }
        else if (command.MinSalary > 0 && command.MaxSalary is not null && command.MaxSalary == command.MinSalary)
        {
            salaryResult = Salary.Range(command.MinSalary, command.MaxSalary.Value, command.SalaryCurrency);
        }
        else
        {
            return Result<CreateVacancyCommandResponse>.Failure(Error.Problem("CreateVacancyCommandHandler.InvalidSalary", "Invalid salary range provided."));
        }

        if (salaryResult.IsFailure)
        {
            return Result<CreateVacancyCommandResponse>.Failure(salaryResult.Error);
        }

        var locationResult = Location.Create(
            command.Country,
            command.City,
            command.Region,
            command.District,
            command.Address,
            command.Latitude,
            command.Longitude);
        if (locationResult.IsFailure)
        {
            return Result<CreateVacancyCommandResponse>.Failure(locationResult.Error);
        }

        var emailResult = Email.Create(command.RecruiterEmail);
        if (emailResult.IsFailure)
        {
            return Result<CreateVacancyCommandResponse>.Failure(emailResult.Error);
        }

        var phoneNumberResult = PhoneNumber.Create(command.RecruiterPhoneNumber, command.RecruiterPhoneNumberRegionCode);
        if (phoneNumberResult.IsFailure)
        {
            return Result<CreateVacancyCommandResponse>.Failure(phoneNumberResult.Error);
        }

        var recruiterInfoResult = RecruiterInfo.Create(
            command.RecruiterFirstName,
            emailResult.Value,
            phoneNumberResult.Value);
        if (recruiterInfoResult.IsFailure)
        {
            return Result<CreateVacancyCommandResponse>.Failure(recruiterInfoResult.Error);
        }

        Result<Vacancy> vacancyResult = command.IsDraft ?
            await _vacancyService.CreateVacancyInDraftStatusAsync(
                employer,
                vacancyTitleResult.Value,
                descriptionResult.Value,
                salaryResult.Value,
                locationResult.Value,
                recruiterInfoResult.Value,
                cancellationToken) :
             await _vacancyService.CreateVacancyInRegisteredStatusAsync(
                employer,
                vacancyTitleResult.Value,
                descriptionResult.Value,
                salaryResult.Value,
                locationResult.Value,
                recruiterInfoResult.Value,
                cancellationToken);
        if (vacancyResult.IsFailure)
        {
            return Result<CreateVacancyCommandResponse>.Failure(vacancyResult.Error);
        }

        _vacancyRepository.Add(vacancyResult.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<CreateVacancyCommandResponse>.Success(new CreateVacancyCommandResponse(vacancyResult.Value.Id.Value));
    }
}
