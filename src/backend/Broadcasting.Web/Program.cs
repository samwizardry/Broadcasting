using Broadcasting.SharedKernel.Abstractions;
using Broadcasting.SharedKernel.Services;
using Broadcasting.Streaming;

using FastEndpoints;
using FastEndpoints.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

builder.Services.AddFastEndpoints()
    .SwaggerDocument();

// SharedKernel
builder.Services.AddSingleton<ISlugifier, DefaultSlugifier>();

// Modules
builder.AddStreamingModule();

var app = builder.Build();

//app.UseAuthentication();
//app.UseAuthorization();

app.UseFastEndpoints(options =>
    {
        options.Endpoints.RoutePrefix = "api";
    })
    .UseSwaggerGen();

app.Run();
