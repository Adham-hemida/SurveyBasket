namespace SurveyBasket.Services;

public interface IPollService
{
	IEnumerable<Poll> GetAll();
	Poll? GetById(int id);
}
