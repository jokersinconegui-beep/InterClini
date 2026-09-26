using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using InterClini.Application.DTOs;
using InterClini.Application.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace InterClini.Infrastructure.Services;

public class FhirService : IFhirService
{
    private readonly FhirClient _client;

    public FhirService(FhirClient client)
    {
        _client = client;
    }

    public async Task<PatientDto?> GetPatientAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var patient = await _client.ReadAsync<Patient>($"Patient/{id}");
            return patient is null ? null : MapToDto(patient);
        }
        catch (FhirOperationException ex) when (ex.Status == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }
    public async Task<IReadOnlyList<PatientDto>> SearchPatientsAsync(string familyName, CancellationToken cancellationToken = default)
    {
        var searchParams = new SearchParams().Where($"family={familyName}");
        var bundle = await _client.SearchAsync<Patient>(searchParams);

        if (bundle?.Entry is null)
            return [];

        return [.. bundle.Entry
    .Select(e => e.Resource)
    .OfType<Patient>()
    .Select(MapToDto)];
    }

    public async Task<ObservationDto> CreateObservationAsync(ObservationDto dto, CancellationToken cancellationToken = default)
    {
        var observation = new Observation
        {
            Status = ObservationStatus.Final,
            Code = new CodeableConcept("http://loinc.org", dto.Code, dto.Display),
            Subject = new ResourceReference($"Patient/{dto.PatientId}"),
            Value = new Quantity
            {
                Value = dto.Value,
                Unit = dto.Unit
            },
            Effective = new FhirDateTime(dto.EffectiveDate ?? DateTime.UtcNow)
        };

        var created = await _client.CreateAsync(observation);
        if (created is null)
            throw new InvalidOperationException("FHIR server returned null on create.");

        return MapToDto(created);
    }
    private static PatientDto MapToDto(Patient patient)
    {
        var name = patient.Name?.FirstOrDefault();
        return new PatientDto
        {
            Id = patient.Id ?? string.Empty,
            FamilyName = name?.Family ?? string.Empty,
            GivenName = name?.Given?.FirstOrDefault() ?? string.Empty,
            Gender = patient.Gender?.ToString() ?? string.Empty,
            BirthDate = patient.BirthDateElement?.Value is string birthDateStr
    && DateOnly.TryParse(birthDateStr, out var bd)
        ? bd
        : null
        };
    }

    private static ObservationDto MapToDto(Observation observation)
    {
        var coding = observation.Code?.Coding?.FirstOrDefault();
        var quantity = observation.Value as Quantity;

        return new ObservationDto
        {
            Id = observation.Id ?? string.Empty,
            PatientId = observation.Subject?.Reference?.Replace("Patient/", "") ?? string.Empty,
            Code = coding?.Code ?? string.Empty,
            Display = coding?.Display ?? string.Empty,
            Value = quantity?.Value,
            Unit = quantity?.Unit ?? string.Empty,
            EffectiveDate = observation.Effective is FhirDateTime fdt
    && fdt.ToDateTimeOffset(TimeSpan.Zero) is DateTimeOffset dto
        ? dto.DateTime
        : null
        };
    }
}