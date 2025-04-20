namespace SurveyBasket.Contracts.Users;

public record ChangePasswordRequest(
	string CurrentPassword,
	string NewPassword
	);

