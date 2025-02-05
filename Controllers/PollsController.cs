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
	public IActionResult GetById(int id)
	{
		var poll=_pollService.GetById(id);
		return poll is null ? NotFound() : Ok(poll);
	}


	[HttpPost("Create")]
	//[Route("Create")]
	public IActionResult Create(Poll poll)
	{
		var createdPoll = _pollService.Create(poll);
		return CreatedAtAction(nameof(GetById), new { id = createdPoll.Id }, createdPoll);

	}

}
