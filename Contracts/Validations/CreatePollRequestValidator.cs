namespace SurveyBasket.Contracts.Validations;

public class CreatePollRequestValidator : AbstractValidator<CreatePollRequest>
{
	public CreatePollRequestValidator()
	{
		RuleFor(x => x.Title).NotEmpty();
	}
}
