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
		return authResult.IsSuccess
			? Ok(authResult.Value)
			: authResult.ToProblem(statusCode: StatusCodes.Status400BadRequest);

	}
	[HttpPost("Refresh")]
	public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken = default)
	{
		var authResult = await _authService.GetRefreshTokenAsync(request, cancellationToken);

		return authResult.IsSuccess
			? Ok(authResult.Value)
			: authResult.ToProblem(statusCode: StatusCodes.Status400BadRequest);

	}
	[HttpPost("revoke-refresh-token")]
	public async Task<IActionResult> RevokeRefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken = default)
	{
		var result = await _authService.RevokeRefreshTokenAsync(request, cancellationToken);
		return result.IsSuccess
			? Ok()
			:result.ToProblem(statusCode: StatusCodes.Status400BadRequest);

	}

}
