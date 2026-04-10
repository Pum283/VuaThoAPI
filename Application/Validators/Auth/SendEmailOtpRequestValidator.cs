using Application.DTOs.Auth;
using FluentValidation;

namespace Application.Validators.Auth
{
    public class SendEmailOtpRequestValidator : AbstractValidator<SendEmailOtpRequest>
    {
        public SendEmailOtpRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email là bắt buộc.")
                .EmailAddress()
                .WithMessage("Email không đúng định dạng.")
                .MaximumLength(256)
                .WithMessage("Email không được vượt quá 256 ký tự.");
        }
    }
}

