using Application.DTOs.Auth;
using FluentValidation;

namespace Application.Validators.Auth
{
    public class SendOtpRequestValidator : AbstractValidator<SendOtpRequest>
    {
        public SendOtpRequestValidator()
        {
            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithMessage("Số điện thoại là bắt buộc.")
                .MaximumLength(20)
                .WithMessage("Số điện thoại không được vượt quá 20 ký tự.");
        }
    }
}

