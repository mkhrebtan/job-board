using Application.Abstractions.Messaging;
using Application.Common.Helpers;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.ResumePostingContext.IDs;
using Domain.Contexts.ResumePostingContext.ValueObjects;
using Domain.Repos.Resumes;
using Domain.Shared.ErrorHandling;
using Domain.Shared.ValueObjects;

namespace Application.Commands.Resumes.Updates.UpdateContactInfo;

internal sealed class UpdateResumeContactInfoCommandHandler : ICommandHandler<UpdateResumeContactInfoCommand>
{
    private readonly IResumeRepository _resumeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateResumeContactInfoCommandHandler(IResumeRepository resumeRepository, IUnitOfWork unitOfWork)
    {
        _resumeRepository = resumeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateResumeContactInfoCommand command, CancellationToken cancellationToken = default)
    {
        var resume = await _resumeRepository.GetByIdAsync(new ResumeId(command.Id), cancellationToken);
        if (resume is null)
        {
            return Result.Failure(Error.NotFound("Resume.NotFound", "The resume was not found."));
        }

        if (!Helpers.TryCreateVO(() => Email.Create(command.Email), out Email email, out Error error))
        {
            return Result.Failure(error);
        }

        if (!Helpers.TryCreateVO(() => PhoneNumber.Create(command.PhoneNumber, command.PhoneNumberRegionCode), out PhoneNumber phoneNumber, out error))
        {
            return Result.Failure(error);
        }

        if (!Helpers.TryCreateVO(() => ContactInfo.Create(email, phoneNumber), out ContactInfo contactInfo, out error))
        {
            return Result.Failure(error);
        }

        var updateResult = resume.UpdateContactInfo(contactInfo);
        if (updateResult.IsFailure)
        {
            return Result.Failure(updateResult.Error);
        }

        _resumeRepository.Update(resume);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}