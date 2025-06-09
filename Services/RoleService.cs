using Microsoft.Identity.Client;
using SurveyBasket.Contracts.Roles;

namespace SurveyBasket.Services;

public class RoleService(
	RoleManager<ApplicationRole> roleManager,
	ApplicationDbContext context
	):IRoleService
{
	private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
	private readonly ApplicationDbContext _context = context;

	public async Task<IEnumerable<RoleResponse>> GetAllAsync(bool includeDisabled=false,CancellationToken cancellationToken=default)
	{
		return await _roleManager.Roles
			.Where(x=>!x.IsDefault&&( !x.IsDeleted || includeDisabled))
			.Select(x => new RoleResponse
			(x.Id,
			x.Name!,
			x.IsDeleted))
			.ToListAsync(cancellationToken);
	}
	public async Task<Result<RoleDetailResponse>> GetAsync(string id)
	{
		if (await _roleManager.FindByIdAsync(id) is not { } role)
			return Result.Failure<RoleDetailResponse>(RolesError.RoleNotFound);

		var permissions=await _roleManager.GetClaimsAsync(role);

		var response=new RoleDetailResponse(role.Id,role.Name!,role.IsDeleted,permissions.Select(x=>x.Value));

		return Result.Success(response);
	}
	public async Task<Result<RoleDetailResponse>> AddAsync(RoleRequest request)
	{
		var roleIsExists = await _roleManager.RoleExistsAsync(request.Name);
		if (roleIsExists)
			return Result.Failure<RoleDetailResponse>(RolesError.RoleDuplicated);

		var allowedPermissions=Permissions.GetAllPermissions();

		if(request.Permissions.Except(allowedPermissions).Any())
			return Result.Failure<RoleDetailResponse>(RolesError.InvalidPermissions);
		var role = new ApplicationRole
		{
			Name = request.Name,
			ConcurrencyStamp = Guid.NewGuid().ToString()
		};

		var result=await _roleManager.CreateAsync(role);
		if(result.Succeeded)
		{
			var permissions = request.Permissions.Select
				(
				x => new IdentityRoleClaim<string>
				{
					ClaimType = Permissions.Type,
					ClaimValue = x,
					RoleId = role.Id
				});
			await _context.AddRangeAsync(permissions);
			await _context.SaveChangesAsync();
			var response = new RoleDetailResponse(role.Id, role.Name, role.IsDeleted, request.Permissions);
			return Result.Success(response);
		}
		else
		{
			var errors = result.Errors.First();
			return Result.Failure<RoleDetailResponse>(new Error(errors.Code,errors.Description,StatusCodes.Status400BadRequest));
		}
	}

	public async Task<Result> UpdateAsync(string id, RoleRequest request)
	{
		var roleIsExists = await _roleManager.Roles.AnyAsync(x=>x.Name==request.Name && x.Id != id);
		if (roleIsExists)
			return Result.Failure(RolesError.RoleDuplicated);

		if (await _roleManager.FindByIdAsync(id) is not { } role)
			return Result.Failure(RolesError.RoleNotFound);

		var allowedPermissions = Permissions.GetAllPermissions();

		if (request.Permissions.Except(allowedPermissions).Any())
			return Result.Failure<RoleDetailResponse>(RolesError.InvalidPermissions);

		role.Name = request.Name;
		var result = await _roleManager.UpdateAsync(role);

		if (result.Succeeded)
		{
			var currentPermissions = await _context.RoleClaims
				.Where(x=>x.RoleId == role.Id&& x.ClaimType == Permissions.Type)
				.Select(x=>x.ClaimValue)
				.ToListAsync();

			var newPermissions=request.Permissions
				.Except(currentPermissions)
				.Select(x => new IdentityRoleClaim<string>
				{
					ClaimType = Permissions.Type,
					ClaimValue = x,
					RoleId = role.Id
				});

			var removedPermissions = currentPermissions.Except(request.Permissions);

			await _context.RoleClaims
				.Where(x => x.RoleId == role.Id && removedPermissions.Contains(x.ClaimValue))
				.ExecuteDeleteAsync();

			await _context.AddRangeAsync(newPermissions);
			await _context.SaveChangesAsync();
		
			return Result.Success();

		}
		else
		{
			var errors = result.Errors.First();
			return Result.Failure<RoleDetailResponse>(new Error(errors.Code, errors.Description, StatusCodes.Status400BadRequest));
		}

	}

	public async Task<Result> ToggleSatausAsync(string id)
	{
		var role = await _roleManager.FindByIdAsync(id);
		if (role is null)
			return Result.Failure(RolesError.RoleNotFound);
		role.IsDeleted = !role.IsDeleted;
		await _roleManager.UpdateAsync(role);
		return Result.Success();
	}
}
