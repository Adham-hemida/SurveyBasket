
namespace SurveyBasket.Services;

public class ResultService(ApplicationDbContext context) : IResultService
{
	private readonly ApplicationDbContext _context = context;

	public async Task<Result<PollVotesResponse>> GetPollVotesAsync(int pollId, CancellationToken cancellationToken = default)
	{
		var pollVotes = await _context.Polls
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
		var pollIsExists =await  _context.Polls.AnyAsync(x => x.Id == pollId,cancellationToken);

		if (!pollIsExists)
			return Result.Failure<IEnumerable<VotesPerDayResponse>>(PollErrors.PollNotFound);

		var VotesPerDay=await _context.Votes
			.Where(x=> x.PollId == pollId)
			.GroupBy(x => new { voteDate=DateOnly.FromDateTime(x.SubmittedOn)})
			.Select(g => new VotesPerDayResponse(
				g.Key.voteDate,
				g.Count()))
			.ToListAsync(cancellationToken);
		return Result.Success<IEnumerable<VotesPerDayResponse>>(VotesPerDay);
	}
}
