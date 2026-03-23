using FluentValidation;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

namespace MiniMediaSonicServer.Api.Validators;

public class GetUserRequestValidator : AbstractValidator<GetUserRequest>
{
    public GetUserRequestValidator()
    {
        
    }
}