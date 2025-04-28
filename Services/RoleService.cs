using SurveyBasket.Contracts.Roles;

namespace SurveyBasket.Services;

public class RoleService(
	RoleManager<ApplicationRole> roleManager,
	ApplicationDbContext context
	):IRoleService
{
	private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
	private readonly ApplicationDbContext _context = context;

	public async Task<IEnumerable<RoleResponse>> GetAllAsync(bool? includeDisabled=false,CancellationToken cancellationToken=default)
	{
		return await _roleManager.Roles
			.Where(x=>!x.IsDefault&&( !x.IsDeleted || includeDisabled == true))
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
}
