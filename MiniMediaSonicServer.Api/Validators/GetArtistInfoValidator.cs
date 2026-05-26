using FluentValidation;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

namespace MiniMediaSonicServer.Api.Validators;

public class GetArtistInfoRequestValidator : AbstractValidator<GetArtistInfoRequest>
{
    public GetArtistInfoRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}