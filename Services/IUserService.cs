using SurveyBasket.Contracts.Users;

namespace SurveyBasket.Services;

public interface IUserService
{
	Task<Result<UserProfileResponse>> GetProfileInfoAsync(string userId);
	Task<Result> UpdateProfileAsync(string userId, UpdateProfileRequest request);
}
