namespace SurveyBasket.Authentication;

public interface IJwtProvider
{
	(string token, int expiresIn) GenerateJwtToken(ApplicationUser user);
	string? ValidateToken(string token);	
}
