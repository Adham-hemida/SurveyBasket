using SurveyBasket.SharedRepository;

namespace SurveyBasket.UnitOfWorks;

public interface IUnitOfWork:IDisposable
{
	IGenericRepository<T> Repository<T>() where T : class;
	Task<int> CompleteAsync(CancellationToken cancellationToken = default);
}
