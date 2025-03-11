using SurveyBasket.Errors;
using System.Security.Claims;

namespace SurveyBasket.Controllers;
[Route("api/Polls/{pollId}/Vote")]
[ApiController]
[Authorize]
public class VotesController(IQuestionService questionService) : ControllerBase
{
	private readonly IQuestionService _questionService = questionService;
	[HttpGet("")]
	public async Task<IActionResult> Start([FromRoute] int pollId, CancellationToken cancellationToken)
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		var result = await _questionService.GetAvailableAsync(pollId,userId!, cancellationToken);
		
		if(result.IsSuccess)
			return Ok(result.Value);

		return result.Error.Equals(VoteErrors.DuplicatedVote)
			? result.ToProblem(StatusCodes.Status409Conflict)
			: result.ToProblem(StatusCodes.Status404NotFound);

	}
}
