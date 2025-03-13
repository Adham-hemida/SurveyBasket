namespace SurveyBasket.Services;

public class ResultService(ApplicationDbContext context) : IResultService
{
	private readonly ApplicationDbContext _context = context;

	public async Task<Result<PollVotesResponse>> GetPollVotesResponseAsync(int pollId, CancellationToken cancellationToken = default)
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
}
