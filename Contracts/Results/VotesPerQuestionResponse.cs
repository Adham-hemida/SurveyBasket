namespace SurveyBasket.Contracts.Results;

public record VotesPerQuestionResponse(
	string Question,
	IEnumerable<VotesPerAnswerResponse> SelectedAnswer
	);

