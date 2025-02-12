namespace SurveyBasket.Services;

public class PollService(ApplicationDbContext context) : IPollService
{
	private ApplicationDbContext _context = context;



	public async Task<IEnumerable<Poll>> GetAllAsync() =>
		await _context.Polls.AsNoTracking().ToListAsync();


	public async Task<Poll?> GetAsync(int id)
	{
		var result = await _context.Polls.FindAsync(id);
		return result;
	}

	public async Task<Poll> AddAsync(Poll poll)
	{
		await _context.AddAsync(poll);
		await _context.SaveChangesAsync();
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
