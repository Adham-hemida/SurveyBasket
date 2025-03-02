namespace SurveyBasket.Errors;

public  static class UserErrors
{
	public static readonly  Error InvalidCredentials = 
		new("invalid.credentials", "Invalid Email/Password");

	public static readonly  Error InvalidJwtTokens =  
		new ("User.InvalidJwtToken", "Invalid Jwt token");

	 public static readonly Error InvalidRefreshToken =
		new("User.InvalidRefreshToken", "Invalid refresh token");
}
