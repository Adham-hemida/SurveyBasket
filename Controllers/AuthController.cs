namespace SurveyBasket.Controllers;
[Route("[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
	private readonly IAuthService _authService = authService;


	[HttpPost("")]
	public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken = default)
	{
		var authResult = await _authService.GetTokenAsync(request, cancellationToken);
		return authResult.IsSuccess ? Ok(authResult.Value) : BadRequest(authResult.Error);
	}
	[HttpPost("Refresh")]
	public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken = default)
	{
		var authResult = await _authService.GetRefreshTokenAsync(request, cancellationToken);
		
		return authResult.IsSuccess ? Ok(authResult.Value) : BadRequest(authResult.Error);

	}
	[HttpPost("revoke-refresh-token")]
	public async Task<IActionResult> RevokeRefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken = default)
	{
		var isRevoked = await _authService.RevokeRefreshTokenAsync(request, cancellationToken);
		return isRevoked.IsSuccess ? Ok() : BadRequest(isRevoked.Error);

	}

}
