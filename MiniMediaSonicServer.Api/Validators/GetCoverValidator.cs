using FluentValidation;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

namespace MiniMediaSonicServer.Api.Validators;

public class GetCoverRequestValidator : AbstractValidator<GetCoverRequest>
{
    public GetCoverRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}