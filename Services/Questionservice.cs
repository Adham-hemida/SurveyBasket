using Microsoft.Extensions.Caching.Hybrid;
using SurveyBasket.Contracts.Answers;
using SurveyBasket.Contracts.Common;
using SurveyBasket.Contracts.Questions;
using System.Collections.Generic;

namespace SurveyBasket.Services;

public class Questionservice(ApplicationDbContext context,HybridCache hybridCache,ILogger<Questionservice> logger) : IQuestionService
{
	private readonly ApplicationDbContext _context = context;
	private readonly HybridCache _hybridCache = hybridCache;
	private readonly ILogger<Questionservice> _logger = logger;
	private const string _cachePrefix = "availbleQuestions";

	public async Task<Result<PaginatedList<QuestionResponse>>> GetAllAsync(int pollId,RequestFilters filters, CancellationToken cancellationToken=default)
	{
		var pollIsExists = await _context.Polls.AnyAsync(x => x.Id == pollId, cancellationToken);
		if (!pollIsExists)
			return Result.Failure<PaginatedList<QuestionResponse>>(PollErrors.PollNotFound);

		var query =  _context.Questions
			.Where(x => x.PollId == pollId)
			.Include(x => x.Answers)
			//.Select(q => new QuestionResponse(
			//	q.Id,
			//	q.Content,
			//	q.Answers.Select(answer => new AnswerResponse(answer.Id, answer.Content)
			//	)))
			.ProjectToType<QuestionResponse>()
			.AsNoTracking();

		var questions=await PaginatedList<QuestionResponse>.CreateAsync(query,filters.PageNumber,filters.PageSize,cancellationToken);
		return Result.Success(questions);

	}
	public async Task<Result<IEnumerable<QuestionResponse>>> GetAvailableAsync(int pollId, string userId, CancellationToken cancellationToken = default)
	{
		var hasVote = await _context.Votes.AnyAsync(x => x.PollId == pollId && x.UserId == userId, cancellationToken);
		if (hasVote)
			return Result.Failure<IEnumerable<QuestionResponse>>(VoteErrors.DuplicatedVote);

		var pollIsExsist = await _context.Polls.AnyAsync(x => x.Id == pollId && x.IsPublished && x.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) && x.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow),cancellationToken);
		if (!pollIsExsist)
			return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);

		var cacheKey = $"{_cachePrefix}-{pollId}";
		var questions = await _hybridCache.GetOrCreateAsync<IEnumerable<QuestionResponse>>(cacheKey,
			async cacheEntry=>
			{
				return await _context.Questions
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
		var question = await _context.Questions
			.Where(x => x.PollId == pollId && x.Id == id)
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
		var pollIsExists = await _context.Polls.AnyAsync(x => x.Id == pollId, cancellationToken);

		if (!pollIsExists)
			return Result.Failure<QuestionResponse>(PollErrors.PollNotFound);

		var questionIsExists = await _context.Questions.AnyAsync(x => x.Content == request.Content, cancellationToken);

		if (questionIsExists)
			return Result.Failure<QuestionResponse>(QuestionErrors.DuplicatedQuestionContent);

		var question = request.Adapt<Question>();
		question.PollId = pollId;

		await _context.AddAsync(question, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);

		await _hybridCache.RemoveAsync($"{_cachePrefix}-{pollId}",cancellationToken);
		return Result.Success(question.Adapt<QuestionResponse>());


	}

	public async Task<Result> UpdateAsync(int pollId, int id, QuestionRequest request, CancellationToken cancellationToken = default)
	{
		//check for duplicate
		var questionIsExists = await _context.Questions.AnyAsync(
			x => x.PollId == pollId
			&& x.Id != id
			&& x.Content == request.Content
			, cancellationToken
				);

		if (questionIsExists)
			return Result.Failure(QuestionErrors.DuplicatedQuestionContent);

		var question = await _context.Questions
			.Include(x => x.Answers)
			.SingleOrDefaultAsync(x => x.PollId == pollId && x.Id == id, cancellationToken
			);

		if(question is  null)
			return Result.Failure(QuestionErrors.QuestionNotFound);

		question.Content = request.Content;
		
		//current Answers
		var currentAnswer=question.Answers.Select(x=>x.Content).ToList();

		//new Answers that is not in database
		var newAnswer=request.Answers.Except(currentAnswer).ToList();

		newAnswer.ForEach(answer => {
			question.Answers.Add(new Answer { Content = answer });
		});

		question.Answers.ToList().ForEach(answer => { 
		question.IsActive=request.Answers.Contains(answer.Content);
		});

		await _context.SaveChangesAsync(cancellationToken);

		await _hybridCache.RemoveAsync($"{_cachePrefix}-{pollId}",cancellationToken);

		return Result.Success();

	}

	public async Task<Result> ToggleSatausAsync(int pollId, int id, CancellationToken cancellationToken = default)
	{
		var question = await _context.Questions.SingleOrDefaultAsync(x => x.PollId == pollId && x.Id == id, cancellationToken);

		if (question is null)
			return Result.Failure(QuestionErrors.QuestionNotFound);
		question.IsActive = !question.IsActive;

		await _context.SaveChangesAsync(cancellationToken);

		await _hybridCache.RemoveAsync($"{_cachePrefix}-{pollId}",cancellationToken);

		return Result.Success();
	}

}

