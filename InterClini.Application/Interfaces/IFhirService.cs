using InterClini.Application.DTOs;

namespace InterClini.Application.Interfaces;

public interface IFhirService
{
    Task<PatientDto?> GetPatientAsync(string id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PatientDto>> SearchPatientsAsync(string familyName, CancellationToken cancellationToken = default);

    Task<ObservationDto> CreateObservationAsync(ObservationDto observation, CancellationToken cancellationToken = default);
}