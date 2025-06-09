using SurveyBasket.Errors;

namespace SurveyBasket.Services;

public class PollService(ApplicationDbContext context) : IPollService
{
	private ApplicationDbContext _context = context;



	public async Task<Result<IEnumerable<PollResponse>>> GetAllAsync( CancellationToken cancellationToken = default)
	{  var result= await _context.Polls
			.ProjectToType<PollResponse>()
			.AsNoTracking().ToListAsync(cancellationToken);

		return Result.Success<IEnumerable<PollResponse>>( result);
	}

	public async Task<Result<IEnumerable<PollResponse>>> GetCurrentAsyncV1( CancellationToken cancellationToken = default)
	{  var result= await _context.Polls
			.Where(x=>x.IsPublished && x.StartsAt<DateOnly.FromDateTime(DateTime.Now) && x.EndsAt > DateOnly.FromDateTime(DateTime.Now))
			.AsNoTracking()
			.ProjectToType<PollResponse>()
			.ToListAsync(cancellationToken);

		return Result.Success<IEnumerable<PollResponse>>( result);
	}


	public async Task<Result<IEnumerable<PollResponseV2>>> GetCurrentAsyncV2( CancellationToken cancellationToken = default)
	{  var result= await _context.Polls
			.Where(x=>x.IsPublished && x.StartsAt<DateOnly.FromDateTime(DateTime.Now) && x.EndsAt > DateOnly.FromDateTime(DateTime.Now))
			.AsNoTracking()
			.ProjectToType<PollResponseV2>()
			.ToListAsync(cancellationToken);

		return Result.Success<IEnumerable<PollResponseV2>>( result);
	}
	
	     


	public async Task<Result<PollResponse>> GetAsync(int id, CancellationToken cancellationToken = default)
	{
		var poll = await _context.Polls.FindAsync(id, cancellationToken);
        return poll is not null ? 
			Result.Success(poll.Adapt<PollResponse>())
			: Result.Failure<PollResponse>(PollErrors.PollNotFound);

	}

	public async Task<Result<PollResponse>> AddAsync(PollRequest request, CancellationToken cancellationToken = default)
	{
		var isExisting= await _context.Polls.AnyAsync(p => p.Title == request.Title, cancellationToken);
		if (isExisting)
			return Result.Failure<PollResponse>(PollErrors.DuplicatedPollTitle);
		// first i need to conert it into domain model (Poll) and then save it in the database as database accepting domain model(Poll)
		var poll = request.Adapt<Poll>();

		await _context.AddAsync(poll, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);

		return Result.Success( poll.Adapt<PollResponse>());
	}

	public async Task<Result> UpdateAsync(int id, PollRequest request, CancellationToken cancellationToken = default)
	{
		var currentPoll = await _context.Polls.FindAsync(id, cancellationToken);
		if (currentPoll is null)
			return Result.Failure(PollErrors.PollNotFound);

		var isExisting = await _context.Polls.AnyAsync(p => p.Title == request.Title && p.Id != id, cancellationToken);
		if (isExisting)
			return Result.Failure(PollErrors.DuplicatedPollTitle);

		currentPoll = request.Adapt(currentPoll);

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
