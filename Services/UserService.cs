
using Microsoft.AspNetCore.Identity;
using SurveyBasket.Contracts.Users;
using System.Data;
using System.Linq;

namespace SurveyBasket.Services;

public class UserService(UserManager<ApplicationUser> userManager,
	ApplicationDbContext context) : IUserService
{
	private readonly UserManager<ApplicationUser> _userManager = userManager;
	private readonly ApplicationDbContext _context = context;


	public async Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		return await (from u in _context.Users
					  join ur in _context.UserRoles
					  on u.Id equals ur.UserId
					  join r in _context.Roles
					  on ur.RoleId equals r.Id into roles
					  where !roles.Any(x => x.Name == DefaultRoles.Member)
					  select new
					  {
						  u.Id,
						  u.FirstName,
						  u.LastName,
						  u.Email,
						  u.IsDisabled,
						  Roles = roles.Select(x => x.Name!).ToList()
					  }
				)
				.GroupBy(u => new { u.Id, u.FirstName, u.LastName, u.Email, u.IsDisabled })
				.Select(u => new UserResponse
				(
					u.Key.Id,
					u.Key.FirstName,
					u.Key.LastName,
					u.Key.Email,
					u.Key.IsDisabled,
					u.SelectMany(x => x.Roles)
				))
			   .ToListAsync(cancellationToken);
	}

	public async Task<Result<UserProfileResponse>> GetProfileInfoAsync(string userId)
	{
		var user = await _userManager.Users
		   .Where(x => x.Id == userId)
		   .ProjectToType<UserProfileResponse>()
		   .SingleAsync();

		return Result.Success(user);
	}
	public async Task<Result> UpdateProfileAsync(string userId, UpdateProfileRequest request)
	{
		//var user = await _userManager.FindByIdAsync(userId);
		//user = request.Adapt(user);
		//await _userManager.UpdateAsync(user!);

		var user = await _userManager.Users
			.Where(x => x.Id == userId)
			.ExecuteUpdateAsync(setter =>
				setter.SetProperty(x => x.FirstName, request.FirstName)
					.SetProperty(x => x.LastName, request.LastName)
			);

		return Result.Success(user);
	}
	public async Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request)
	{
		var user = await _userManager.FindByIdAsync(userId);
		var result = await _userManager.ChangePasswordAsync(user!, request.CurrentPassword, request.NewPassword);
		if (result.Succeeded)
			return Result.Success();

		var error = result.Errors.First();
		return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
	}


}
