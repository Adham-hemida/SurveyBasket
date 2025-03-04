using SurveyBasket.Contracts.Answers;
using SurveyBasket.Contracts.Questions;
using SurveyBasket.Errors;
using System.Linq;

namespace SurveyBasket.Services;

public class Questionservice(ApplicationDbContext context) : IQuestionService
{
	private readonly ApplicationDbContext _context = context;

	public async Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int pollId, CancellationToken cancellationToken)
	{
		var pollIsExists = await _context.Polls.AnyAsync(x => x.Id == pollId, cancellationToken);
		if (!pollIsExists)
			return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);

		var questions = await _context.Questions
			.Where(x => x.PollId == pollId)
			.Include(x => x.Answers)
			//.Select(q => new QuestionResponse(
			//	q.Id,
			//	q.Content,
			//	q.Answers.Select(answer => new AnswerResponse(answer.Id, answer.Content)
			//	)))
			.ProjectToType<QuestionResponse>()
			.AsNoTracking()
			.ToListAsync(cancellationToken);
		return Result.Success<IEnumerable<QuestionResponse>>(questions);

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
		return Result.Success();

	}

	public async Task<Result> ToggleSatausAsync(int pollId, int id, CancellationToken cancellationToken = default)
	{
		var question = await _context.Questions.SingleOrDefaultAsync(x => x.PollId == pollId && x.Id == id, cancellationToken);

		if (question is null)
			return Result.Failure(QuestionErrors.QuestionNotFound);
		question.IsActive = !question.IsActive;

		await _context.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}

