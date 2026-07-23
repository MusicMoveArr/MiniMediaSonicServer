using MiniMediaSonicServer.WebJob.AutoLike.Application.Services;

namespace MiniMediaSonicServer.WebJob.AutoLike.Application.Models.Database;

public class RateDuplicateTrackModel
{
    public Guid MetadataId { get; set; }
    public int Rating { get; set; }
    public DateTime? RatedAt { get; set; }
    public string? AcoustIdFingerprint { get; set; }
    public required string ArtistName { get; set; }
    public required string TrackTitle { get; set; }
    
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
                return [];
            }

            FingerPrintService fingerPrintService = new FingerPrintService();
            _acoustIdFingerprintData = fingerPrintService.DecodeAcoustIdFingerprint(AcoustIdFingerprint);
            return _acoustIdFingerprintData ?? [];
        }
    }
}