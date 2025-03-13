namespace SurveyBasket.Contracts.Results;

public record VotesPerDayResponse(
	DateOnly Date,
	int NumberOfVotes
	);

