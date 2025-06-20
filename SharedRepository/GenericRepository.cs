using System.Linq.Expressions;
using System.Threading;

namespace SurveyBasket.SharedRepository;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
	private readonly ApplicationDbContext _context;
	private readonly DbSet<T> _dbSet;
	public GenericRepository(ApplicationDbContext context, DbSet<T> _dbSet)
	{
		_context = context;
		_dbSet = _context.Set<T>();
	}

	public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
	{
		return await _dbSet.AnyAsync(predicate, cancellationToken);
	}

	public async Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default)
	{
		await _dbSet.AddAsync(entity, cancellationToken);
		return entity;
	}

	public void DeleteAsync(T entity)
	{ 
		_dbSet.Remove(entity);
	}

	public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, string[] includes = null, CancellationToken cancellationToken = default)
	{
		IQueryable<T> query = _dbSet;

		if (includes != null)
			foreach (var include in includes)
				query = query.Include(include);

		return await query.Where(criteria).ToListAsync(cancellationToken);
	}

	public async Task<IEnumerable<TDestination>> FindAllProjectedAsync<TDestination>(Expression<Func<T, bool>>? criteria = null, CancellationToken cancellationToken = default)
	{
		IQueryable<T> query = _dbSet;

		if (criteria != null)
			query = query.Where(criteria);

		return await query
			.AsNoTracking()
			.ProjectToType<TDestination>()
			.ToListAsync(cancellationToken);
	}

	public async Task<IEnumerable<TDestination>> FindAllProjectedWithSelect<TDestination>(Expression<Func<T, TDestination>> selector, Expression<Func<T, bool>>? criteria = null, CancellationToken cancellationToken = default)
	{
		IQueryable<T> query = _dbSet;

		if (criteria != null)
			query = query.Where(criteria);

		return await query
			.AsNoTracking()
			.Select(selector)
			.ToListAsync(cancellationToken);
	}

	public async Task<T> FindAsync(Expression<Func<T, bool>> criteria, string[] includes = null,CancellationToken cancellationToken = default)
	{
		IQueryable<T> query = _dbSet;

		if (includes != null)
			foreach (var include in includes)
				query = query.Include(include);

		return await query.SingleOrDefaultAsync(criteria,cancellationToken);
	}

	public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		return await _dbSet.AsNoTracking().ToListAsync(cancellationToken);
	}

	public IQueryable<T> GetAsQueryable()
	{
		return _dbSet.AsQueryable();
	}

	public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
	{
		return await _dbSet.FindAsync(id,cancellationToken);
	}

	public T UpdateAsync(T entity)
	{
		_dbSet.Update(entity);
		return  entity;
	}
}
