using SurveyBasket.Contracts.Votes;

namespace SurveyBasket.Services;

public interface IVoteService
{
	Task<Result> AddAsync(int pollId,string userId, VoteRequest request,CancellationToken cancellationToken);
}
