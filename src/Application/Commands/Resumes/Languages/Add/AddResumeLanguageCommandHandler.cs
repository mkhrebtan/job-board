using Application.Abstractions.Messaging;
using Application.Commands.Resumes.Create;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.ResumePostingContext.Enums;
using Domain.Repos.Resumes;
using Domain.Shared.ErrorHandling;

namespace Application.Commands.Resumes.Languages.Add;

internal sealed class AddResumeLanguageCommandHandler : ICommandHandler<AddResumeLanguageCommand>
{
    private readonly IResumeRepository _resumeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddResumeLanguageCommandHandler(IResumeRepository resumeRepository, IUnitOfWork unitOfWork)
    {
        _resumeRepository = resumeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AddResumeLanguageCommand command, CancellationToken cancellationToken = default)
    {
        var resume = await _resumeRepository.GetByIdAsync(command.Id, cancellationToken);
        if (resume is null)
        {
            return Result.Failure(Error.NotFound("Resume.NotFound", "The resume was not found."));
        }

        var proficiencyResult = LanguageLevel.FromCode(command.Language.ProficiencyLevel);
        if (proficiencyResult is null)
        {
            return Result<CreateResumeCommandResponse>.Failure(Error.Validation("CreateResumeCommand.InvalidLanguageProficiency", $"Invalid language proficiency: '{lang.ProficiencyLevel}'."));
        }

        var langResult = resume.AddLanguage(command.Language.Language, proficiencyResult);
        if (langResult.IsFailure)
        {
            return Result.Failure(langResult.Error);
        }

        _resumeRepository.Update(resume);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}