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
		return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
	}
	[HttpGet("current")]

	public async Task<IActionResult> GetCurrent(CancellationToken cancellationToken = default)
	{
		var result = await _pollService.GetCurrentAsync(cancellationToken);
		return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
	}

	[HttpGet("{id}")]
	//[Route("{id}")]
	public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken = default)
	{
		var result = await _pollService.GetAsync(id, cancellationToken);
		return result.IsSuccess ? Ok(result.Value) : result.ToProblem();

	}

	[HttpPost("")]
	//[Route("")]
	public async Task<IActionResult> Create([FromBody] PollRequest request, CancellationToken cancellationToken)
	{
		var result = await _pollService.AddAsync(request, cancellationToken);

		return result.IsSuccess
			? CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value)
			: result.ToProblem();


	}

	[HttpPut("{id}")]
	//[Route("")]
	public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequest request, CancellationToken cancellationToken)
	{
		var result = await _pollService.UpdateAsync(id, request, cancellationToken);
		return result.IsSuccess ? NoContent() : result.ToProblem();




	}
	[HttpDelete("{id}")]
	//[Route("")]
	public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
	{
		var result = await _pollService.DeleteAsync(id, cancellationToken);
		return result.IsSuccess ? NoContent() : result.ToProblem();
	}
	[HttpPut("{id}/togglePublish")]

	public async Task<IActionResult> TogglePublish([FromRoute] int id, CancellationToken cancellationToken)
	{
		var result = await _pollService.TogglePublishStatusAsync(id, cancellationToken);
		return result.IsSuccess ? NoContent() : result.ToProblem();

	}

}
