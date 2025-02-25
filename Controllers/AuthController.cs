namespace SurveyBasket.Controllers;
[Route("[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
	private readonly IAuthService _authService = authService;


	[HttpPost("")]
	public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken = default)
	{
		var authResult = await _authService.GetTokenAsync(request.Email, request.Password, cancellationToken);
		return authResult.IsSuccess ? Ok(authResult.Value) : BadRequest(authResult.Error);
	}
	[HttpPost("Refresh")]
	public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken = default)
	{
		var authResult = await _authService.GetRefreshTokenAsync(request.token, request.refreshToken, cancellationToken);
		if (authResult is null)
			return BadRequest("Invalid Token");
		return Ok(authResult);
	}
	[HttpPost("revoke-refresh-token")]
	public async Task<IActionResult> RevokeRefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken = default)
	{
		var isRevoked = await _authService.RevokeRefreshTokenAsync(request.token, request.refreshToken, cancellationToken);
		if(isRevoked)
		   return Ok();
		return BadRequest("Operation Failed");
	}

}
