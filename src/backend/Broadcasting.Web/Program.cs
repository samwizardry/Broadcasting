using Broadcasting.ServiceDefaults;
using Broadcasting.SharedKernel.Abstractions;
using Broadcasting.SharedKernel.Services;
using Broadcasting.Streaming;

using FastEndpoints;
using FastEndpoints.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHttpClient();

builder.Services.AddFastEndpoints()
    .SwaggerDocument();

// SharedKernel
builder.Services.AddSingleton<ISlugifier, DefaultSlugifier>();

// Modules
builder.AddStreamingModule();

var app = builder.Build();

// Migrate module databases
await app.MigrateStreamingAsync();

app.MapDefaultEndpoints();

app.UseFastEndpoints(options =>
    {
        options.Endpoints.RoutePrefix = "api";
    })
    .UseSwaggerGen();

app.Run();
