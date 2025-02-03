using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyBasket.Models;

namespace SurveyBasket.Controllers;
[Route("api/[controller]")]// /api/polls
[ApiController]
public class PollsController : ControllerBase
{
	private static List<Poll> _polls = new List<Poll>();
	[HttpGet]
	public IActionResult Get()
	{
		return Ok(_polls);
	}
}
