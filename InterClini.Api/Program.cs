using Hl7.Fhir.Rest;
using InterClini.Api.Endpoints;
using InterClini.Application.Interfaces;
using InterClini.Application.Options;
using InterClini.Infrastructure.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<FhirServerOptions>(
    builder.Configuration.GetSection(FhirServerOptions.SectionName));

builder.Services.AddSingleton(sp =>
{
    var options = sp.GetRequiredService<IOptions<FhirServerOptions>>().Value;

    var settings = new FhirClientSettings
    {
        PreferredFormat = ResourceFormat.Json,
        VerifyFhirVersion = false
    };

    return new FhirClient(options.BaseUrl, settings);
});

builder.Services.AddScoped<IFhirService, FhirService>();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapFhirEndpoints();

app.Run();