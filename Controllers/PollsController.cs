using SurveyBasket.Services;

namespace SurveyBasket.Controllers;

[Route("api/[controller]")]// /api/Polls
[ApiController]
public class PollsController(IPollService pollService) : ControllerBase
{
	private readonly IPollService _pollService = pollService;

	[HttpGet]
	[Route("getall")]
	public IActionResult GetAll()
	{
		return Ok(_pollService.GetAll());
	}
	[HttpGet]
	[Route("get/{id}")]
	public IActionResult GetById(int id)
	{
		var poll=_pollService.GetById(id);
		return poll is null ? NotFound() : Ok(poll);
	}

	public IPollService Get_pollService()
	{
		return _pollService;
	}

	[HttpPost]
	[Route("")]
	public IActionResult Create(Poll poll)
	{
		
	}

}
