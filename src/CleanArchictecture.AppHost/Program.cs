var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.CleanArchictecture_WebAPI>("cleanarchictecture-webapi");

builder.Build().Run();
