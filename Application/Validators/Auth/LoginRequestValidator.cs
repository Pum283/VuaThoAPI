using Application.DTOs.Auth;
using FluentValidation;

namespace Application.Validators.Auth
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Identifier)
                .NotEmpty()
                .WithMessage("Số điện thoại hoặc email là bắt buộc.")
                .MaximumLength(256)
                .WithMessage("Số điện thoại hoặc email không được vượt quá 256 ký tự.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Mật khẩu là bắt buộc.")
                .MaximumLength(100)
                .WithMessage("Mật khẩu không được vượt quá 100 ký tự.");
        }
    }
}

