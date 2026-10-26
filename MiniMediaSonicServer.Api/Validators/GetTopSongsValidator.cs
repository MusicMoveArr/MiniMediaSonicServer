using FluentValidation;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

namespace MiniMediaSonicServer.Api.Validators;

public class GetTopSongsRequestValidator : AbstractValidator<GetTopSongsRequest>
{
    public GetTopSongsRequestValidator()
    {
        
    }
}