using InterClini.Application.DTOs;
using InterClini.Application.Interfaces;

namespace InterClini.Api.Endpoints;

public static class FhirEndpoints
{
    public static IEndpointRouteBuilder MapFhirEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/fhir")
            .WithTags("FHIR");

        group.MapGet("/patient/{id}", async (
            string id,
            IFhirService fhirService,
            CancellationToken cancellationToken) =>
        {
            var patient = await fhirService.GetPatientAsync(id, cancellationToken);

            return patient is null
                ? Results.NotFound(new { message = $"Patient {id} not found" })
                : Results.Ok(patient);
        })
        .WithName("GetPatient")
        .Produces<PatientDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/patients", async (
            string family,
            IFhirService fhirService,
            CancellationToken cancellationToken) =>
        {
            var patients = await fhirService.SearchPatientsAsync(family, cancellationToken);
            return Results.Ok(patients);
        })
        .WithName("SearchPatients")
        .Produces<IReadOnlyList<PatientDto>>(StatusCodes.Status200OK);

        group.MapPost("/observation", async (
            ObservationDto dto,
            IFhirService fhirService,
            CancellationToken cancellationToken) =>
        {
            var created = await fhirService.CreateObservationAsync(dto, cancellationToken);
            return Results.Created($"/api/fhir/observation/{created.Id}", created);
        })
        .WithName("CreateObservation")
        .Produces<ObservationDto>(StatusCodes.Status201Created);

        return app;
    }
}