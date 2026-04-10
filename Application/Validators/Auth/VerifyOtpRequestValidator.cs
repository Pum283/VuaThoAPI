using Application.DTOs.Auth;
using FluentValidation;

namespace Application.Validators.Auth
{
    public class VerifyOtpRequestValidator : AbstractValidator<VerifyOtpRequest>
    {
        public VerifyOtpRequestValidator()
        {
            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithMessage("Số điện thoại là bắt buộc.")
                .MaximumLength(20)
                .WithMessage("Số điện thoại không được vượt quá 20 ký tự.");

            RuleFor(x => x.OtpCode)
                .NotEmpty()
                .WithMessage("Mã OTP là bắt buộc.")
                .Matches("^[0-9]{4,8}$")
                .WithMessage("Mã OTP không hợp lệ.");
        }
    }
}

