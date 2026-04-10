using Application.DTOs.User;
using FluentValidation;

namespace Application.Validators
{
    /// <summary>
    /// Validate dữ liệu tạo mới người dùng.
    /// </summary>
    public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserDtoValidator()
        {
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

            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage("Email không đúng định dạng.")
                .When(x => !string.IsNullOrWhiteSpace(x.Email));
        }
    }
}

