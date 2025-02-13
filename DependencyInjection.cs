using FluentValidation.AspNetCore;
using MapsterMapper;
using SurveyBasket.Persistence;

namespace SurveyBasket;

public static class DependencyInjection
{
	public static IServiceCollection AddDependencies(this IServiceCollection services,IConfiguration configuration)
	{
		var connectionString = configuration.GetConnectionString("DefaultConnection")??
			throw new InvalidOperationException("connection string 'Default connection not found'");
		services.AddDbContext<ApplicationDbContext>(
			options =>options.UseSqlServer(connectionString)
		  );
		services.AddControllers();

		services.
			AddMapsterConfig()
			.AddFluentValidationConfig();
		
		services.AddScoped<IPollService, PollService>();

		services.AddOpenApi();

		return services;
	}
	public static IServiceCollection AddMapsterConfig(this IServiceCollection services)
	{

		//Add Mapster
		var mappingConfig = TypeAdapterConfig.GlobalSettings;
		mappingConfig.Scan(Assembly.GetExecutingAssembly());
		services.AddSingleton<IMapper>(new Mapper(mappingConfig));
		

		return services;
	}
	public static IServiceCollection AddFluentValidationConfig(this IServiceCollection services)
	{
		services
			.AddFluentValidationAutoValidation()
			.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());


		return services;
	}
}
