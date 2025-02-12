
namespace SurveyBasket.Services;

public interface IPollService
{
	Task<IEnumerable<Poll>> GetAllAsync( CancellationToken cancellationToken = default);
	Task<Poll?> GetAsync(int id, CancellationToken cancellationToken = default);
	Task<Poll?> AddAsync(Poll poll, CancellationToken cancellationToken=default);
	//bool Update(int id, Poll poll);
	//bool Delete(int id);
}
