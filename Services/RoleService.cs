using SurveyBasket.Contracts.Roles;

namespace SurveyBasket.Services;

public class RoleService(RoleManager<ApplicationRole> roleManager):IRoleService
{
	private readonly RoleManager<ApplicationRole> _roleManager = roleManager;

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
}
