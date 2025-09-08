using Application.Abstractions.Messaging;
using Application.Common.Helpers;
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

        if (!Helpers.TryCreateVO(() => VacancyTitle.Create(command.Title), out VacancyTitle vacancyTitle, out Error error))
        {
            return Result<CreateVacancyCommandResponse>.Failure(error);
        }

        if (!Helpers.TryCreateVO(() => RichTextContent.Create(command.DescriptionMarkdown, _markdownParser), out RichTextContent description, out error))
        {
            return Result<CreateVacancyCommandResponse>.Failure(error);
        }

        Result<Salary> salaryResult = default!;
        if (command.MinSalary == 0 && (command.MaxSalary is null || command.MaxSalary == 0))
        {
            salaryResult = Salary.None();
        }
        else if (command.MinSalary > 0 && (command.MaxSalary is null || command.MaxSalary == command.MinSalary) && command.SalaryCurrency is not null)
        {
            salaryResult = Salary.Fixed(command.MinSalary, command.SalaryCurrency);
        }
        else if (command.MinSalary > 0 && command.MaxSalary is not null && command.MaxSalary != command.MinSalary && command.SalaryCurrency is not null)
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

        if (!Helpers.TryCreateVO(() => Location.Create(command.Country, command.City, command.Region, command.District, command.Address, command.Latitude, command.Longitude), out Location location, out error))
        {
            return Result<CreateVacancyCommandResponse>.Failure(error);
        }

        if (!Helpers.TryCreateVO(() => Email.Create(command.RecruiterEmail), out Email email, out error))
        {
            return Result<CreateVacancyCommandResponse>.Failure(error);
        }

        if (!Helpers.TryCreateVO(() => PhoneNumber.Create(command.RecruiterPhoneNumber, command.RecruiterPhoneNumberRegionCode), out PhoneNumber phoneNumber, out error))
        {
            return Result<CreateVacancyCommandResponse>.Failure(error);
        }

        if (!Helpers.TryCreateVO(() => RecruiterInfo.Create( command.RecruiterFirstName, email, phoneNumber), out RecruiterInfo recruiterInfo, out error))
        {
            return Result<CreateVacancyCommandResponse>.Failure(error);
        }

        Result<Vacancy> vacancyResult = command.IsDraft ?
            await _vacancyService.CreateVacancyInDraftStatusAsync(
                employer,
                vacancyTitle,
                description,
                salaryResult.Value,
                location,
                recruiterInfo,
                cancellationToken) :
             await _vacancyService.CreateVacancyInRegisteredStatusAsync(
                employer,
                vacancyTitle,
                description,
                salaryResult.Value,
                location,
                recruiterInfo,
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
