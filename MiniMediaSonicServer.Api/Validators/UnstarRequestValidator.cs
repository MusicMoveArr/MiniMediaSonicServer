using FluentValidation;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

namespace MiniMediaSonicServer.Api.Validators;

public class UnstarRequestValidator : AbstractValidator<UnstarRequest>
{
    public UnstarRequestValidator()
    {
        
    }
}