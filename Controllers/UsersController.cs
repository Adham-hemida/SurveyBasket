using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyBasket.Contracts.Users;

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
	
	[HttpGet("{id}")]
	[HasPermission(Permissions.GetUsers)]
	public async Task<IActionResult> Get([FromRoute] string id)
	{
		var result = await _userService.GetAsync(id);
		return result.IsSuccess ? Ok(result.Value)
			: result.ToProblem();
	}	
	[HttpPost("")]
	[HasPermission(Permissions.AddUsers)]
	public async Task<IActionResult> Add([FromBody] CreateUserRequest request,CancellationToken cancellationToken)
	{
		var result = await _userService.AddAsync(request, cancellationToken);
		return result.IsSuccess ? CreatedAtAction(nameof(Get), new { id = result.Value.Id }, result.Value)
			: result.ToProblem();
	}
	[HttpPut("{id}")]
	[HasPermission(Permissions.UpdateUsers)]
	public async Task<IActionResult> Update([FromRoute]string id,[FromBody] UpdateUserRequest request,CancellationToken cancellationToken)
	{
		var result = await _userService.UpdateAsync(id,request, cancellationToken);
		return result.IsSuccess ? NoContent()
			: result.ToProblem();
	}

	[HttpPut("{id}/toggle-status")]
	[HasPermission(Permissions.UpdateUsers)]
	public async Task<IActionResult> ToggleStatus([FromRoute] string id)
	{
		var result = await _userService.ToggleSatausAsync(id);
		return result.IsSuccess ? NoContent() : result.ToProblem();
	}
}
