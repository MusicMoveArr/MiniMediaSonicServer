using FluentValidation;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

namespace MiniMediaSonicServer.Api.Validators;

public class DeleteBookmarkRequestValidator : AbstractValidator<DeleteBookmarkRequest>
{
    public DeleteBookmarkRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}