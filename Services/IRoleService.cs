using SurveyBasket.Contracts.Roles;

namespace SurveyBasket.Services;

public interface IRoleService
{
	Task<IEnumerable<RoleResponse>> GetAllAsync(bool? includeDisabled = false, CancellationToken cancellationToken = default);
	Task<Result<RoleDetailResponse>> GetAsync(string id);
}
