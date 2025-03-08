namespace SurveyBasket.Entities;

public sealed class VoteAnswer
{
	public int Id { get; set; }
	public int AnswerId { get; set; }
	public int VoteId { get; set; }
	public int QuestionId { get; set; }

	public Answer Answer { get; set; } = default!;
	public Vote Vote { get; set; } = default!;
	public Question Question { get; set; } = default!;

}
