namespace SurveyBasket.Errors;

public class PollErrors
{
	public static readonly Error PollNotFound = new("poll.not_found", "No Poll was found with the given Id"); 
}
