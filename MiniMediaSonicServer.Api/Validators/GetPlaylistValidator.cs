using FluentValidation;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

namespace MiniMediaSonicServer.Api.Validators;

public class GetPlaylistRequestRequestValidator : AbstractValidator<GetPlaylistRequest>
{
    public GetPlaylistRequestRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty)
            .NotEmpty();
    }
}