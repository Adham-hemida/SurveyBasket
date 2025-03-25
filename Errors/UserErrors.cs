namespace SurveyBasket.Errors;

public  static class UserErrors
{
	public static readonly  Error InvalidCredentials = 
		new("invalid.credentials", "Invalid Email/Password", StatusCodes.Status401Unauthorized);

	public static readonly  Error InvalidJwtTokens =  
		new ("User.InvalidJwtToken", "Invalid Jwt token", StatusCodes.Status401Unauthorized);

	 public static readonly Error InvalidRefreshToken =
		new("User.InvalidRefreshToken", "Invalid refresh token", StatusCodes.Status401Unauthorized);

	public static readonly Error DuplicatedEmail =
		new("User.DuplicatedEmail", "Another user with the same email is already exists", StatusCodes.Status409Conflict);

}
