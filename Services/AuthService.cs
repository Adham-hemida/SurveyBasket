using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using SurveyBasket.Authentication;
using System.Security.Cryptography;
using System.Text;

namespace SurveyBasket.Services;

public class AuthService(
	UserManager<ApplicationUser> userManager,
	SignInManager<ApplicationUser> signInManager,
	IJwtProvider jwtProvider,
	ILogger<AuthService> logger
	) : IAuthService
{
	private readonly UserManager<ApplicationUser> _userManager = userManager;
	private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
	private readonly IJwtProvider _jwtProvider = jwtProvider;
	private readonly ILogger<AuthService> _logger = logger;
	private readonly int _refreshTokenExpirationDays=14;

	public async Task<Result<AuthResponse>> GetTokenAsync(LoginRequest loginRequest, CancellationToken cancellationToken = default)
	{

		if (await _userManager.FindByEmailAsync(loginRequest.Email) is not { } user)
			return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

		var result = await _signInManager.PasswordSignInAsync(user, loginRequest.Password, false, false);
		if (result.Succeeded)
		   {
			var (token, expiresIn) = _jwtProvider.GenerateJwtToken(user);

			var refreshToken = GenerateRefreshToken();
			var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpirationDays);

			user.RefreshTokens.Add(new RefreshToken
			{
				Token = refreshToken,
				ExpiresOn = refreshTokenExpiration
			});

			await _userManager.UpdateAsync(user);
			var response = new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, token, expiresIn, refreshToken, refreshTokenExpiration);
			return Result.Success(response);
		  }
		return Result.Failure<AuthResponse>(result.IsNotAllowed ?UserErrors.EmailNotConfirmed:UserErrors.InvalidCredentials);
	}

	public async Task<Result<AuthResponse>> GetRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest, CancellationToken cancellationToken = default)
	{
		var userId = _jwtProvider.ValidateToken(refreshTokenRequest.token);
		if (userId is null)
			return Result.Failure<AuthResponse>(UserErrors.InvalidJwtTokens);

		var user = await _userManager.FindByIdAsync(userId);
		if (user is null)
			return Result.Failure<AuthResponse>(UserErrors.InvalidJwtTokens);

		var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshTokenRequest.refreshToken && x.IsActive);
		if (userRefreshToken is null)
			return Result.Failure<AuthResponse>(UserErrors.InvalidJwtTokens);

		userRefreshToken.RevokedOn = DateTime.UtcNow;

		var (newToken, expiresIn) = _jwtProvider.GenerateJwtToken(user);
		var newRefreshToken = GenerateRefreshToken();
		var newRefreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpirationDays);

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
	public async Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
	{
		var emailExists = await _userManager.Users.AnyAsync(x => x.Email == request.Email, cancellationToken);
		if (emailExists)
			return Result.Failure<AuthResponse>(UserErrors.DuplicatedEmail);

		var user = request.Adapt<ApplicationUser>();
		var result = await _userManager.CreateAsync(user, request.Password);
		if (result.Succeeded)
		{
			var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
			code= WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
			_logger.LogInformation("Confirmation code: {code}", code);

			//TODO: Send email with the confirmation link
			return Result.Success();
		}
		else
		{
			var error = result.Errors.First();
			return Result.Failure<AuthResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
		}
	}

	private static string GenerateRefreshToken()
	{
		return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
	}

	
}
