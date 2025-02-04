namespace SurveyBasket.Controllers;

[Route("api/[controller]")]// /api/Polls
[ApiController]
public class PollsController : ControllerBase
{
	[HttpGet]
	[Route("getall")]
	public IActionResult GetAll()
	{
		return Ok(_polls);
	}
	[HttpGet]
	[Route("get/{id}")]
	public IActionResult GetById(int id)
	{
		var poll=_polls.SingleOrDefault(p => p.Id == id);
		return poll is null ? NotFound() : Ok(poll);
	}

}
