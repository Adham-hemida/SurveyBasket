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
		return Ok(_pollService.GetAll());
	}

	[HttpGet("get/{id}")]
	//[Route("get/{id}")]
	public IActionResult GetById([FromRoute] int id)
	{
		var poll=_pollService.GetById(id);
		return poll is null ? NotFound() : Ok(poll.MapToResponse());
	}

	[HttpPost("Create")]
	//[Route("Create")]
	public IActionResult Create([FromBody] CreatePollRequest request)
	{
		var createdPoll = _pollService.Create(request.mapToPoll());
		return CreatedAtAction(nameof(GetById), new { id = createdPoll.Id }, createdPoll);
	}

	[HttpPut("Update/{id}")]
	//[Route("Update")]
	public IActionResult Update([FromRoute] int id,[FromBody]CreatePollRequest request)
	{
		var updated = _pollService.Update(id, request.mapToPoll());
		if(!updated)
			return NotFound();
		else
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
