using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SurveyBasket.Controllers;

[Route("api/[controller]")]// /api/Polls
[ApiController]
public class PollsController(IPollService pollService) : ControllerBase
{
	private readonly IPollService _pollService = pollService;

	[HttpGet("getall")]
	//[Route("getall")]
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
	public async Task<IActionResult> Create([FromBody] CreatePollRequest request,CancellationToken cancellationToken)
	{
		var createdpoll = await _pollService.AddAsync(request.Adapt<Poll>(), cancellationToken);
		return CreatedAtAction(nameof(GetById), new { id = createdpoll.Id }, createdpoll);

	}

	//[HttpPut("Update/{id}")]
	////[Route("Update")]
	//public IActionResult Update([FromRoute] int id,[FromBody]CreatePollRequest request)
	//{
	//	var updated = _pollService.Update(id, request.Adapt<Poll>());
	//	if (!updated)
	//		return NotFound();
	//	else
	//		return NoContent();

	//}
	//[HttpDelete("Delete/{id}")]
	////[Route("Delete")]
	//public IActionResult Delete([FromRoute] int id)
	//{
	//	var isDeleted= _pollService.Delete(id);
	//	if (!isDeleted)
	//		return NotFound();
	//	else
	//		return NoContent();
	//}

}
