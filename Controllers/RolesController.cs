
namespace SurveyBasket.Controllers;
[Route("api/[controller]")]
[ApiController]
public class RolesController(IRoleService roleService) : ControllerBase
{
	private readonly IRoleService _roleService = roleService;

	[HttpGet("")]
	[HasPermission(Permissions.GetRoles)]
	public async Task<IActionResult> GetAll([FromQuery] bool includeDisabled,CancellationToken cancellationToken)
	{
		var roles = await _roleService.GetAllAsync(includeDisabled,cancellationToken);
		return Ok(roles);
	}
}
