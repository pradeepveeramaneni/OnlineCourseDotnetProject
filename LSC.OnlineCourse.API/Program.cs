
using LSC.OnlineCourse.API.common;
using LSC.OnlineCourse.API.Middlewares;
using LSC.OnlineCourse.Data;
using LSC.OnlineCourse.Data.Entities;
using LSC.OnlineCourse.Service;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Logging;
using Serilog;
using Serilog.Templates;
using System.Linq.Expressions;
using System.Net;

namespace LSC.OnlineCourse.API
{
    public class Program
    {
        public static void Main(string[] args)
        { // Configure Serilog with the settings
            Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.Debug()
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .CreateBootstrapLogger();


            try {

                var builder = WebApplication.CreateBuilder(args);
                var configuration = builder.Configuration;

                builder.Services.AddHealthChecks()
                    .AddSqlServer(
                      connectionString: configuration.GetConnectionString("DbContext"),
                      healthQuery: "SELECT 1;", // Query to check database health.
                      name: "sqlserver",
                      failureStatus: HealthStatus.Degraded, // Degraded health status if the check fails.
                      tags: new[] { "db", "sql" })
                   .AddCheck("Memory", new PrivateMemoryHealthCheck(1024 * 1024 * 1024)); // A custom health check for memory.



                builder.Services.AddApplicationInsightsTelemetry();

                builder.Host.UseSerilog((context, services, loggerConfiguration) => loggerConfiguration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .WriteTo.Console(new ExpressionTemplate(
                    // Include trace and span ids when present.
                    "[{@t:HH:mm:ss} {@l:u3}{#if @tr is not null} ({substring(@tr,0,4)}:{substring(@sp,0,4)}){#end}] {@m}\n{@x}"))
                .WriteTo.ApplicationInsights(
                  services.GetRequiredService<TelemetryConfiguration>(),
                  TelemetryConverter.Traces));

                Log.Information("Starting the smartcodebypradeepraoveeramaneni API...");


                // Add services to the container.

                builder.Services.AddDbContextPool<OnlineCourseDbContext>(options =>
                {
                    options.UseSqlServer(
                        configuration.GetConnectionString("DbContext"),
                        provideroptions => provideroptions.EnableRetryOnFailure()
                        );
                   // options.EnableSensitiveDataLogging();
                    options.EnableDetailedErrors();
                });

                builder.Services.AddControllers();

                builder.Services.AddAutoMapper(typeof(MappingProfile));

                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                builder.Services.AddScoped<ICourseCategoryRepository, CourseCategoryRepository>();
                builder.Services.AddScoped<ICourseCategoryService, CourseCategoryService>();

                builder.Services.AddScoped<ICourseRepository, CourseRepository>();
                builder.Services.AddScoped<ICourseService, CourseService>();

                builder.Services.AddScoped<IVideoRequestRepository, VideoRequestRepository>();
                builder.Services.AddScoped<IVideoRequestService, VideoRequestService>();

                builder.Services.AddTransient<RequestBodyLoggingMiddleware>();
                builder.Services.AddTransient<ResponseBodyLoggingMiddleware>();
                builder.Services.AddScoped<IUserClaims, UserClaims>();


                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("default", policy =>
                    {
                        policy.WithOrigins("http://localhost:4200", "https://smartcoddingbypradeep-ui.azurewebsites.net") // Corrected frontend URL without trailing slash
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials();

                    });
                });

                #region AD B2C configuration
                builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                  .AddMicrosoftIdentityWebApi(options =>
                  {
                      configuration.Bind("AzureAdB2C", options);

                      options.Events = new JwtBearerEvents();
                      //{
                      //    OnTokenValidated = context =>
                      //    {
                      //        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();

                      //        // Access the scope claim (scp) directly
                      //        var scopeClaim = context.Principal?.Claims.FirstOrDefault(c => c.Type == "scp")?.Value;

                      //        if (scopeClaim != null)
                      //        {
                      //            logger.LogInformation("Scope found in token: {Scope}", scopeClaim);
                      //        }
                      //        else
                      //        {
                      //            logger.LogWarning("Scope claim not found in token.");
                      //        }

                      //        return Task.CompletedTask;
                      //    },
                      //    OnAuthenticationFailed = context =>
                      //    {
                      //        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                      //        logger.LogError("Authentication failed: {Message}", context.Exception.Message);
                      //        return Task.CompletedTask;
                      //    },
                      //    OnChallenge = context =>
                      //    {
                      //        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                      //        logger.LogError("Challenge error: {ErrorDescription}", context.ErrorDescription);
                      //        return Task.CompletedTask;
                      //    }
                      //};
                  }, options => { configuration.Bind("AzureAdB2C", options); });

                // The following flag can be used to get more descriptive errors in development environments
                IdentityModelEventSource.ShowPII = false;
                #endregion AD B2C configuration

                var app = builder.Build();

                app.UseCors("default");
                app.UseExceptionHandler(errorApp =>
                {
                    errorApp.Run(async context =>
                    {
                        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                        var exception = exceptionHandlerPathFeature?.Error;

                        Log.Error(exception, "Unhandled exception occurred. {ExceptionDetails}", exception?.ToString());
                        Console.WriteLine(exception?.ToString());
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        await context.Response.WriteAsync("An unexpected error occurred. Please try again later.");
                    });
                });

                app.UseMiddleware<RequestResponseLoggingMiddleware>();
                app.UseMiddleware<RequestBodyLoggingMiddleware>();
                app.UseMiddleware<ResponseBodyLoggingMiddleware>();

                // Configure the HTTP request pipeline.
               // if (app.Environment.IsDevelopment())
               // {
                    app.UseSwagger();
                    app.UseSwaggerUI();
               // }

                app.UseRouting();

                app.UseHttpsRedirection();

                app.UseAuthentication();
                app.UseAuthorization();



                app.MapHealthChecks("/health", new HealthCheckOptions
                {
                    ResponseWriter = HealthCheckResponseWriter.WriteJsonResponse
                });

                // Liveness probe
                app.MapHealthChecks("/health/live", new HealthCheckOptions
                {
                    Predicate = _ => false, // No specific checks, just indicates the app is live
                    ResponseWriter = async (context, report) =>
                    {
                        context.Response.ContentType = "application/json";
                        var json = new
                        {
                            status = report.Status.ToString(),
                            description = "Liveness check - the app is up"
                        };
                        await context.Response.WriteAsJsonAsync(json);
                    }
                });

                // Readiness probe
                app.MapHealthChecks("/health/ready", new HealthCheckOptions
                {
                    Predicate = check => check.Tags.Contains("ready"), // Only run checks tagged as "ready"
                    ResponseWriter = HealthCheckResponseWriter.WriteJsonResponse
                });



                app.MapControllers();

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }


        }
}
}
