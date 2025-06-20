
using SurveyBasket.UnitOfWorks;

namespace SurveyBasket.Services;

public class ResultService(ApplicationDbContext context,IUnitOfWork unitOfWork) : IResultService
{
	private readonly ApplicationDbContext _context = context;
	private readonly IUnitOfWork _unitOfWork = unitOfWork;

	public async Task<Result<PollVotesResponse>> GetPollVotesAsync(int pollId, CancellationToken cancellationToken = default)
	{

		var pollVotes = await _unitOfWork.Repository<Poll>()
			.GetAsQueryable()
			.Where(x => x.Id == pollId)
			.Select(x => new PollVotesResponse(
				x.Title,
				x.Votes.Select(v => new VoteResponse(
					$"{v.User.FirstName} {v.User.LastName}",
					v.SubmittedOn,
					v.VoteAnswers.Select(a => new QuestionAnswerResponse(
						a.Question.Content,
						a.Answer.Content
					))
				))
			))
			.SingleOrDefaultAsync(cancellationToken);
		return pollVotes is null ? Result.Failure<PollVotesResponse>(PollErrors.PollNotFound)
			: Result.Success(pollVotes);

	}

	public async Task<Result<IEnumerable<VotesPerDayResponse>>> GetVotesPerDayAsync(int pollId, CancellationToken cancellationToken = default)
	{
		var pollIsExists = await _unitOfWork.Repository<Poll>()
			.AnyAsync(x => x.Id == pollId, cancellationToken);

		if (!pollIsExists)
			return Result.Failure<IEnumerable<VotesPerDayResponse>>(PollErrors.PollNotFound);


		var VotesPerDay = await _unitOfWork.Repository<Vote>()
			.GetAsQueryable()
			.Where(x => x.PollId == pollId)
			.GroupBy(x => new { voteDate = DateOnly.FromDateTime(x.SubmittedOn) })
			.Select(g => new VotesPerDayResponse(
				g.Key.voteDate,
				g.Count()))
			.ToListAsync(cancellationToken);
		return Result.Success<IEnumerable<VotesPerDayResponse>>(VotesPerDay);
	}

	public async Task<Result<IEnumerable<VotesPerQuestionResponse>>> GetVotesPerQuestionAsync(int pollId, CancellationToken cancellationToken = default)
	{
		var pollIsExists = await _unitOfWork.Repository<Poll>()
			.AnyAsync(x => x.Id == pollId, cancellationToken);

		if (!pollIsExists)
			return Result.Failure<IEnumerable<VotesPerQuestionResponse>>(PollErrors.PollNotFound);


		var VotesPerQuestion = await _unitOfWork.Repository<VoteAnswer>()
			.GetAsQueryable()
			.Where(x => x.Vote.PollId == pollId)
			.Select(x => new VotesPerQuestionResponse(
				x.Question.Content,
				x.Question.Votes
					.GroupBy(g => new { AnswerId = g.Answer.Id, AnswerContent = g.Answer.Content })
					.Select(g => new VotesPerAnswerResponse(
						g.Key.AnswerContent,
						g.Count()
					))
			)).ToListAsync(cancellationToken);


		return Result.Success<IEnumerable<VotesPerQuestionResponse>>(VotesPerQuestion);
	}
}
