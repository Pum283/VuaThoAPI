using Application.DTOs.User;
using Application.Interfaces.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    /// <summary>
    /// Xác thực và đăng ký tài khoản.
    /// </summary>
    public class AuthController : ApiController
    {
        private readonly IUserService _userService;
        private readonly IValidator<CreateUserDto> _createUserValidator;

        /// <summary>
        /// Khởi tạo controller xác thực.
        /// </summary>
        public AuthController(IUserService userService, IValidator<CreateUserDto> createUserValidator)
        {
            _userService = userService;
            _createUserValidator = createUserValidator;
        }

        /// <summary>
        /// Đăng ký người dùng mới.
        /// </summary>
        /// <param name="dto">Thông tin đăng ký.</param>
        /// <param name="cancellationToken">Token huỷ request.</param>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] CreateUserDto dto, CancellationToken cancellationToken)
        {
            var validation = await _createUserValidator.ValidateAsync(dto, cancellationToken);
            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(x => x.ErrorMessage).ToList();
                return BadRequest(errors);
            }

            var result = await _userService.CreateAsync(dto, cancellationToken);
            return HandleResult(result);
        }
    }
}

