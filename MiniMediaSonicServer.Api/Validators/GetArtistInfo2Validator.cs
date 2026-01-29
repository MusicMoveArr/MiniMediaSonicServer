using FluentValidation;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

namespace MiniMediaSonicServer.Api.Validators;

public class GetArtistInfo2RequestValidator : AbstractValidator<GetArtistInfo2Request>
{
    public GetArtistInfo2RequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}