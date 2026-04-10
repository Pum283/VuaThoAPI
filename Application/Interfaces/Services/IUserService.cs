using Application.Common;
using Application.DTOs.User;

namespace Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<OperationResult<UserResponse>> CreateAsync(CreateUserDto dto, CancellationToken cancellationToken = default);
    }
}

