using FluentValidation;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

namespace MiniMediaSonicServer.Api.Validators;

public class GetSongRequestValidator : AbstractValidator<GetSongRequest>
{
    public GetSongRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty)
            .NotEmpty();
    }
}