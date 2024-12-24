using Fhir.Patients.Domain.Contracts;
using Fhir.Patients.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Fhir.Patients.Database.Mongo.Registration
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddMongoDb(
            this IServiceCollection services,
            string? databaseName = null)
        {
            //TODO: configure collection names

            databaseName ??= "mongodb";

            services
                .AddSingleton(s =>
                {
                    var mongoClient = s.GetRequiredService<IMongoClient>();
                    return mongoClient.GetDatabase(databaseName).GetCollection<Patient>("patients");
                })
                .AddSingleton<IRepository<Patient>, MongoRepository<Patient>>();

            return services;
        }
    }
}
