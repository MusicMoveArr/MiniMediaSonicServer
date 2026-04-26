namespace MiniMediaSonicServer.Api.Certificates;

//thanks to, https://github.com/joseftw/jos.echo
public class TlsConfiguration
{
    public bool HasCertificate => !string.IsNullOrWhiteSpace(Certificate?.CertificateFile);
    public CertificateConfiguration? Certificate { get; set; }
}