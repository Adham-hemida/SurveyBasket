using FluentValidation.AspNetCore;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SurveyBasket.Authentication;
using System.Text;

namespace SurveyBasket;

public static class DependencyInjection
{
	public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
	{
		var allowOrigins=configuration.GetSection("AllowOrigins").Get<string[]>();

		services.AddCors(options =>
		options.AddDefaultPolicy(bulider =>
		{
			bulider.AllowAnyMethod()
			.AllowAnyHeader()
			.WithOrigins(allowOrigins!);
		}));

		var connectionString = configuration.GetConnectionString("DefaultConnection") ??
			throw new InvalidOperationException("connection string 'Default connection not found'");
		services.AddDbContext<ApplicationDbContext>(
			options => options.UseSqlServer(connectionString)
		  );

		services.AddControllers();
		services.AddAuthConfig(configuration);
		services.
			AddMapsterConfig()
			.AddFluentValidationConfig();

		services.AddScoped<IPollService, PollService>();
		services.AddScoped<IAuthService, AuthService>();

		services.AddOpenApi();

		return services;
	}
	private static IServiceCollection AddMapsterConfig(this IServiceCollection services)
	{

		//Add Mapster
		var mappingConfig = TypeAdapterConfig.GlobalSettings;
		mappingConfig.Scan(Assembly.GetExecutingAssembly());
		services.AddSingleton<IMapper>(new Mapper(mappingConfig));


		return services;
	}
	private static IServiceCollection AddFluentValidationConfig(this IServiceCollection services)
	{
		services
			.AddFluentValidationAutoValidation()
			.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());


		return services;
	}
	private static IServiceCollection AddAuthConfig(this IServiceCollection services, IConfiguration configuration)
	{
		services
			.AddIdentity<ApplicationUser, IdentityRole>()
			.AddEntityFrameworkStores<ApplicationDbContext>();

		services.AddSingleton<IJwtProvider, JwtProvider>();

	   // services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.sectionName));
	   //add it because i Had Validation need to use 
	   services.AddOptions<JwtOptions>()
			.Bind(configuration.GetSection(JwtOptions.sectionName))
			.ValidateDataAnnotations()
			.ValidateOnStart();
		// to use it to get the name of attributes in JwtOptions class
		var JwtSettings = configuration.GetSection(JwtOptions.sectionName).Get<JwtOptions>();

		services.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

		})

			.AddJwtBearer(o =>
			{
				o.SaveToken = true;
				o.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidIssuer = JwtSettings?.Issuer,
					ValidAudience = JwtSettings?.Audience,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSettings?.Key!))
				};
			});


		return services;
	}
}