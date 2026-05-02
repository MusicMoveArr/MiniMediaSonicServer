using System.Diagnostics;

namespace MiniMediaSonicServer.Application.Services;

public class TranscodeService
{
    public async Task<Stream> TranscodeAsync(string filePath, string targetFormat, int bitrate)
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
                FileName = "ffmpeg",
                Arguments = $"-i \"{escapedFilePath}\" -map 0:a:0 -b:a {bitrate}k -v 0 {parameters}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };

        process.Start();
        return process.StandardOutput.BaseStream;
    }
}