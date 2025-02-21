namespace SurveyBasket.Contracts.Authentication;

public record RefreshTokenRequest(
	string token,
	string refreshToken
	);
