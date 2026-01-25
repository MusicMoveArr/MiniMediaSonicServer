using FluentValidation;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

namespace MiniMediaSonicServer.Api.Validators;

public class GetArtistRequestValidator : AbstractValidator<GetArtistRequest>
{
    public GetArtistRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}