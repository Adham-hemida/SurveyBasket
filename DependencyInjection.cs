using FluentValidation.AspNetCore;
using Hangfire;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.IdentityModel.Tokens;
using SurveyBasket.Authentication;
using SurveyBasket.Settings;
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

		services.AddHybridCache();

		services.AddControllers();
		services.AddAuthConfig(configuration);
		services.
			AddMapsterConfig()
			.AddFluentValidationConfig();

		services.AddScoped<IPollService, PollService>();
		services.AddScoped<IAuthService, AuthService>();
		services.AddScoped<IUserService, UserService>();
		services.AddScoped<IRoleService, RoleService>();
		services.AddScoped<IEmailSender, EmailService>();
		services.AddScoped<IQuestionService,Questionservice>();
		services.AddScoped<IVoteService,VoteService>();
		services.AddScoped<IResultService, ResultService>();

		services.AddExceptionHandler<GlobalExceptionHandler>();
		services.AddProblemDetails();

		services.AddBackgroundJobsConfig(configuration);

		services.AddHealthChecks()
			.AddSqlServer(name: "database", connectionString: configuration.GetConnectionString("DefaultConnection")!);

		services.AddOpenApi();
		
		services.AddHttpContextAccessor();
		services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));

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
			.AddIdentity<ApplicationUser, ApplicationRole>()
			.AddEntityFrameworkStores<ApplicationDbContext>()
			.AddDefaultTokenProviders();

		services.AddTransient<IAuthorizationHandler,PermissionAuthorizationHandler>();
		services.AddTransient<IAuthorizationPolicyProvider,PermissionAuthorizationPolicyProvider>();

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

		services.Configure<IdentityOptions>(options =>
		{
			options.Password.RequiredLength = 8;
			options.User.RequireUniqueEmail = true;
			options.SignIn.RequireConfirmedEmail = true;
		}

			);

		return services;
	}	
	private static IServiceCollection AddBackgroundJobsConfig(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddHangfire(config => config
	       .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
	       .UseSimpleAssemblyNameTypeSerializer()
	       .UseRecommendedSerializerSettings()
	       .UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection")));

		services.AddHangfireServer();


		return services;
	}
}