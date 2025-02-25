using Microsoft.AspNetCore.Authorization;

namespace SurveyBasket.Controllers;

[Route("api/[controller]")]// /api/Polls
[ApiController]
[Authorize]
public class PollsController(IPollService pollService) : ControllerBase
{
	private readonly IPollService _pollService = pollService;

	[HttpGet("")]
	
	public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
	{
		var polls = await _pollService.GetAllAsync(cancellationToken);
		return Ok(polls);
	}

	[HttpGet("{id}")]
	//[Route("{id}")]
	public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken = default)
	{
		var poll = await _pollService.GetAsync(id,cancellationToken);
		return poll.IsSuccess?
			Ok(poll.Value)
			: NotFound(poll.Error);

	}

	[HttpPost("")]
	//[Route("")]
	public async Task<IActionResult> Create([FromBody] PollRequest request,CancellationToken cancellationToken)
	{
		var createdpoll = await _pollService.AddAsync(request,cancellationToken);
		return CreatedAtAction(nameof(GetById), new { id = createdpoll.Id}, createdpoll);

	}

	[HttpPut("{id}")]
	//[Route("")]
	public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequest request, CancellationToken cancellationToken)
	{
		var result = await _pollService.UpdateAsync(id, request, cancellationToken);
		
		return result.IsSuccess ?
			NoContent()
			: NotFound(result.Error);

	}
	[HttpDelete("{id}")]
	//[Route("")]
	public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
	{
		var result = await _pollService.DeleteAsync(id, cancellationToken);
		return result.IsSuccess ?
			NoContent()
			: NotFound(result.Error);
	}
	[HttpPut("{id}/togglePublish")]

	public async Task<IActionResult> TogglePublish([FromRoute] int id, CancellationToken cancellationToken)
	{
		var result = await _pollService.TogglePublishStatusAsync(id, cancellationToken);
		return (result.IsSuccess ?
			NoContent()
			: NotFound(result.Error));

	}

}
