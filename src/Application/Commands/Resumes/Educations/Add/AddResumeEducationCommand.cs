using Application.Abstractions.Messaging;
using Application.Commands.Resumes.Create;
using Application.Commands.Resumes.Dtos;

namespace Application.Commands.Resumes.Educations.Add;

public record AddResumeEducationCommand(Guid Id, EducationDto Education) : ICommand<AddResumeEducationCommandResponse>;

public record AddResumeEducationCommandResponse(Guid Id);