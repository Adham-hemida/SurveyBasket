namespace SurveyBasket.Authentication;

public interface IJwtProvider
{
	(string token, int expiresIn) GenerateJwtToken(ApplicationUser user, IEnumerable<string> roles, IEnumerable<string> permissions);
	string? ValidateToken(string token);	
}
