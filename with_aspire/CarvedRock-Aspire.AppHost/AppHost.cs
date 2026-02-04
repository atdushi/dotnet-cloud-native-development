using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var carvedRockDb = builder.AddPostgres("postgres")
    .WithImageTag("12")
    .AddDatabase("CarvedRockPostgres");

var idsrv = builder.AddProject<Projects.CarvedRock_Identity>("carvedrock-identity");
var idEndpoint = idsrv.GetEndpoint("https");

var api = builder.AddProject<Projects.CarvedRock_Api>("carvedrock-api")
    .WithEnvironment("Auth__Authority", idEndpoint)
    .WithReference(carvedRockDb);

var smtp = builder.AddSmtp4Dev("SmtpUri");

builder.AddProject<Projects.CarvedRock_WebApp>("carvedrock-webapp")
    .WithEnvironment("Auth__Authority", idEndpoint)
    .WithReference(api)
    .WithReference(smtp);
    //.WithEnvironment("CarvedRock__ApiBaseUrl", api.GetEndpoint("https"));

builder.Build().Run();
