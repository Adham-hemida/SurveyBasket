
namespace SurveyBasket.Services;

public interface IPollService
{
	Task<IEnumerable<Poll>> GetAllAsync( CancellationToken cancellationToken = default);
	Task<Poll?> GetAsync(int id, CancellationToken cancellationToken = default);
	Task<Poll?> AddAsync(Poll poll, CancellationToken cancellationToken=default);
	Task<bool> UpdateAsync(int id, Poll poll, CancellationToken cancellationToken = default);
	Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
	Task<bool> TogglePublishStatusAsync(int id, CancellationToken cancellationToken = default);
}
