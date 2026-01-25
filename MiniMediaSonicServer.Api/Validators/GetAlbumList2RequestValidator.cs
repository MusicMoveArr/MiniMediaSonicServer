using FluentValidation;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

namespace MiniMediaSonicServer.Api.Validators;

public class GetAlbumList2RequestValidator : AbstractValidator<GetAlbumList2Request>
{
    private string[] allowedTypes = 
    [
        "random",
        "newest",
        "highest",
        "frequent",
        "recent",
        "alphabeticalByName",
        "alphabeticalByArtist",
        "starred",
        "byGenre",
        "byYear"
    ];
    
    public GetAlbumList2RequestValidator()
    {
        RuleFor(x => x.Type).Must(x => allowedTypes.Contains(x));
    }
}