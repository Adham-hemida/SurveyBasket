namespace SurveyBasket.Models;

public class Poll
{
	public int Id { get; set; }
	public string Title { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;


public static explicit operator PollResponse(Poll poll)
	{
		return new()
		{
			Id = poll.Id,
			Title = poll.Title,
			Description = poll.Description
		};
	} }