using SurveyBasket.Contracts.Questions;

namespace SurveyBasket.Controllers;
[Route("api/Polls/{pollId}/[controller]")]
[ApiController]
public class QuestionsController(IQuestionService questionService) : ControllerBase
{
	private readonly IQuestionService _questionService = questionService;

	[HttpGet("")]
	[HasPermission(Permissions.GetQuestions)]
	public async Task<IActionResult> GetAll([FromRoute] int pollId, CancellationToken cancellationToken)
	{
		var result = await _questionService.GetAllAsync(pollId, cancellationToken);
		return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
	}
	[HttpGet("{id}")]
	[HasPermission(Permissions.GetQuestions)]
	public async Task<IActionResult> GetById([FromRoute] int pollId, [FromRoute] int id, CancellationToken cancellationToken)
	{
		var result = await _questionService.GetAsync(pollId, id, cancellationToken);
		return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
	}

	[HttpPost("")]
	[HasPermission(Permissions.AddQuestions)]
	public async Task<IActionResult> Create([FromRoute] int pollId, [FromBody] QuestionRequest questionRequest, CancellationToken cancellationToken)
	{
		var result = await _questionService.AddAsync(pollId, questionRequest, cancellationToken);

		return result.IsSuccess
			? CreatedAtAction(nameof(GetById), new { pollId, result.Value.Id }, result.Value)
			: result.ToProblem();

	}
	[HttpPost("{id}")]
	[HasPermission(Permissions.updateQuestions)]
	public async Task<IActionResult> Update([FromRoute] int pollId, [FromRoute] int id, [FromBody] QuestionRequest questionRequest, CancellationToken cancellationToken)
	{
		var result = await _questionService.UpdateAsync(pollId, id, questionRequest, cancellationToken);
		return result.IsSuccess ? NoContent() : result.ToProblem();
	}

	[HttpPut("{id}/toggleStatus")]
	[HasPermission(Permissions.updateQuestions)]
	public async Task<IActionResult> ToggleStatus([FromRoute] int pollId, [FromRoute] int id, CancellationToken cancellationToken)
	{
		var result = await _questionService.ToggleSatausAsync(pollId, id, cancellationToken);

		return result.IsSuccess	? NoContent(): result.ToProblem();
	}
}
