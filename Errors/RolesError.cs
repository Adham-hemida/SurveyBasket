namespace SurveyBasket.Errors;

public  static class RolesError
{
	public static readonly  Error RoleNotFound =
		new("role.Notfound", "Role not found", StatusCodes.Status404NotFound);

	public static readonly Error RoleAlreadyExists =
		new("role.alreadyexists", "Role already exists", StatusCodes.Status409Conflict);
}
