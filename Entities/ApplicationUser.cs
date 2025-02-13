

using Microsoft.AspNetCore.Identity;

namespace SurveyBasket.Entities;

public sealed class ApplicationUser : IdentityUser
{
	public string FirstName { get; set; } = string.Empty;
	public string LastName { get; set; } = string.Empty;
}
