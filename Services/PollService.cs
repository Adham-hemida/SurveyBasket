namespace SurveyBasket.Services;

public class PollService : IPollService
{
	private static List<Poll> _polls = new List<Poll> {
		new Poll{Id=1,Title="First Poll",Summary="This is the first poll"},
		new Poll{Id=2,Title="Second Poll",Summary="This is the second poll"},
	};

	

	public IEnumerable<Poll> GetAll() => _polls;


	public Poll? GetById(int id) => _polls.SingleOrDefault(p => p.Id == id);

	public Poll Create(Poll poll)
	{
		poll.Id = _polls.Count + 1;
		_polls.Add(poll);
		return poll;
	}

	public bool Update(int id, Poll poll)
	{
		var currentPoll = GetById(id);
		if (currentPoll is null)
			return false;

		currentPoll.Title = poll.Title;
		currentPoll.Summary = poll.Summary;
		return true;


	}

	public bool Delete(int id)
	{
		var poll = GetById(id);
		if (poll is null)
			return false;
		_polls.Remove(poll);
		return true;
	}
}
