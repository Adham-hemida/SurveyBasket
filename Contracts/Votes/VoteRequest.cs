namespace SurveyBasket.Contracts.Votes;

public record VoteRequest(
	IEnumerable<VoteAnswerRequest> Answers
	);
