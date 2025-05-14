using SurveyBasket.Contracts.Common;
using SurveyBasket.Contracts.Users;
using System.Linq.Dynamic.Core;
using System.Data;

namespace SurveyBasket.Services;

public class UserService(UserManager<ApplicationUser> userManager,
	IRoleService roleService,
	ApplicationDbContext context) : IUserService
{
	private readonly UserManager<ApplicationUser> _userManager = userManager;
	private readonly IRoleService _roleService = roleService;
	private readonly ApplicationDbContext _context = context;


	public async Task<PaginatedList<UserResponse>> GetAllAsync(RequestFilters filters, CancellationToken cancellationToken = default)
	{
		var query = from u in _context.Users
					join ur in _context.UserRoles on u.Id equals ur.UserId
					join r in _context.Roles on ur.RoleId equals r.Id into roles
					where !roles.Any(x => x.Name == DefaultRoles.Member)
					select new
					{
						u.Id,
						u.FirstName,
						u.LastName,
						u.Email,
						u.IsDisabled,
						Roles = roles.Select(x => x.Name).ToList()
					};

		// Apply filtering based on the search value
		if (!string.IsNullOrEmpty(filters.SearchValue))
		{
			query = query.Where(u => u.FirstName.Contains(filters.SearchValue) || u.LastName.Contains(filters.SearchValue) || u.Email.Contains(filters.SearchValue));
		}
		if (!string.IsNullOrEmpty(filters.SortColumn))
		{
			query = query.OrderBy($"{filters.SortColumn} {filters.SortDirection}");
		}
		// تحويل الاستعلام إلى AsEnumerable لتطبيق العمليات في الذاكرة
		var resultList = query.AsEnumerable()
							  .GroupBy(u => new
							  {
								  u.Id,
								  u.FirstName,
								  u.LastName,
								  u.Email,
								  u.IsDisabled
							  })
							  .Select(u => new UserResponse
							  (
								  u.Key.Id,
								  u.Key.FirstName,
								  u.Key.LastName,
								  u.Key.Email,
								  u.Key.IsDisabled,
								  u.SelectMany(x => x.Roles).ToList()
							  ))
							  .ToList();
		return await PaginatedList<UserResponse>.Create1(
			resultList,
			filters.PageNumber,
			filters.PageSize,cancellationToken
			);
		//	return await (from u in _context.Users
		//				  join ur in _context.UserRoles
		//				  on u.Id equals ur.UserId
		//				  join r in _context.Roles
		//				  on ur.RoleId equals r.Id into roles
		//				  where !roles.Any(x => x.Name == DefaultRoles.Member)
		//				  select new
		//				  {
		//					  u.Id,
		//					  u.FirstName,
		//					  u.LastName,
		//					  u.Email,
		//					  u.IsDisabled,
		//					  Roles = roles.Select(x => x.Name!).ToList()
		//				  }
		//			)
		//			.GroupBy(u => new { u.Id, u.FirstName, u.LastName, u.Email, u.IsDisabled })
		//			.Select(u => new UserResponse
		//			(
		//				u.Key.Id,
		//				u.Key.FirstName,
		//				u.Key.LastName,
		//				u.Key.Email,
		//				u.Key.IsDisabled,
		//				u.SelectMany(x => x.Roles)
		//			))
		//		   .ToListAsync(cancellationToken);
	
	}

	public async Task<Result<UserResponse>> GetAsync(string id)
	{
		if (await _userManager.FindByIdAsync(id) is not { } user)
			return Result.Failure<UserResponse>(UserErrors.UserNotFound);

		var userRoles = await _userManager.GetRolesAsync(user);
		//	var response = new UserResponse(user.Id, user.FirstName, user.LastName, user.Email, user.IsDisabled, roles);
		var response = (user, userRoles).Adapt<UserResponse>();
		return Result.Success(response);
	}

	public async Task<Result<UserResponse>> AddAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
	{
		var emailIsExist = await _userManager.Users.AnyAsync(x => x.Email == request.Email, cancellationToken);
		if (emailIsExist)
			return Result.Failure<UserResponse>(UserErrors.DuplicatedEmail);

		var allowRoles = await _roleService.GetAllAsync(cancellationToken: cancellationToken);

		if (request.Roles.Except(allowRoles.Select(x => x.Name)).Any())
			return Result.Failure<UserResponse>(UserErrors.InvalidRoles);

		var user = request.Adapt<ApplicationUser>();
		var result = await _userManager.CreateAsync(user, request.Password);
		if (result.Succeeded)
		{
			await _userManager.AddToRolesAsync(user, request.Roles);
			var response = (user, request.Roles).Adapt<UserResponse>();
			return Result.Success(response);
		}
		else
		{
			var error = result.Errors.First();
			return Result.Failure<UserResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
		}
	}


	public async Task<Result> UpdateAsync(string id, UpdateUserRequest request, CancellationToken cancellationToken = default)
	{
		var emailIsExist = await _userManager.Users.AnyAsync(x => x.Email == request.Email && x.Id != id, cancellationToken);
		if (emailIsExist)
			return Result.Failure(UserErrors.DuplicatedEmail);

		var allowRoles = await _roleService.GetAllAsync(cancellationToken: cancellationToken);

		if (request.Roles.Except(allowRoles.Select(x => x.Name)).Any())
			return Result.Failure(UserErrors.InvalidRoles);

		if (await _userManager.FindByIdAsync(id) is not { } user)
			return Result.Failure(UserErrors.UserNotFound);

		user = request.Adapt(user);
		var result = await _userManager.UpdateAsync(user);
		if (result.Succeeded)
		{
			await _context.UserRoles
				.Where(x => x.UserId == id)
				.ExecuteDeleteAsync(cancellationToken);

			await _userManager.AddToRolesAsync(user, request.Roles);
			return Result.Success();
		}
		else
		{
			var error = result.Errors.First();
			return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
		}
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

	public async Task<Result> ToggleSatausAsync(string id)
	{
		var user = await _userManager.FindByIdAsync(id);
		if (user is null)
			return Result.Failure(UserErrors.UserNotFound);

		user.IsDisabled = !user.IsDisabled;

		await _userManager.UpdateAsync(user);

		return Result.Success();
	}


	public async Task<Result> UnlockAsync(string id)
	{
		var user = await _userManager.FindByIdAsync(id);
		if (user is null)
			return Result.Failure(UserErrors.UserNotFound);

		var result = await _userManager.SetLockoutEndDateAsync(user, null);

		if (result.Succeeded)
			return Result.Success();

		var error = result.Errors.First();
		return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
	}

}
