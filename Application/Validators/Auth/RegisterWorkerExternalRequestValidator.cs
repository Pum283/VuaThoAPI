using Application.DTOs.Auth;
using FluentValidation;

namespace Application.Validators.Auth
{
    public class RegisterWorkerExternalRequestValidator : AbstractValidator<RegisterWorkerExternalRequest>
    {
        public RegisterWorkerExternalRequestValidator()
        {
            RuleFor(x => x.OtpVerificationToken)
                .NotEmpty()
                .WithMessage("Token xác thực OTP là bắt buộc.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithMessage("Số điện thoại là bắt buộc.")
                .MaximumLength(20)
                .WithMessage("Số điện thoại không được vượt quá 20 ký tự.");

            RuleFor(x => x.FullName)
                .NotEmpty()
                .WithMessage("Họ và tên là bắt buộc.")
                .MaximumLength(200)
                .WithMessage("Họ và tên không được vượt quá 200 ký tự.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Mật khẩu là bắt buộc.")
                .MinimumLength(8)
                .WithMessage("Mật khẩu phải có ít nhất 8 ký tự.")
                .MaximumLength(100)
                .WithMessage("Mật khẩu không được vượt quá 100 ký tự.");

            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage("Email không đúng định dạng.")
                .When(x => !string.IsNullOrWhiteSpace(x.Email));

            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90)
                .WithMessage("Vĩ độ không hợp lệ.");

            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180)
                .WithMessage("Kinh độ không hợp lệ.");
        }
    }
}

