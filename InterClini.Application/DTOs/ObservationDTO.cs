namespace InterClini.Application.DTOs;

public class ObservationDto
{
    public string Id { get; set; } = string.Empty;
    public string PatientId { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Display { get; set; } = string.Empty;
    public decimal? Value { get; set; }
    public string Unit { get; set; } = string.Empty;
    public DateTime? EffectiveDate { get; set; }
}