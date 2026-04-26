using System.Security.Cryptography.X509Certificates;

namespace MiniMediaSonicServer.Api.Certificates;

//thanks to, https://github.com/joseftw/jos.echo
public class CachingMountedCertificateReader : ICertificateReader
{
    private static readonly SemaphoreSlim Semaphore;
    private readonly MountedCertificateReader _certificateReader;
    private static DateTime? _lastRefreshed;
    private static X509Certificate2? _certificate;

    static CachingMountedCertificateReader()
    {
        Semaphore = new SemaphoreSlim(1, 1);
    }

    public CachingMountedCertificateReader(MountedCertificateReader certificateReader)
    {
        _certificateReader = certificateReader ?? throw new ArgumentNullException(nameof(certificateReader));
    }

    public X509Certificate2 Read()
    {
        if (!NeedsRefresh(_certificate))
        {
            return _certificate!;
        }

        Semaphore.Wait();

        if (!NeedsRefresh(_certificate))
        {
            Semaphore.Release(1);
            return _certificate!;
        }

        _certificate = _certificateReader.Read();
        _lastRefreshed = TimeProvider.System.GetUtcNow().DateTime;
        Semaphore.Release(1);
        return _certificate;
    }

    private static bool NeedsRefresh(X509Certificate2? certificate)
    {
        var now = TimeProvider.System.GetUtcNow();
        return certificate is null || certificate.NotAfter <= now || (now - _lastRefreshed!.Value).TotalMinutes >= 60;
    }
}