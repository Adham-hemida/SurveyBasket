using MapsterMapper;

namespace SurveyBasket.Controllers;

[Route("api/[controller]")]// /api/Polls
[ApiController]
public class PollsController(IPollService pollService) : ControllerBase
{
	private readonly IPollService _pollService = pollService;

	[HttpGet("getall")]
	//[Route("getall")]
	public IActionResult GetAll()
	{
		var polls = _pollService.GetAll();
		return Ok(polls);
	}

	[HttpGet("get/{id}")]
	//[Route("get/{id}")]
	public IActionResult GetById([FromRoute] int id)
	{
		var poll=_pollService.GetById(id);
		if (poll is null)
			return NotFound();

		var response =poll.Adapt<PollResponse>();
		return Ok(response);

	}

	[HttpPost("Create")]
	//[Route("Create")]
	public IActionResult Create([FromBody] CreatePollRequest request)
	{
		var createdpoll= _pollService.Create(request.Adapt<Poll>());
		//	var createdPoll = _pollService.Create((Poll)request);
		//return CreatedAtAction(nameof(GetById), new { id = createdPoll.Id }, createdPoll);
		return CreatedAtAction(nameof(GetById), new { id = createdpoll.Id },createdpoll);	

	}

	[HttpPut("Update/{id}")]
	//[Route("Update")]
	public IActionResult Update([FromRoute] int id,[FromBody]CreatePollRequest request)
	{
	//	var updated = _pollService.Update(id,((Poll)request));
	//	if(!updated)
	//		return NotFound();
	//	else
		return NoContent();

	}
	[HttpDelete("Delete/{id}")]
	//[Route("Delete")]
	public IActionResult Delete([FromRoute] int id)
	{
		var isDeleted= _pollService.Delete(id);
		if (!isDeleted)
			return NotFound();
		else
			return NoContent();
	}

}
