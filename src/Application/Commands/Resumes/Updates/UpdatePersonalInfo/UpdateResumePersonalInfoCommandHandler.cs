using Application.Abstractions.Messaging;
using Application.Common.Helpers;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.ResumePostingContext.ValueObjects;
using Domain.Repos.Resumes;
using Domain.Shared.ErrorHandling;

namespace Application.Commands.Resumes.Updates.UpdatePersonalInfo;

internal sealed class UpdateResumePersonalInfoCommandHandler : ICommandHandler<UpdateResumePersonalInfoCommand>
{
    private readonly IResumeRepository _resumeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateResumePersonalInfoCommandHandler(IResumeRepository resumeRepository, IUnitOfWork unitOfWork)
    {
        _resumeRepository = resumeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateResumePersonalInfoCommand command, CancellationToken cancellationToken = default)
    {
        var resume = await _resumeRepository.GetByIdAsync(command.Id, cancellationToken);
        if (resume == null)
        {
            return Result.Failure(Error.NotFound("Resume.NotFound", "The resume was not found."));
        }

        if (!Helpers.TryCreateVO(() => PersonalInfo.Create(command.FirstName, command.LastName), out PersonalInfo personalInfo, out Error error))
        {
            return Result.Failure(error);
        }

        var updateResult = resume.UpdatePersonalInfo(personalInfo);
        if (updateResult.IsFailure)
        {
            return Result.Failure(updateResult.Error);
        }

        _resumeRepository.Update(resume);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}