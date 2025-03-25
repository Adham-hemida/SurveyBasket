namespace SurveyBasket.Services;
public interface IAuthService
{

	Task<Result<AuthResponse>> GetTokenAsync(LoginRequest loginRequest, CancellationToken cancellationToken = default);
	Task<Result<AuthResponse>> GetRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest, CancellationToken cancellationToken = default);
	Task<Result> RevokeRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest, CancellationToken cancellationToken = default);
	Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);


}
