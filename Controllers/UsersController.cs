using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SurveyBasket.Controllers;
[Route("api/[controller]")]
[ApiController]
public class UsersController(IUserService userService) : ControllerBase
{
	private readonly IUserService _userService = userService;

	[HttpGet("")]
	[HasPermission(Permissions.GetUsers)]
	public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
	{
	  return Ok(await _userService.GetAllAsync(cancellationToken));
	}
}
