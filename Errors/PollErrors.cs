namespace SurveyBasket.Errors;

public static class PollErrors
{
	public static readonly Error PollNotFound =
		new("poll.not_found", "No Poll was found with the given Id", StatusCodes.Status404NotFound);

	public static readonly Error DuplicatedPollTitle =
		new("poll.DuplicatedTitle", "Another Poll with the same title is already exist", StatusCodes.Status409Conflict);
}
