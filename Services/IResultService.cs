namespace SurveyBasket.Services;

public interface IResultService
{
	Task<Result<PollVotesResponse>> GetPollVotesResponseAsync(int pollId,CancellationToken cancellationToken=default);
}
