using Microsoft.AspNetCore.RateLimiting;

namespace SurveyBasket.Controllers;
[Route("[controller]")]
[ApiController]
[EnableRateLimiting(RateLimiters.IpLimiter)]
public class AuthController(IAuthService authService, ILogger<AuthController> logger) : ControllerBase
{
	private readonly IAuthService _authService = authService;
	private readonly ILogger<AuthController> _logger = logger;

	[HttpPost("")]
	public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken = default)
	{
		_logger.LogInformation("loggging with email {email} and password {password}", request.Email, request.Password);
		var authResult = await _authService.GetTokenAsync(request, cancellationToken);
		return authResult.IsSuccess ? Ok(authResult.Value) : authResult.ToProblem();

	}
	[HttpPost("Refresh")]
	public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken = default)
	{
		var authResult = await _authService.GetRefreshTokenAsync(request, cancellationToken);

		return authResult.IsSuccess ? Ok(authResult.Value) : authResult.ToProblem();

	}
	[HttpPost("revoke-refresh-token")]
	public async Task<IActionResult> RevokeRefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken = default)
	{
		var result = await _authService.RevokeRefreshTokenAsync(request, cancellationToken);
		return result.IsSuccess ? Ok() : result.ToProblem();

	}
	[HttpPost("register")]
	[DisableRateLimiting]
	public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken = default)
	{
		var result = await _authService.RegisterAsync(request, cancellationToken);

		return result.IsSuccess ? Ok() : result.ToProblem();
	}
	[HttpPost("confirm-email")]
	public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request, CancellationToken cancellationToken = default)
	{
		var result = await _authService.ConfirmEmailAsync(request);

		return result.IsSuccess ? Ok() : result.ToProblem();
	}
	[HttpPost("resend-confirm-email")]
	public async Task<IActionResult> ResendConfirmEmail([FromBody] ResendConfirmEmailRequest request, CancellationToken cancellationToken = default)
	{
		var result = await _authService.ResendConfirmEmailAsync(request);

		return result.IsSuccess ? Ok() : result.ToProblem();
	}

	[HttpPost("forget-password")]
	public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordRequest request)
	{
		var result = await _authService.SendResetPasswordCodeAsync(request.Email);

		return result.IsSuccess ? Ok() : result.ToProblem();
	}
	[HttpPost("reset-password")]
	public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
	{
		var result = await _authService.ResetPasswordAsync(request);

		return result.IsSuccess ? Ok() : result.ToProblem();
	}



}
