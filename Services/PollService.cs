namespace SurveyBasket.Services;

public class PollService : IPollService
{
	private static List<Poll> _polls = new List<Poll> {
		new Poll{Id=1,Title="First Poll",Description="This is the first poll"},
		new Poll{Id=2,Title="Second Poll",Description="This is the second poll"},
	};

	public IEnumerable<Poll> GetAll()=> _polls;


	public Poll? GetById(int id) => _polls.SingleOrDefault(p => p.Id == id);

}
