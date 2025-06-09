using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using SurveyBasket.Authentication;
using SurveyBasket.Helpers;
using System.Security.Cryptography;
using System.Text;

namespace SurveyBasket.Services;

public class AuthService(
	UserManager<ApplicationUser> userManager,
	SignInManager<ApplicationUser> signInManager,
	IJwtProvider jwtProvider,
	ILogger<AuthService> logger,
	IEmailSender emailSender,
	IHttpContextAccessor httpContextAccessor,
	ApplicationDbContext context
	) : IAuthService
{
	private readonly UserManager<ApplicationUser> _userManager = userManager;
	private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
	private readonly IJwtProvider _jwtProvider = jwtProvider;
	private readonly ILogger<AuthService> _logger = logger;
	private readonly IEmailSender _emailSender = emailSender;
	private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
	private readonly ApplicationDbContext _context = context;
	private readonly int _refreshTokenExpirationDays=14;

	public async Task<Result<AuthResponse>> GetTokenAsync(LoginRequest loginRequest, CancellationToken cancellationToken = default)
	{

		if (await _userManager.FindByEmailAsync(loginRequest.Email) is not { } user)
			return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

		if (user.IsDisabled)
			return Result.Failure<AuthResponse>(UserErrors.DisabledUser);

		var result = await _signInManager.PasswordSignInAsync(user, loginRequest.Password, false, true);
		if (result.Succeeded)
		   {
			var (userRoles, userPermissions) = await GetUserRolesPermissions(user, cancellationToken);
			var (token, expiresIn) = _jwtProvider.GenerateJwtToken(user, userRoles, userPermissions);

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

		var error = result.IsNotAllowed
			? UserErrors.EmailNotConfirmed
			: result.IsLockedOut
			? UserErrors.LockedUser
			: UserErrors.InvalidCredentials;

		return Result.Failure<AuthResponse>(error);
	}

	public async Task<Result<AuthResponse>> GetRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest, CancellationToken cancellationToken = default)
	{
		var userId = _jwtProvider.ValidateToken(refreshTokenRequest.token);
		if (userId is null)
			return Result.Failure<AuthResponse>(UserErrors.InvalidJwtTokens);

		var user = await _userManager.FindByIdAsync(userId);
		if (user is null)
			return Result.Failure<AuthResponse>(UserErrors.InvalidJwtTokens);

		if(user.IsDisabled)
			return Result.Failure<AuthResponse>(UserErrors.DisabledUser);

		if (user.LockoutEnd > DateTime.UtcNow)
			return Result.Failure<AuthResponse>(UserErrors.LockedUser);

		var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshTokenRequest.refreshToken && x.IsActive);
		if (userRefreshToken is null)
			return Result.Failure<AuthResponse>(UserErrors.InvalidJwtTokens);

		userRefreshToken.RevokedOn = DateTime.UtcNow;

		var(userRoles, userPermissions) = await GetUserRolesPermissions(user, cancellationToken);
		var (newToken, expiresIn) = _jwtProvider.GenerateJwtToken(user,userRoles, userPermissions);
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
			return Result.Failure(UserErrors.DuplicatedEmail);

		var user = request.Adapt<ApplicationUser>();
		var result = await _userManager.CreateAsync(user, request.Password);
		if (result.Succeeded)
		{
			var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
			code= WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
			_logger.LogInformation("Confirmation code: {code}", code);

			//TODO: Send email with the confirmation link
			await SendConfirmationEmail(user, code);
			return Result.Success();
		}
		else
		{
			var error = result.Errors.First();
			return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
		}
	}
	public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request)
	{
		if(await _userManager.FindByIdAsync(request.UserId) is not { } user)
			return Result.Failure(UserErrors.InvalidCode);

		if(user.EmailConfirmed)
			return Result.Failure(UserErrors.DuplicatedConfirmation);

		var code = request.Code;
		try
		{
			code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
		}
		catch(FormatException)
		{
			return Result.Failure(UserErrors.InvalidCode);
		}
		var result= await _userManager.ConfirmEmailAsync(user, code);

		if (result.Succeeded)
		{ 
			await _userManager.AddToRoleAsync(user, DefaultRoles.Member);
			return Result.Success();
		}
		else
		{
			var error = result.Errors.First();
			return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
		}
		
	}
	public async Task<Result> ResendConfirmEmailAsync(ResendConfirmEmailRequest request)
	{
		if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
			return Result.Success();

		if (!user.EmailConfirmed)
			return Result.Failure(UserErrors.DuplicatedConfirmation);

		var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
		code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
		_logger.LogInformation("Confirmation code: {code}", code);

		//TODO: Send email with the confirmation link
		await SendConfirmationEmail(user,code);
		return Result.Success();

	}

	public async Task<Result> SendResetPasswordCodeAsync(string email)
	{
		if (await _userManager.FindByEmailAsync(email) is not { } user)
			return Result.Success();

		if (!user.EmailConfirmed)
			return Result.Failure(UserErrors.EmailNotConfirmed with { statusCode=StatusCodes.Status400BadRequest});

		var code = await _userManager.GeneratePasswordResetTokenAsync(user);
		code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
		_logger.LogInformation("Reset password code: {code}", code);

		await SendResetPasswordEmail(user, code);
		return Result.Success();

	}

	public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request)
	{
		var user = await _userManager.FindByEmailAsync(request.Email);
		if (user is null || !user.EmailConfirmed)
			return Result.Failure(UserErrors.InvalidCode);
		IdentityResult result;
		try
		{
			var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
			result = await _userManager.ResetPasswordAsync(user, code, request.NewPassword);
		}
		catch (FormatException)
		{
			result = IdentityResult.Failed(_userManager.ErrorDescriber.InvalidToken());
		}
		if (result.Succeeded)
			return Result.Success();
		else
		{
			var error = result.Errors.First();
			return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status401Unauthorized));
		}
	}



	private static string GenerateRefreshToken()
	{
		return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
	}


	private async Task SendConfirmationEmail(ApplicationUser user, string code)
	{
		var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

		var emailBody = EmailBodyBuilder.GenerateEmailBody("EmailConfirmation",
			templateModel: new Dictionary<string, string>
			{
				{ "{{name}}", user.FirstName },
					{ "{{action_url}}", $"{origin}/auth/emailConfirmation?userId={user.Id}&code={code}" }
			}
		);

		BackgroundJob.Enqueue(()=> _emailSender.SendEmailAsync(user.Email!, "✅ Survey Basket: Email Confirmation", emailBody));
		await Task.CompletedTask;
	}

	private async Task SendResetPasswordEmail(ApplicationUser user, string code)
	{
		var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

		var emailBody = EmailBodyBuilder.GenerateEmailBody("ForgetPassword",
			templateModel: new Dictionary<string, string>
			{
				{ "{{name}}", user.FirstName },
					{ "{{action_url}}", $"{origin}/auth/forgetPassword?email={user.Email}&code={code}" }
			}
		);

		BackgroundJob.Enqueue(()=> _emailSender.SendEmailAsync(user.Email!, "✅ Survey Basket: Change Password", emailBody));
		await Task.CompletedTask;
	}

	private async Task<(IEnumerable<string>roles,IEnumerable<string>permissions)>GetUserRolesPermissions(ApplicationUser user,CancellationToken cancellationToken)
	{
		var userRoles=await _userManager.GetRolesAsync(user);

		var userPermissions=await (
			from r in _context.Roles
			join p in _context.RoleClaims
			on r.Id equals p.RoleId
			where userRoles.Contains(r.Name!)
			select p.ClaimValue!)
			.Distinct()
			.ToListAsync(cancellationToken);

		return (userRoles,userPermissions);
	}
}
