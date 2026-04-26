using System.Security.Cryptography.X509Certificates;

namespace MiniMediaSonicServer.Api.Certificates;

//thanks to, https://github.com/joseftw/jos.echo
public class NullCertificateReader : ICertificateReader
{
    public X509Certificate2? Read()
    {
        return null;
    }
}