namespace SurveyBasket.Errors;

public static class QuestionErrors
{
	public static readonly Error QuestionNotFound =
		new("Question.not_found", "No Question was found with the given Id", StatusCodes.Status404NotFound);

	public static readonly Error DuplicatedQuestionContent =
		new("Question.DuplicatedContent", " Another Question with same content is already exist", StatusCodes.Status409Conflict);
}
