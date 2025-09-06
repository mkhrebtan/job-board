using Application.Abstractions.Messaging;
using Application.Commands.Resumes.Create;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.ResumePostingContext.Enums;
using Domain.Contexts.ResumePostingContext.IDs;
using Domain.Repos.Resumes;
using Domain.Shared.ErrorHandling;

namespace Application.Commands.Resumes.Languages.Add;

internal sealed class AddResumeLanguageCommandHandler : ICommandHandler<AddResumeLanguageCommand, AddResumeLanguageCommandResponse>
{
    private readonly IResumeRepository _resumeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddResumeLanguageCommandHandler(IResumeRepository resumeRepository, IUnitOfWork unitOfWork)
    {
        _resumeRepository = resumeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AddResumeLanguageCommandResponse>> Handle(AddResumeLanguageCommand command, CancellationToken cancellationToken = default)
    {
        var resume = await _resumeRepository.GetByIdAsync(new ResumeId(command.Id), cancellationToken);
        if (resume is null)
        {
            return Result<AddResumeLanguageCommandResponse>.Failure(Error.NotFound("Resume.NotFound", "The resume was not found."));
        }

        var proficiencyResult = LanguageLevel.FromCode(command.Language.ProficiencyLevel);
        if (proficiencyResult is null)
        {
            return Result<AddResumeLanguageCommandResponse>.Failure(Error.Validation("CreateResumeCommand.InvalidLanguageProficiency", $"Invalid language proficiency: '{command.Language.ProficiencyLevel}'."));
        }

        var langResult = resume.AddLanguage(command.Language.Language, proficiencyResult);
        if (langResult.IsFailure)
        {
            return Result<AddResumeLanguageCommandResponse>.Failure(langResult.Error);
        }

        _resumeRepository.Update(resume);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<AddResumeLanguageCommandResponse>.Success(new AddResumeLanguageCommandResponse(langResult.Value.Value));
    }
}