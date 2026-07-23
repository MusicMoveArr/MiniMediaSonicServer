namespace BaseTests.Configurations;

public class AuthenticationConfiguration
{
    public required string Username { get; init; }
    public required string Salt { get; init; }
    public required string Token { get; init; }
}