using FluentValidation;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

namespace MiniMediaSonicServer.Api.Validators;

public class StreamRequestValidator : AbstractValidator<StreamRequest>
{
    public StreamRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}