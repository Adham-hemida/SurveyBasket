namespace SurveyBasket.Services;

public interface IPollService
{
	IEnumerable<Poll> GetAll();
	Poll? GetById(int id);
	Poll Create(Poll poll);
	bool Update(int id, Poll poll);
}
