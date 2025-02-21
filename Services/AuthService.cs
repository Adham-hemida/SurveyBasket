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
		//generate refreshToken
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


	public async Task<AuthResponse?> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
	{
		//I need to validate the token and refreshToken
		var userId = _jwtProvider.ValidateToken(token);
		if (userId is null)
			return null;
		//if i reach here, the token is valid and userId is not null and i got it
		//get the user from the database
		var user = await _userManager.FindByIdAsync(userId);
		if (user is null)
			return null;
		//check if the refreshToken is valid(that mean The refreshToken i send is true) or not and get it from the database and check if it is active or not 
		var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);
		if (userRefreshToken is null)
			return null;
		//if i reach here, the refreshToken is valid and i got it from the database 
		//I revoked it as i need user to use it one time and need to generate new one
		userRefreshToken.RevokedOn = DateTime.UtcNow;

		// i need to generate new token and refreshToken
		//generate new  token
		var (newToken, expiresIn) = _jwtProvider.GenerateJwtToken(user);
		//generate new refreshToken
		var newRefreshToken = GenerateRefreshToken();
		var newRefreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpirationDays);

		//save refresh token in Database
		user.RefreshTokens.Add(new RefreshToken
		{
			Token = newRefreshToken,
			ExpiresOn = newRefreshTokenExpiration
		});
		await _userManager.UpdateAsync(user);
		return new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, newToken, expiresIn,newRefreshToken, newRefreshTokenExpiration);

	}
	private static string GenerateRefreshToken()
	{
		return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
	}
}
