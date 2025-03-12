namespace SurveyBasket.Contracts.Votes;

public class VoteRequestValidator:AbstractValidator<VoteRequest>
{
	public VoteRequestValidator()
	{
		RuleFor(x => x.Answers)
			.NotEmpty();
	}
}
