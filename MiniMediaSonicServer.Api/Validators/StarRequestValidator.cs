using FluentValidation;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

namespace MiniMediaSonicServer.Api.Validators;

public class StarRequestValidator : AbstractValidator<StarRequest>
{
    public StarRequestValidator()
    {
        
    }
}