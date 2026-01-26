using System.Security.Cryptography;
using System.Text;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using MiniMediaSonicServer.Application.Configurations;
using MiniMediaSonicServer.Application.Repositories;
using Aes = System.Security.Cryptography.Aes;

namespace MiniMediaSonicServer.Api.Filters;

public sealed class SubsonicAuthFilter : IAsyncActionFilter
{
    private readonly UserRepository _userRepository;
    private readonly EncryptionKeysConfiguration _encryptionKeysConfiguration;
    
    public SubsonicAuthFilter(
        UserRepository userRepository,
        IOptions<EncryptionKeysConfiguration> encryptionKeysConfiguration)
    {
        _userRepository = userRepository;
        _encryptionKeysConfiguration = encryptionKeysConfiguration.Value;
    }
    
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var ctx = context.HttpContext;
        var q = ctx.Request.Query;
        var username = q["u"].FirstOrDefault() ?? "";
        var password = q["p"].FirstOrDefault() ?? "";
        var token = q["t"].FirstOrDefault() ?? "";
        var salt = q["s"].FirstOrDefault() ?? "";
        
        Console.WriteLine(ctx.Request.Path);

        if (string.IsNullOrWhiteSpace(username))
        {
            context.Result = SubsonicResults.FailActionResult(ctx, 10, "Required parameter is missing");
            return;
        }

        var authenticated = false;
        var user = await _userRepository.GetUserByUsernameAsync(username);
        
        if (user == null)
        {
            context.Result = SubsonicResults.FailActionResult(ctx, 40, "Wrong username or password");
            return;
        }
        
        if (!string.IsNullOrWhiteSpace(token) && !string.IsNullOrWhiteSpace(salt))
        {
            authenticated = ValidateToken(user.TokenBasedAuth, token, salt);
        }
        else if (!string.IsNullOrWhiteSpace(password))
        {
            authenticated = ValidatePassword(password, user.Password);
        }

        if (!authenticated)
        {
            context.Result = SubsonicResults.FailActionResult(ctx, 40, "Wrong username or password");
            return;
        }

        ctx.Items["user"] = user;
        await next();
    }

    private bool ValidateToken(string hashedPassword, string token, string salt)
    {
        string password = DecryptPassword(hashedPassword, Convert.FromHexString(_encryptionKeysConfiguration.UserPasswordKey));
        string serverToken = Md5Hex(password + salt);

        return CryptographicOperations.FixedTimeEquals(
            Encoding.ASCII.GetBytes(serverToken),
            Encoding.ASCII.GetBytes(token.ToLowerInvariant()));
    }
    private static string Md5Hex(string input)
    {
        using var md5 = MD5.Create();
        byte[] bytes = Encoding.UTF8.GetBytes(input);
        byte[] hash = md5.ComputeHash(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private bool ValidatePassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
    
    private string DecryptPassword(string encrypted, byte[] key)
    {
        byte[] combined = Convert.FromBase64String(encrypted);

        using var aes = Aes.Create();
        aes.Key = key;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        byte[] iv = new byte[aes.BlockSize / 8];
        byte[] cipherText = new byte[combined.Length - iv.Length];

        Buffer.BlockCopy(combined, 0, iv, 0, iv.Length);
        Buffer.BlockCopy(combined, iv.Length, cipherText, 0, cipherText.Length);

        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        byte[] plaintextBytes = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);

        return Encoding.UTF8.GetString(plaintextBytes);
    }
}