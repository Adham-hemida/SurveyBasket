namespace SurveyBasket.Contracts.Polls;

public class PollRequestValidator : AbstractValidator<PollRequest>
{
	public PollRequestValidator()
	{

		RuleFor(x => x.Title)
		   .NotEmpty()
		   .WithMessage("{PropertyName} is required")
		   .Length(3, 100)
		   .WithMessage("{PropertyName} must be within {MinLength} and {MaxLength} : the total you entered {TotalLength}");


		RuleFor(x => x.Summary)
		   .NotEmpty()
		   .WithMessage("{PropertyName} is required")
		   .Length(3, 1500);

		RuleFor(x => x.StartsAt)
			.NotEmpty()
			.GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
			.WithMessage("{PropertyName} must be greater than Or Equal today");

		RuleFor(x => x.EndsAt)
			.NotEmpty();

		RuleFor(x => x)//valide for all Model as i need to check the relation between StartsAt and EndsAt (need to models from it startsAt and EndsAt)
			.Must(BeAValidDate)
			.WithName(nameof(PollRequest.EndsAt))
			.WithMessage("{PropertyName} must be greater or Equal than StartsAt");
	}
	private bool BeAValidDate(PollRequest pollRequest)//compare two values of models from it startsAt and EndsAt so i need the all model
	{
		return pollRequest.EndsAt >= pollRequest.StartsAt;
	}

}
