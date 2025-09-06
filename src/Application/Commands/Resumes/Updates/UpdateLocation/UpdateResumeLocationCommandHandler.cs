using Application.Abstractions.Messaging;
using Application.Common.Helpers;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.JobPostingContext.ValueObjects;
using Domain.Contexts.ResumePostingContext.IDs;
using Domain.Repos.Resumes;
using Domain.Shared.ErrorHandling;

namespace Application.Commands.Resumes.Updates.UpdateLocation;

internal sealed class UpdateResumeLocationCommandHandler : ICommandHandler<UpdateResumeLocationCommand>
{
    private readonly IResumeRepository _resumeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateResumeLocationCommandHandler(IResumeRepository resumeRepository, IUnitOfWork unitOfWork)
    {
        _resumeRepository = resumeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateResumeLocationCommand command, CancellationToken cancellationToken = default)
    {
        var resume = await _resumeRepository.GetByIdAsync(new ResumeId(command.Id), cancellationToken);
        if (resume is null)
        {
            return Result.Failure(Error.NotFound("Resume.NotFound", "The resume was not found."));
        }

        if (!Helpers.TryCreateVO(() => Location.Create(command.Country, command.City, command.Region, command.District, latitude: command.Latitude, longitude: command.Longitude), out Location location, out Error error))
        {
            return Result.Failure(error);
        }

        var updateResult = resume.UpdateLocation(location);
        if (updateResult.IsFailure)
        {
            return Result.Failure(updateResult.Error);
        }

        _resumeRepository.Update(resume);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}