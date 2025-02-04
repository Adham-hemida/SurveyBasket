namespace SurveyBasket.Controllers;

[Route("api/[controller]")]// /api/Polls
[ApiController]
public class PollsController : ControllerBase
{
	private static List<Poll> _polls = new List<Poll> {
		new Poll{Id=1,Title="First Poll",Description="This is the first poll"},
		new Poll{Id=2,Title="Second Poll",Description="This is the second poll"},
	};
	[HttpGet]
	[Route("getall")]
	public IActionResult Get()
	{
		return Ok(_polls);
	}
	[HttpGet]
	[Route("get/{id}")]
	public IActionResult Get(int id)
	{
		var poll=_polls.SingleOrDefault(p => p.Id == id);
		return poll is null ? NotFound() : Ok(poll);
	}

}
