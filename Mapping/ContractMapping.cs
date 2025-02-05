

namespace SurveyBasket.Mapping;

public static class ContractMapping
{
	public static PollResponse MapToResponse(this Poll poll)
	{
		return new()
		{
			Id = poll.Id,
			Title = poll.Title,
			Description = poll.Description
		};
	}
	public static Poll mapToPoll(this CreatePollRequest request)
	{
		return new()
		{
			Title = request.Title,
			Description = request.Description
		};
	}

}
