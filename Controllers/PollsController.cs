using SurveyBasket.Errors;

namespace SurveyBasket.Controllers;

[Route("api/[controller]")]// /api/Polls
[ApiController]
[Authorize]
public class PollsController(IPollService pollService) : ControllerBase
{
	private readonly IPollService _pollService = pollService;

	[HttpGet("")]

	public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
	{
		var result = await _pollService.GetAllAsync(cancellationToken);
		return result.IsSuccess
			?Ok(result.Value)
			:result.ToProblem(statusCode:StatusCodes.Status404NotFound);
	}

	[HttpGet("{id}")]
	//[Route("{id}")]
	public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken = default)
	{
		var result = await _pollService.GetAsync(id, cancellationToken);
		return result.IsSuccess
			? Ok(result.Value)
			: result.ToProblem(statusCode: StatusCodes.Status404NotFound);

	}

	[HttpPost("")]
	//[Route("")]
	public async Task<IActionResult> Create([FromBody] PollRequest request, CancellationToken cancellationToken)
	{
		var result = await _pollService.AddAsync(request, cancellationToken);
		
		return result.IsSuccess
			? CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value)
			: result.ToProblem(statusCode: StatusCodes.Status409Conflict);
		

	}

	[HttpPut("{id}")]
	//[Route("")]
	public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequest request, CancellationToken cancellationToken)
	{
		var result = await _pollService.UpdateAsync(id, request, cancellationToken);
		if (result.IsSuccess)
			return NoContent();

		return result.Error.Equals(PollErrors.DuplicatedPollTitle)
			? result.ToProblem(statusCode: StatusCodes.Status409Conflict)
			: result.ToProblem(statusCode: StatusCodes.Status404NotFound);



	}
	[HttpDelete("{id}")]
	//[Route("")]
	public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
	{
		var result = await _pollService.DeleteAsync(id, cancellationToken);
		return result.IsSuccess
			? NoContent()
			: result.ToProblem(statusCode: StatusCodes.Status404NotFound);
	}
	[HttpPut("{id}/togglePublish")]

	public async Task<IActionResult> TogglePublish([FromRoute] int id, CancellationToken cancellationToken)
	{
		var result = await _pollService.TogglePublishStatusAsync(id, cancellationToken);
		return (result.IsSuccess
			 ? NoContent()
			: result.ToProblem(statusCode: StatusCodes.Status404NotFound));

	}

}
