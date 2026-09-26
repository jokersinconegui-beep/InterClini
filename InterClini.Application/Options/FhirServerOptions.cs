namespace InterClini.Application.Options;

public class FhirServerOptions
{
    public const string SectionName = "FhirServer";

    public string BaseUrl { get; set; } = string.Empty;
}