using MiniMediaSonicServer.Application.Models;
using SmartFormat;

namespace MiniMediaSonicServer.Application.Helpers;

public static class ArtistHelper
{
    public static string GetUncoupledArtistName(string? artist)
    {
        if (string.IsNullOrWhiteSpace(artist))
        {
            return string.Empty;
        }
        
        string[] splitCharacters =
        [
            ",",
            "&",
            "+",
            "/",
            " feat",
            ";"
        ];

        string? newArtistName = splitCharacters
            .Where(splitChar => artist.Contains(splitChar))
            .Select(splitChar => artist.Substring(0, artist.IndexOf(splitChar)).Trim())
            .Where(split => split.Length > 0)
            .OrderBy(split => split.Length)
            .FirstOrDefault();

        if (string.IsNullOrWhiteSpace(newArtistName))
        {
            return artist;
        }
        return newArtistName;
    }
    
    public static string GetShortWordVersion(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        if (value.Length <= maxLength)
        {
            return value;
        }

        string result = string.Empty;
        string[] wordSplit = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        foreach (string word in wordSplit)
        {
            if (result.Length + word.Length <= maxLength)
            {
                result += word + " ";
            }
        }

        if (string.IsNullOrEmpty(result) && value.Length >= maxLength)
        {
            result = value.Substring(0, maxLength);
        }


        char[] charsToCleanup = "`~!@#$%^&*()_+-=[]{};':\",./<>? ".ToCharArray();
        while(result.Length > 0)
        {
            if (!result.Skip(result.Length - 1).TakeLast(1).Any(c => charsToCleanup.Contains(c)))
            {
                break;
            }

            result = result.Substring(0, result.Length - 1);
        }

        return result.Trim();
    }
    
    public static string GetFormatName(MusicCacheTrackModel model, string format, string seperator)
    {
        model.Artist = ReplaceDirectorySeparators(model.Artist, seperator);
        model.Title = ReplaceDirectorySeparators(model.Title, seperator);
        model.Album = ReplaceDirectorySeparators(model.Album, seperator);
        
        format = Smart.Format(format, model);
        format = format.Trim();
        return format;
    }
    
    public static string ReplaceDirectorySeparators(string? input, string seperator)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }
        
        if (input.Contains('/'))
        {
            input = input.Replace("/", seperator);
        }
        else if (input.Contains('\\'))
        {
            input = input.Replace("\\", seperator);
        }

        return input;
    }
}