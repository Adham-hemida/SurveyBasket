using Microsoft.AspNetCore.Identity;
using SurveyBasket.Authentication;
using SurveyBasket.Errors;
using System.Security.Cryptography;

namespace SurveyBasket.Services;

public class AuthService(UserManager<ApplicationUser> userManager,IJwtProvider jwtProvider) : IAuthService
{
	private readonly UserManager<ApplicationUser> _userManager = userManager;
	private readonly IJwtProvider _jwtProvider = jwtProvider;
	private readonly int _refreshTokenExpirationDays=14;	

	public async Task<Result<AuthResponse>> GetTokenAsync(LoginRequest loginRequest, CancellationToken cancellationToken = default)
	{
		//Check user if exists
		var user = await _userManager.FindByEmailAsync(loginRequest.Email);
		if(user is null)
			return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
		//check password if it correct
		var isValidPassword = await _userManager.CheckPasswordAsync(user, loginRequest.Password);
		if(!isValidPassword)
			return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
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
		var response = new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, token, expiresIn, refreshToken, refreshTokenExpiration);
		return  Result.Success(response);
	}
	

	public async Task<Result<AuthResponse>> GetRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest, CancellationToken cancellationToken = default)
	{
		//I need to validate the token and refreshToken
		var userId = _jwtProvider.ValidateToken(refreshTokenRequest.token);
		if (userId is null)
			return Result.Failure<AuthResponse>(UserErrors.InvalidJwtTokens);
		//if i reach here, the token is valid and userId is not null and i got it
		//get the user from the database
		var user = await _userManager.FindByIdAsync(userId);
		if (user is null)
			return Result.Failure<AuthResponse>(UserErrors.InvalidJwtTokens);
		//check if the refreshToken is valid(that mean The refreshToken i send is true) or not and get it from the database and check if it is active or not 
		var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshTokenRequest.refreshToken && x.IsActive);
		if (userRefreshToken is null)
			return Result.Failure<AuthResponse>(UserErrors.InvalidJwtTokens);
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
			var result=new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, newToken, expiresIn,newRefreshToken, newRefreshTokenExpiration);
		return Result.Success(result);

	}
	public async Task<Result> RevokeRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest, CancellationToken cancellationToken = default)
	{
		var userId = _jwtProvider.ValidateToken(refreshTokenRequest.token);
		if(userId is null)
			return Result.Failure(UserErrors.InvalidRefreshToken);
		var user=await _userManager.FindByIdAsync(userId);
		if(user is null)
			return Result.Failure(UserErrors.InvalidRefreshToken);
		var userRefreshToken =user.RefreshTokens.SingleOrDefault(x => x.Token == refreshTokenRequest.refreshToken && x.IsActive);	
		if(userRefreshToken is null)
			return Result.Failure(UserErrors.InvalidRefreshToken);
		userRefreshToken.RevokedOn = DateTime.UtcNow;
		await _userManager.UpdateAsync(user);
		return Result.Success();
	}
	private static string GenerateRefreshToken()
	{
		return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
	}

	
}
