using Microsoft.Extensions.Caching.Hybrid;
using SurveyBasket.Contracts.Answers;
using SurveyBasket.Contracts.Common;
using SurveyBasket.Contracts.Questions;
using SurveyBasket.UnitOfWorks;
using System.Linq.Dynamic.Core;

namespace SurveyBasket.Services;

public class Questionservice(ApplicationDbContext context, HybridCache hybridCache, ILogger<Questionservice> logger,IUnitOfWork unitOfWork) : IQuestionService
{
	private readonly ApplicationDbContext _context = context;
	private readonly HybridCache _hybridCache = hybridCache;
	private readonly ILogger<Questionservice> _logger = logger;
	private readonly IUnitOfWork _unitOfWork = unitOfWork;
	private const string _cachePrefix = "availbleQuestions";

	public async Task<Result<PaginatedList<QuestionResponse>>> GetAllAsync(int pollId, RequestFilters filters, CancellationToken cancellationToken = default)
	{
		var pollIsExists = await _unitOfWork.Repository<Poll>()
			.AnyAsync(x => x.Id == pollId, cancellationToken: cancellationToken);

		if (!pollIsExists)
			return Result.Failure<PaginatedList<QuestionResponse>>(PollErrors.PollNotFound);

		var query = _unitOfWork.Repository<Question>()
			.GetAsQueryable().Where(x => x.PollId == pollId);
			

		if (!string.IsNullOrEmpty(filters.SearchValue))
		{
			query = query.Where(x => x.Content.Contains(filters.SearchValue));
		}
		if (!string.IsNullOrEmpty(filters.SortColumn))
		{
			query = query.OrderBy($"{filters.SortColumn} {filters.SortDirection}");
		}

		var source = query.Include(x => x.Answers)
			//.Select(q => new QuestionResponse(
			//	q.Id,
			//	q.Content,
			//	q.Answers.Select(answer => new AnswerResponse(answer.Id, answer.Content)
			//	)))
			.ProjectToType<QuestionResponse>()
			.AsNoTracking();

		var questions = await PaginatedList<QuestionResponse>.CreateAsync(source, filters.PageNumber, filters.PageSize, cancellationToken);
		return Result.Success(questions);

	}
	public async Task<Result<IEnumerable<QuestionResponse>>> GetAvailableAsync(int pollId, string userId, CancellationToken cancellationToken = default)
	{
		var hasVote = await _unitOfWork.Repository<Vote>()
			.AnyAsync(x => x.PollId == pollId && x.UserId == userId, cancellationToken: cancellationToken);
		
		if (hasVote)
			return Result.Failure<IEnumerable<QuestionResponse>>(VoteErrors.DuplicatedVote);

		var pollIsExsist = await _unitOfWork.Repository<Poll>()
			.AnyAsync(x => x.Id == pollId && x.IsPublished && x.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) && x.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken: cancellationToken);
		
		if (!pollIsExsist)
			return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);

		var cacheKey = $"{_cachePrefix}-{pollId}";
		var questions = await _hybridCache.GetOrCreateAsync<IEnumerable<QuestionResponse>>(cacheKey,
			async cacheEntry =>
			{
				
				return await _unitOfWork.Repository<Question>()
				.GetAsQueryable()
			    .Where(x => x.IsActive && x.PollId == pollId)
			    .Include(x => x.Answers)
			    .Select(q => new QuestionResponse(
				q.Id,
				q.Content,
				q.Answers.Where(x => x.IsActive).Select(answer => new AnswerResponse(answer.Id, answer.Content))
				))
			.AsNoTracking()
			.ToListAsync(cancellationToken);
			});



		return Result.Success(questions);
	}


	public async Task<Result<QuestionResponse>> GetAsync(int pollId, int id, CancellationToken cancellationToken = default)
	{
		

		var question=await _unitOfWork.Repository<Question>()
			.GetAsQueryable().Where(x => x.PollId == pollId && x.Id == id)
			.Include(x => x.Answers)
			.ProjectToType<QuestionResponse>()
			.AsNoTracking()
			.SingleOrDefaultAsync(cancellationToken);

		if (question is null)
			return Result.Failure<QuestionResponse>(QuestionErrors.QuestionNotFound);

		return Result.Success(question);

	}


	public async Task<Result<QuestionResponse>> AddAsync(int pollId, QuestionRequest request, CancellationToken cancellationToken)
	{
		var pollIsExists = await _unitOfWork.Repository<Poll>()
			.AnyAsync(x => x.Id == pollId, cancellationToken);

		if (!pollIsExists)
			return Result.Failure<QuestionResponse>(PollErrors.PollNotFound);

		var questionIsExists = await _unitOfWork.Repository<Question>()
			.GetAsQueryable().AnyAsync(x => x.Content == request.Content, cancellationToken);
			

		if (questionIsExists)
			return Result.Failure<QuestionResponse>(QuestionErrors.DuplicatedQuestionContent);

		var question = request.Adapt<Question>();
		question.PollId = pollId;

		await _unitOfWork.Repository<Question>().CreateAsync(question, cancellationToken);
		await _unitOfWork.CompleteAsync(cancellationToken);

		await _hybridCache.RemoveAsync($"{_cachePrefix}-{pollId}", cancellationToken);
		return Result.Success(question.Adapt<QuestionResponse>());


	}

	public async Task<Result> UpdateAsync(int pollId, int id, QuestionRequest request, CancellationToken cancellationToken = default)
	{
	
		var questionIsExists = await _unitOfWork.Repository<Question>()
			.GetAsQueryable().AnyAsync(
				x => x.PollId == pollId
				&& x.Id != id
				&& x.Content == request.Content, cancellationToken: cancellationToken
			);

		if (questionIsExists)
			return Result.Failure(QuestionErrors.DuplicatedQuestionContent);


		var question = await _unitOfWork.Repository<Question>()
			.FindAsync(x => x.PollId == pollId && x.Id == id,  ["Answers"], cancellationToken);


		if (question is null)
			return Result.Failure(QuestionErrors.QuestionNotFound);

		question.Content = request.Content;

		//current Answers
		var currentAnswer = question.Answers.Select(x => x.Content).ToList();

		//new Answers that is not in database
		var newAnswer = request.Answers.Except(currentAnswer).ToList();

		newAnswer.ForEach(answer =>
		{
			question.Answers.Add(new Answer { Content = answer });
		});

		question.Answers.ToList().ForEach(answer =>
		{
			question.IsActive = request.Answers.Contains(answer.Content);
		});

		await _unitOfWork.CompleteAsync(cancellationToken);

		await _hybridCache.RemoveAsync($"{_cachePrefix}-{pollId}", cancellationToken);

		return Result.Success();

	}

	public async Task<Result> ToggleSatausAsync(int pollId, int id, CancellationToken cancellationToken = default)
	{
		var question = await _unitOfWork.Repository<Question>()
			.FindAsync(x => x.PollId == pollId && x.Id == id, cancellationToken: cancellationToken);

		if (question is null)
			return Result.Failure(QuestionErrors.QuestionNotFound);
		question.IsActive = !question.IsActive;

		await _unitOfWork.CompleteAsync(cancellationToken);

		await _hybridCache.RemoveAsync($"{_cachePrefix}-{pollId}", cancellationToken);

		return Result.Success();
	}

}

