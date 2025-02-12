namespace SurveyBasket.Services;

public class PollService(ApplicationDbContext context) : IPollService
{
	private ApplicationDbContext _context = context;



	public async Task<IEnumerable<Poll>> GetAllAsync( CancellationToken cancellationToken = default) =>
		await _context.Polls.AsNoTracking().ToListAsync(cancellationToken);


	public async Task<Poll?> GetAsync(int id, CancellationToken cancellationToken = default)
	{
		var result = await _context.Polls.FindAsync(id, cancellationToken);
		return result;
	}

	public async Task<Poll> AddAsync(Poll poll, CancellationToken cancellationToken=default)
	{
		await _context.AddAsync(poll,cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);
		return poll;
	}

	//public bool Update(int id, Poll poll)
	//{
	//	var currentPoll = GetById(id);
	//	if (currentPoll is null)
	//		return false;

	//	currentPoll.Title = poll.Title;
	//	currentPoll.Summary = poll.Summary;
	//	return true;


	//}

	//public bool Delete(int id)
	//{
	//	var poll = GetById(id);
	//	if (poll is null)
	//		return false;
	//	_polls.Remove(poll);
	//	return true;
	//}
}
