using FluentValidation;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

namespace MiniMediaSonicServer.Api.Validators;

public class SetRatingRequestValidator : AbstractValidator<SetRatingRequest>
{
    public SetRatingRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty)
            .NotNull();
        
        RuleFor(x => x.Rating)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(5);
    }
}