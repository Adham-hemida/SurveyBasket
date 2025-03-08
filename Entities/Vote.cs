namespace SurveyBasket.Entities;

public sealed class Vote
{
	public int Id { get; set; }
	public DateTime SubmittedOn { get; set; } = DateTime.UtcNow;
	public int PollId { get; set; }
	public Poll Poll { get; set; } = default!;
	public string UserId { get; set; } = string.Empty;
	public ApplicationUser User { get; set; } = default!;

	public ICollection<VoteAnswer> VoteAnswers { get; set; } = [];
}
