using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using MiniMediaSonicServer.Application.Configurations;
using MiniMediaSonicServer.Application.Models.Database;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Repositories;
using Aes = System.Security.Cryptography.Aes;

namespace MiniMediaSonicServer.Application.Services;

public class UserService
{
    private readonly UserRepository _userRepository;
    private readonly EncryptionKeysConfiguration _encryptionKeysConfiguration;
    public UserService(UserRepository userRepository,
        IOptions<EncryptionKeysConfiguration> encryptionKeysConfiguration)
    {
        _userRepository = userRepository;
        _encryptionKeysConfiguration = encryptionKeysConfiguration.Value;
    }
    
    public async Task<UserModel?> GetUserByUsernameAsync(string username)
    {
        return await _userRepository.GetUserByUsernameAsync(username);
    }
    
    public async Task<List<UserModel>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllUsersAsync();
    }

    public async Task UpdateUserAsync(UpdateUserRequest request)
    {
        await _userRepository.UpdateUserByUsernameAsync(request);

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            string encryptedPass = EncryptPassword(request.Password, 
                Convert.FromHexString(_encryptionKeysConfiguration.UserPasswordKey));
            string hashedbcrypt = BCrypt.Net.BCrypt.HashPassword(request.Password);
            
            await _userRepository.UpdatePasswordByUsernameAsync(request.Username, hashedbcrypt, encryptedPass);
        }
    }

    public async Task<bool> CreateUserAsync(CreateUserRequest request)
    {
        if (await _userRepository.UserExistsByUsernameAsync(request.Username) ||
            await _userRepository.UserExistsByEmailAsync(request.Email))
        {
            return false;
        }

        string encryptedPass = EncryptPassword(request.Password, 
            Convert.FromHexString(_encryptionKeysConfiguration.UserPasswordKey));
        string hashedbcrypt = BCrypt.Net.BCrypt.HashPassword(request.Password);
        
        await _userRepository.CreateUserAsync(request, hashedbcrypt, encryptedPass);
        return true;
    }
    
    public async Task SetUserDeletedByUsernameAsync(string username)
    {
        await _userRepository.SetUserDeletedByUsernameAsync(username);
    }
    
    public async Task<DateTime?> GetUserDateTimeAsync(Guid userId)
    {
        TimeZoneInfo? userTimezone = await _userRepository.GetTimezoneByUserIdAsync(userId);
        return userTimezone != null ? DateTime.UtcNow.Add(userTimezone.BaseUtcOffset) : null;
    }
    
    public async Task<DateTime> GetUserOrServerDateTimeAsync(Guid userId)
    {
        DateTime? userTime = await GetUserDateTimeAsync(userId);
        return userTime ?? DateTime.Now;
    }

    public bool ValidatePassword(string password, string hashedPassword)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message + "\r\n" + e.StackTrace);
            return false;
        }
    }
    public bool ValidateToken(string hashedPassword, string token, string salt)
    {
        string password = DecryptPassword(hashedPassword, Convert.FromHexString(_encryptionKeysConfiguration.UserPasswordKey));

        if (string.IsNullOrWhiteSpace(password))
        {
            return false;
        }
        
        string serverToken = Md5Hex(password + salt);

        return CryptographicOperations.FixedTimeEquals(
            Encoding.ASCII.GetBytes(serverToken),
            Encoding.ASCII.GetBytes(token.ToLowerInvariant()));
    }
    
    private string EncryptPassword(string plaintext, byte[] key)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        aes.GenerateIV();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var encryptor = aes.CreateEncryptor();
        byte[] plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
        byte[] cipherText = encryptor.TransformFinalBlock(plaintextBytes, 0, plaintextBytes.Length);

        // Store IV + ciphertext
        byte[] combined = new byte[aes.IV.Length + cipherText.Length];
        Buffer.BlockCopy(aes.IV, 0, combined, 0, aes.IV.Length);
        Buffer.BlockCopy(cipherText, 0, combined, aes.IV.Length, cipherText.Length);

        return Convert.ToBase64String(combined);
    }

    public async Task CreateFirstUserAsync()
    {
        bool anyUserExists = await _userRepository.AnyUserExistsAsync();
        if (!anyUserExists)
        {
            CreateUserRequest newUserRequest = new CreateUserRequest
            {
                AdminRole = true,
                CommentRole = true,
                CoverArtRole = true,
                DownloadRole = true,
                Email = "1",
                JukeboxRole = true,
                Password = Guid.NewGuid().ToString(),
                PodcastRole = true,
                ShareRole = true,
                SettingsRole = true,
                StreamRole = true,
                UploadRole = true,
                Username = "admin",
                VideoConversionRole = true,
                MusicFolderId = 1,
                MaxBitRate = 0
            };
            bool success = await CreateUserAsync(newUserRequest);
            if (success)
            {
                Console.WriteLine($"First account is created, username: {newUserRequest.Username}, password: {newUserRequest.Password}");
            }
        }
    }

    private string Md5Hex(string input)
    {
        using var md5 = MD5.Create();
        byte[] bytes = Encoding.UTF8.GetBytes(input);
        byte[] hash = md5.ComputeHash(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
    
    private string DecryptPassword(string encrypted, byte[] key)
    {
        Span<byte> combined = new Span<byte>(new byte[256]);
        
        if (!Convert.TryFromBase64String(encrypted, combined, out int combinedLength))
        {
            return string.Empty;
        }

        using var aes = Aes.Create();
        aes.Key = key;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        byte[] iv = new byte[aes.BlockSize / 8];
        byte[] cipherText = new byte[combinedLength - iv.Length];

        byte[] combinedBytes = combined.ToArray();
        Buffer.BlockCopy(combinedBytes, 0, iv, 0, iv.Length);
        Buffer.BlockCopy(combinedBytes, iv.Length, cipherText, 0, cipherText.Length);

        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        byte[] plaintextBytes = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);

        return Encoding.UTF8.GetString(plaintextBytes);
    }
}