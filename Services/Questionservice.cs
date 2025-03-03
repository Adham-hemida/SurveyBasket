using SurveyBasket.Contracts.Answers;
using SurveyBasket.Contracts.Questions;
using SurveyBasket.Errors;

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
			.ToListAsync();
		return Result.Success<IEnumerable<QuestionResponse>>(questions);

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

}
