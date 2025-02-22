using Microsoft.AspNetCore.Authorization;

namespace SurveyBasket.Controllers;

[Route("api/[controller]")]// /api/Polls
[ApiController]
[Authorize]
public class PollsController(IPollService pollService) : ControllerBase
{
	private readonly IPollService _pollService = pollService;

	[HttpGet("getall")]
	
	public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
	{
		var polls = await _pollService.GetAllAsync(cancellationToken);
		var response = polls.Adapt<IEnumerable<PollResponse>>();
		return Ok(response);
	}

	[HttpGet("get/{id}")]
	//[Route("get/{id}")]
	public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken = default)
	{
		var poll = await _pollService.GetAsync(id,cancellationToken);
		if (poll is null)
			return NotFound();

		var response = poll.Adapt<PollResponse>();
		return Ok(response);

	}

	[HttpPost("Create")]
	//[Route("Create")]
	public async Task<IActionResult> Create([FromBody] PollRequest request,CancellationToken cancellationToken)
	{
		var createdpoll = await _pollService.AddAsync(request.Adapt<Poll>(), cancellationToken);
		return CreatedAtAction(nameof(GetById), new { id = createdpoll.Id }, createdpoll);

	}

	[HttpPut("Update/{id}")]
	//[Route("Update")]
	public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequest request, CancellationToken cancellationToken)
	{
		var updated =await _pollService.UpdateAsync(id, request.Adapt<Poll>(), cancellationToken);
		if (!updated)
			return NotFound();
		else
			return NoContent();

	}
	[HttpDelete("Delete/{id}")]
	//[Route("Delete")]
	public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
	{
		var isDeleted = await _pollService.DeleteAsync(id, cancellationToken);
		if (!isDeleted)
			return NotFound();
		else
			return NoContent();
	}
	[HttpPut("{id}/togglePublish")]
	
	public async Task<IActionResult> TogglePublish([FromRoute] int id,  CancellationToken cancellationToken)
	{
		var updated = await _pollService.TogglePublishStatusAsync(id, cancellationToken);
		if (!updated)
			return NotFound();
		else
			return NoContent();

	}

}
