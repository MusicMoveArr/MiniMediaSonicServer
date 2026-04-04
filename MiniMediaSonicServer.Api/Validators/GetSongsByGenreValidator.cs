using FluentValidation;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

namespace MiniMediaSonicServer.Api.Validators;

public class GetSongsByGenreRequestValidator : AbstractValidator<GetSongsByGenreRequest>
{
    public GetSongsByGenreRequestValidator()
    {
        RuleFor(x => x.Genre)
            .NotEmpty()
            .NotNull();
    }
}