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
		if (authResult is null)
			return BadRequest("Invalid Email/Password");
		return Ok(authResult);
	}
}
