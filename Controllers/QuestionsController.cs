using SurveyBasket.Contracts.Questions;
using SurveyBasket.Errors;

namespace SurveyBasket.Controllers;
[Route("api/Polls/{pollId}/[controller]")]
[ApiController]
[Authorize]
public class QuestionsController(IQuestionService questionService) : ControllerBase
{
	private readonly IQuestionService _questionService = questionService;

	[HttpGet("")]
	public async Task<IActionResult> GetAll([FromRoute]int pollId,CancellationToken cancellationToken)
	{
		var result = await _questionService.GetAllAsync(pollId, cancellationToken);
		return result.IsSuccess
			?Ok(result.Value)
			:result.ToProblem(statusCode:StatusCodes.Status404NotFound);
	}
	[HttpGet("{id}")]
	public IActionResult Get()
	{
		return Ok();
	}

	[HttpPost("")]
	public  async Task<IActionResult> Create([FromRoute]int pollId,[FromBody]QuestionRequest questionRequest,CancellationToken cancellationToken)
	{
		var result=await _questionService.AddAsync(pollId,questionRequest,cancellationToken);

		if (result.IsSuccess)
			return CreatedAtAction(nameof(Get),new {pollId,result.Value.Id},result.Value);

		return result.Error.Equals(QuestionErrors.QuestionNotFound)
			? result.ToProblem(statusCode: StatusCodes.Status404NotFound)
			:result.ToProblem(statusCode: StatusCodes.Status409Conflict);

	}
}
