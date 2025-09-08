using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

var builder = DistributedApplication.CreateBuilder(args);

var pg = builder.AddPostgres("pg").WithEnvironment("POSTGRES_PASSWORD","postgres").WithDataVolume();
var db = pg.AddDatabase("turnament");

var api = builder.AddProject("Turnament.Api", "../src/Turnament.Api/Turnament.Api.csproj")
    .WithReference(db)
    .WithEnvironment("ConnectionStrings__Default", "Host=localhost;Port=5432;Database=turnament;Username=postgres;Password=postgres");

builder.Build().Run();