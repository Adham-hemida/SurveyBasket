using Hangfire;
using HangfireBasicAuthenticationFilter;
using Serilog;
using SurveyBasket;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDependencies(builder.Configuration);
builder.Host.UseSerilog((context, configuration)
	=> configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddDistributedMemoryCache();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
//	app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "v1"));
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseHangfireDashboard("/jobs", new DashboardOptions
{
	Authorization =
	[
       new HangfireCustomBasicAuthenticationFilter
	   {
		   User = app.Configuration.GetValue<string>("HangfireSettings:Username"),
		   Pass = app.Configuration.GetValue<string>("HangfireSettings:Password")
	   }
	]
});

app.UseCors();
app.UseAuthorization();
app.MapControllers();
app.UseExceptionHandler();
app.UseRateLimiter();
app.MapHealthChecks("health",new HealthCheckOptions
{
	ResponseWriter=UIResponseWriter.WriteHealthCheckUIResponse
});
app.Run();
