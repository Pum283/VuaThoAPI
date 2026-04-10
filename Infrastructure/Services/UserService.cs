using Application.Common;
using Application.DTOs.User;
using Application.Interfaces.Services;
using Domain.Entity;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<OperationResult<UserResponse>> CreateAsync(CreateUserDto dto, CancellationToken cancellationToken = default)
        {
            var phoneNumber = dto.PhoneNumber.Trim();

            var isExists = await _context.Users
                .AnyAsync(x => x.PhoneNumber == phoneNumber, cancellationToken);

            if (isExists)
            {
                return OperationResult<UserResponse>.Failure("Số điện thoại đã được sử dụng.");
            }

            var user = new User
            {
                PhoneNumber = phoneNumber,
                FullName = dto.FullName.Trim(),
                Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim(),
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);

            var response = new UserResponse
            {
                UserId = user.Id,
                PhoneNumber = user.PhoneNumber,
                FullName = user.FullName,
                Email = user.Email
            };

            return OperationResult<UserResponse>.Success(response, "Đăng ký người dùng thành công.");
        }
    }
}

