namespace SurveyBasket.Contracts.Results;

public record VotesPerAnswerResponse(
	string Answer,
	int Count
	);