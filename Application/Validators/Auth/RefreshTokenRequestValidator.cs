using Application.DTOs.Auth;
using FluentValidation;

namespace Application.Validators.Auth
{
    public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
    {
        public RefreshTokenRequestValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty()
                .WithMessage("Refresh token là bắt buộc.")
                .MaximumLength(2048)
                .WithMessage("Refresh token không hợp lệ.");
        }
    }
}

