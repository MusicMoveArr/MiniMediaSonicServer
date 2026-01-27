using FluentValidation;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

namespace MiniMediaSonicServer.Api.Validators;

public class Search3RequestRequestValidator : AbstractValidator<Search3Request>
{
    public Search3RequestRequestValidator()
    {
        RuleFor(x => x.Query)
            .NotEmpty();
    }
}