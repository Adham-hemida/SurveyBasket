namespace SurveyBasket.Contracts.Questions;

public record QuestionRequest(
	string Content,
	List<string> Answers
	);