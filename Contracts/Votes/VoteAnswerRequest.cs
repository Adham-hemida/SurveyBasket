namespace SurveyBasket.Contracts.Votes;

public record VoteAnswerRequest(
	int QuestionId,
	int AnswerId
	);

