using FluentValidation;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

namespace MiniMediaSonicServer.Api.Validators;

public class GetAlbumRequestValidator : AbstractValidator<GetAlbumRequest>
{
    public GetAlbumRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}