using SurveyBasket.Abstractions.Const;

namespace SurveyBasket.Contracts.Authentication;

public class RegisterRequestValidator:AbstractValidator<RegisterRequest>
{
	public RegisterRequestValidator()
	{
		RuleFor(x => x.Email)
			.EmailAddress()
			.NotEmpty();

		RuleFor(x => x.Password)
			.Matches(RegexPatterns.Password)
			.NotEmpty()
            .WithMessage("Password should be at least 8 digits and should contains Lowercase, NonAlphanumeric and Uppercase");
		
		RuleFor(x => x.FirstName)
			.NotEmpty()
			.Length(3,100);

		RuleFor(x => x.LastName)
		  .NotEmpty()
		  .Length(3, 100);
	}
}
