namespace MiniMediaSonicServer.Api.Certificates;

//thanks to, https://github.com/joseftw/jos.echo
public class CertificateConfiguration
{
    public bool HasKeyFile => !string.IsNullOrWhiteSpace(KeyFile);
    public bool HasPassword => !string.IsNullOrWhiteSpace(Password);
    public required string CertificateFile { get; init; }
    public string? KeyFile { get; init; }
    public string? Password { get; init; }
}