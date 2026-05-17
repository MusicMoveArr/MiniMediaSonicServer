namespace MiniMediaSonicServer.Application.Models;

public class CoverArtModel
{
    public byte[] CoverData { get; set; }
    public string ContentType { get; private set; }

    public CoverArtModel(byte[] coverData, string contentType)
    {
        this.CoverData = coverData;
        this.ContentType = contentType;
    }

    public CoverArtModel(string filePath, byte[] coverData)
    {
        this.CoverData = coverData;
        if (filePath.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
        {
            ContentType = "image/png";
        }
        else if (filePath.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
        {
            ContentType = "image/jpeg";
        }
        else if (filePath.EndsWith(".webp", StringComparison.OrdinalIgnoreCase))
        {
            ContentType = "image/webp";
        }
        else
        {
            ContentType = "image/jpeg";
        }
    }
}