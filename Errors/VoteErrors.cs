namespace SurveyBasket.Errors;

public static class VoteErrors
{
	

	public static readonly Error DuplicatedVote = 
		new("Vote.DuplicatedVote", "This user is already voted before for this poll"); 
}
