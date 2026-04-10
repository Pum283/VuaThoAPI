using Application.Common;
using Application.DTOs.Auth;

namespace Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<OperationResult> SendOtpAsync(SendOtpRequest request, CancellationToken cancellationToken = default);
        Task<OperationResult<VerifyOtpResponse>> VerifyOtpAsync(VerifyOtpRequest request, CancellationToken cancellationToken = default);

        Task<OperationResult> SendEmailOtpAsync(SendEmailOtpRequest request, CancellationToken cancellationToken = default);
        Task<OperationResult<VerifyOtpResponse>> VerifyEmailOtpAsync(VerifyEmailOtpRequest request, CancellationToken cancellationToken = default);

        Task<OperationResult<AuthResponse>> RegisterCustomerAsync(RegisterCustomerRequest request, CancellationToken cancellationToken = default);
        Task<OperationResult<AuthResponse>> RegisterWorkerExternalAsync(RegisterWorkerExternalRequest request, CancellationToken cancellationToken = default);
        Task<OperationResult<AuthResponse>> RegisterWorkerDirectAsync(RegisterWorkerDirectRequest request, CancellationToken cancellationToken = default);

        Task<OperationResult<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
        Task<OperationResult<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);
    }
}

