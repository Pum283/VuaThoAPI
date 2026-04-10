using Application.DTOs.Auth;
using FluentValidation;

namespace Application.Validators.Auth
{
    public class VerifyEmailOtpRequestValidator : AbstractValidator<VerifyEmailOtpRequest>
    {
        public VerifyEmailOtpRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email là bắt buộc.")
                .EmailAddress()
                .WithMessage("Email không đúng định dạng.")
                .MaximumLength(256)
                .WithMessage("Email không được vượt quá 256 ký tự.");

            RuleFor(x => x.OtpCode)
                .NotEmpty()
                .WithMessage("Mã OTP là bắt buộc.")
                .Matches("^[0-9]{4,8}$")
                .WithMessage("Mã OTP không hợp lệ.");
        }
    }
}

