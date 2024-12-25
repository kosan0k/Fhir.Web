var builder = DistributedApplication.CreateBuilder(args);

var mongo = builder.AddMongoDB("mongo")
                   .WithLifetime(ContainerLifetime.Persistent)
                   .WithDataVolume();

var mongodb = mongo.AddDatabase("mongodb");

builder.AddProject<Projects.Fhir_Patients_Web>(name: "fhir-patients-web")
       .WithReference(mongodb)
       .WaitFor(mongodb);

builder.Build().Run();
