using FluentValidation;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

namespace MiniMediaSonicServer.Api.Validators;

public class GetTopSongsRequestValidator : AbstractValidator<GetTopSongsRequest>
{
    public GetTopSongsRequestValidator()
    {
        RuleFor(request => request.Artist)
            .MinimumLength(1)
            .NotEmpty();
    }
}