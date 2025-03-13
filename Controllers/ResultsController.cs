using System.Runtime.CompilerServices;

namespace SurveyBasket.Controllers;
[Route("api/Polls/{pollId}/[controller]")]
[ApiController]
[Authorize]
public class ResultsController(IResultService resultService) : ControllerBase
{
	private readonly IResultService _resultService = resultService;
	[HttpGet("row-data")]
	public async Task<IActionResult> PollVotes([FromRoute]int pollId,CancellationToken cancellationToken)
	{
		var result = await _resultService.GetPollVotesResponseAsync(pollId, cancellationToken);
		return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
	}
}
