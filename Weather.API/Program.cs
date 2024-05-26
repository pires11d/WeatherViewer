using Microsoft.OpenApi.Models;
using Quartz;
using System.Reflection;
using Weather.Lib.Jobs;
using Weather.Lib.Services;
using Weather.Lib.Services.Interfaces;
using Weather.Lib.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

//CONFIGURATION
OpenWeatherService.ApiUrl = builder.Configuration["OpenWeather:ApiUrl"];
OpenWeatherService.ApiKey = builder.Configuration["OpenWeather:ApiKey"];
OpenWeatherService.Units = builder.Configuration["OpenWeather:Units"];
RequestHelper.ConfigureClient(OpenWeatherService.ApiUrl);
WeatherService.Cities = builder.Configuration["Configs:DefaultCities"]?.Split(",").Select(x => x.Trim()).ToList();
var jobInterval = builder.Configuration["Configs:JobInterval"] ?? "0 0/2 * * * ?";

//SERVICES
builder.Services.AddScoped<IWeatherService, WeatherService>();
builder.Services.AddScoped<IOpenWeatherService, OpenWeatherService>();

//SCHEDULES
builder.Services.AddQuartz(q =>
{
    //q.UseMicrosoftDependencyInjectionScopedJobFactory();
    var jobKey = new JobKey("ScheduledJob");
    q.AddJob<WeatherJob>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("Job-trigger")
        .WithCronSchedule(jobInterval));

});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

//SWAGGER
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Weather.API", Version = "1.0.0" });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
    c.EnableAnnotations();
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
