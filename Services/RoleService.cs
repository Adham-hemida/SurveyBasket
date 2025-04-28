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
}
