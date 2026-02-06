using System.Diagnostics;

namespace MiniMediaSonicServer.Application.Services;

public class TranscodeService
{
    public async Task<byte[]?> TranscodeAsync(string filePath, string targetFormat, int bitrate)
    {
        string parameters = string.Empty;
        switch (targetFormat)
        {
            case "mp3":
                parameters = "-f mp3 -";
                break;
            case "opus":
                parameters = "-c:a libopus -f opus -";
                break;
            case "aac":
                parameters = "-c:a aac -f adts -";
                break;
            default:
                return null;
        }
        
        string escapedFilePath = filePath.Replace("\"", "\\\"");
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",  // Command to call
                Arguments = $"-i \"{escapedFilePath}\" -map 0:a:0 -b:a {bitrate}k -v 0 {parameters}",  // Path to the audio file
                RedirectStandardOutput = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };

        process.Start();
        
        using MemoryStream stream = new MemoryStream();
        await process.StandardOutput.BaseStream.CopyToAsync(stream);
        
        await process.WaitForExitAsync();
        return stream.ToArray();
    }
}