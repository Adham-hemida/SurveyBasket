using Serilog;
using SurveyBasket;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDependencies(builder.Configuration);
builder.Host.UseSerilog((context,configuration) 
    => configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddMemoryCache();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json","v1"));
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();
app.UseExceptionHandler();
app.Run();
