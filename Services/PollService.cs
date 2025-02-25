using SurveyBasket.Errors;
using System.Reflection.Metadata.Ecma335;

namespace SurveyBasket.Services;

public class PollService(ApplicationDbContext context) : IPollService
{
	private ApplicationDbContext _context = context;



	public async Task<IEnumerable<PollResponse>> GetAllAsync( CancellationToken cancellationToken = default)
	{  var result= await _context.Polls.AsNoTracking().ToListAsync(cancellationToken);
		return  result.Adapt<IEnumerable<PollResponse>>();
	}
	
	     


	public async Task<Result<PollResponse>> GetAsync(int id, CancellationToken cancellationToken = default)
	{
		var poll = await _context.Polls.FindAsync(id, cancellationToken);
        return poll is not null ? 
			Result.Success(poll.Adapt<PollResponse>())
			: Result.Failure<PollResponse>(PollErrors.PollNotFound);

	}

	public async Task<PollResponse> AddAsync(PollRequest request, CancellationToken cancellationToken = default)
	{
		// first i need to conert it into domain model (Poll) and then save it in the database as database accepting domain model(Poll)
		var poll = request.Adapt<Poll>();

		await _context.AddAsync(poll, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);
		return poll.Adapt<PollResponse>();
	}

	public async Task<Result> UpdateAsync(int id, PollRequest poll, CancellationToken cancellationToken = default)
	{
		var currentPoll = await _context.Polls.FindAsync(id, cancellationToken);
		if (currentPoll is null)
			return Result.Failure(PollErrors.PollNotFound);

		currentPoll.Title = poll.Title;
		currentPoll.Summary = poll.Summary;
		currentPoll.StartsAt = poll.StartsAt;
		currentPoll.EndsAt = poll.EndsAt;

		await _context.SaveChangesAsync(cancellationToken);

		return Result.Success();


	}

	public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
	{
		var poll = await _context.Polls.FindAsync(id, cancellationToken);
		if (poll is null)
			return Result.Failure(PollErrors.PollNotFound);
		_context.Remove(poll);
		await _context.SaveChangesAsync(cancellationToken);
		return Result.Success();
	}

	public async Task<Result> TogglePublishStatusAsync(int id, CancellationToken cancellationToken = default)
	{
		var poll = await _context.Polls.FindAsync(id, cancellationToken);
		if (poll is null)
			return Result.Failure(PollErrors.PollNotFound);
		poll.IsPublished = !poll.IsPublished;
		await _context.SaveChangesAsync(cancellationToken);
		return Result.Success();
	}
}
