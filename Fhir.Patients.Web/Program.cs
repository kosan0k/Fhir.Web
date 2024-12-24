
using Fhir.Patients.Database.Mongo.Registration;
using NLog.Extensions.Logging;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Fhir.Patients.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddServiceDefaults(); //Add services for Aspire support

        builder.AddMongoDBClient("mongodb"); //Add mongo db from aspire        

        builder.Services
            .AddLogging( //Add logging via Nlog
                logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                    logging.AddNLog("nlog.config");
                })
            .AddMongoDb()
            .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())) // Add MediatR
            .AddControllers()
            .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

        try
        {
            app.MapDefaultEndpoints();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection()
               .UseAuthorization();

            app.MapControllers();

            app.Run();

            app.Logger.LogInformation("Application started");
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "Can not start the application");
        }
    }
}
