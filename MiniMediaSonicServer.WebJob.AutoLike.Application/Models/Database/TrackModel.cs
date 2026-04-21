using MiniMediaSonicServer.WebJob.AutoLike.Application.Services;

namespace MiniMediaSonicServer.WebJob.AutoLike.Application.Models.Database;

public class TrackModel
{
    public string ArtistName { get; set; }
    public Guid ArtistId { get; set; }
    
    public string AlbumTitle { get; set; }
    public Guid AlbumId { get; set; }
    
    public Guid MetadataId { get; set; }
    public string TrackTitle { get; set; }
    public string AcoustIdFingerprint { get; set; }
    public int Rating { get; set; }
    public DateTime? RatedAt { get; set; }
    
    private int[]? _acoustIdFingerprintData;
    public int[] AcoustIdFingerprintData
    {
        get
        {
            if (_acoustIdFingerprintData?.Length > 0)
            {
                return _acoustIdFingerprintData;
            }
            
            if (string.IsNullOrWhiteSpace(AcoustIdFingerprint))
            {
                return new int[0];
            }

            FingerPrintService fingerPrintService = new FingerPrintService();
            _acoustIdFingerprintData = fingerPrintService.DecodeAcoustIdFingerprint(AcoustIdFingerprint);
            return _acoustIdFingerprintData;
        }
    }
}