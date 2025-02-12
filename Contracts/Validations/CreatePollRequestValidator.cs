namespace SurveyBasket.Contracts.Validations;

public class CreatePollRequestValidator : AbstractValidator<CreatePollRequest>
{
	public CreatePollRequestValidator()
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

		 
	}

}
