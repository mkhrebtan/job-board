using Application.Abstractions.Messaging;
using Application.Commands.Resumes.Dtos;

namespace Application.Commands.Resumes.WorkExperiences.Add;

public record AddResumeWorkExperienceCommand(Guid Id, WorkExperienceDto WorkExperience) : ICommand<AddResumeWorkExperienceCommandResponse>;

public record AddResumeWorkExperienceCommandResponse(Guid Id);