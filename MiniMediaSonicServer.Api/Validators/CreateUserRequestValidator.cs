using FluentValidation;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

namespace MiniMediaSonicServer.Api.Validators;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(request => request.Username)
            .Matches("^[a-zA-Z0-9_-]+$")
            .NotEmpty();
        
        RuleFor(request => request.Password).NotEmpty();
        RuleFor(request => request.Email).NotEmpty();
    }
}