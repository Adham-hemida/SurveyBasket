namespace SurveyBasket.Authentication;

public class JwtOptions
{
	public static string sectionName = "Jwt";
	public string Key { get; set; } = string.Empty;
	public string Issuer { get; set; } = string.Empty;
	public string Audience { get; set; } = string.Empty;
	public int ExpiresInMinutes  { get; set; }
}
