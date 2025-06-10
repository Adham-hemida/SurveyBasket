
using Asp.Versioning;
using Microsoft.AspNetCore.RateLimiting;

namespace SurveyBasket.Controllers;
[ApiVersion(1, Deprecated = true)]
[ApiVersion(2)]
[Route("api/[controller]")]// /api/Polls
[ApiController]
public class PollsController(IPollService pollService) : ControllerBase
{
	private readonly IPollService _pollService = pollService;

	[HttpGet("")]
	[HasPermission(Permissions.GetPolls)]
	public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
	{
		var result = await _pollService.GetAllAsync(cancellationToken);
		return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
	}

	[MapToApiVersion(1)]
	[HttpGet("current")]
	[Authorize(Roles = DefaultRoles.Member)]
	[EnableRateLimiting(RateLimiters.UserLimiter)]
	public async Task<IActionResult> GetCurrentV1(CancellationToken cancellationToken = default)
	{
		var result = await _pollService.GetCurrentAsyncV1(cancellationToken);
		return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
	}


	[MapToApiVersion(2)]
	[HttpGet("current")]
	[Authorize(Roles = DefaultRoles.Member)]
	[EnableRateLimiting(RateLimiters.UserLimiter)]
	public async Task<IActionResult> GetCurrentV2(CancellationToken cancellationToken = default)
	{
		var result = await _pollService.GetCurrentAsyncV2(cancellationToken);
		return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
	}

	[HttpGet("{id}")]
	//[Route("{id}")]
	[HasPermission(Permissions.GetPolls)]
	public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken = default)
	{
		var result = await _pollService.GetAsync(id, cancellationToken);
		return result.IsSuccess ? Ok(result.Value) : result.ToProblem();

	}

	[HttpPost("")]
	//[Route("")]
	[HasPermission(Permissions.AddPolls)]
	public async Task<IActionResult> Create([FromBody] PollRequest request, CancellationToken cancellationToken)
	{
		var result = await _pollService.AddAsync(request, cancellationToken);

		return result.IsSuccess
			? CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value)
			: result.ToProblem();


	}

	[HttpPut("{id}")]
	//[Route("")]
	[HasPermission(Permissions.UpdatePolls)]
	public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequest request, CancellationToken cancellationToken)
	{
		var result = await _pollService.UpdateAsync(id, request, cancellationToken);
		return result.IsSuccess ? NoContent() : result.ToProblem();




	}
	[HttpDelete("{id}")]
	//[Route("")]
	[HasPermission(Permissions.DeletePolls)]
	public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
	{
		var result = await _pollService.DeleteAsync(id, cancellationToken);
		return result.IsSuccess ? NoContent() : result.ToProblem();
	}

	[HttpPut("{id}/togglePublish")]
	[HasPermission(Permissions.UpdatePolls)]
	public async Task<IActionResult> TogglePublish([FromRoute] int id, CancellationToken cancellationToken)
	{
		var result = await _pollService.TogglePublishStatusAsync(id, cancellationToken);
		return result.IsSuccess ? NoContent() : result.ToProblem();

	}

}
