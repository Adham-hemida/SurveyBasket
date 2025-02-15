using Microsoft.AspNetCore.Identity;
using SurveyBasket.Authentication;

namespace SurveyBasket.Services;

public class AuthService(UserManager<ApplicationUser> userManager,IJwtProvider jwtProvider) : IAuthService
{
	private readonly UserManager<ApplicationUser> _userManager = userManager;
	private readonly IJwtProvider _jwtProvider = jwtProvider;

	public async Task<AuthResponse?> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
	{
		//Check user if exists
		var user = await _userManager.FindByEmailAsync(email);
		if(user is null)
			return null;
		//check password if it correct
		var isValidPassword = await _userManager.CheckPasswordAsync(user, password);
		if(!isValidPassword)
			return null;
		//generate token
		var (token, expiresIn) = _jwtProvider.GenerateJwtToken(user);

		return new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName,token,expiresIn);


	}
}
