using Microsoft.AspNetCore.Identity;
using SurveyBasket.Authentication;
using System.Security.Cryptography;

namespace SurveyBasket.Services;

public class AuthService(UserManager<ApplicationUser> userManager,IJwtProvider jwtProvider) : IAuthService
{
	private readonly UserManager<ApplicationUser> _userManager = userManager;
	private readonly IJwtProvider _jwtProvider = jwtProvider;
	private readonly int _refreshTokenExpirationDays=14;	

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
		var refreshToken = GenerateRefreshToken();
		var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpirationDays);

		//save refresh token in Database
		user.RefreshTokens.Add(new RefreshToken
		{
			Token = refreshToken,
			ExpiresOn = refreshTokenExpiration
		});
		await _userManager.UpdateAsync(user);
		return new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName,token,expiresIn,refreshToken,refreshTokenExpiration);


	}
	private static string GenerateRefreshToken()
	{
		return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));	
	}
}
