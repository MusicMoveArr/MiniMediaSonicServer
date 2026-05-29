using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MiniMediaSonicServer.Tests.Configurations;
using RestSharp;

namespace MiniMediaSonicServer.Tests;

[Trait("Category", "Integration")]
public abstract class IntegrationTest : IClassFixture<ApiWebApplicationFactory>
{
    protected readonly ApiWebApplicationFactory _factory;
    public readonly RestClient Client;
    protected readonly AuthenticationConfiguration? _authConfig;
 
    public IntegrationTest(ApiWebApplicationFactory fixture)
    {
        _factory = fixture;
        Client = new RestClient(_factory.CreateClient());
        _authConfig = fixture.Configuration.GetSection("Authentication").Get<AuthenticationConfiguration>();
    }

    public RestRequest GetRequest(string url)
    {
        RestRequest request = new RestRequest(url);
        request.AddParameter("u", _authConfig.Username);
        request.AddParameter("t", _authConfig.Token);
        request.AddParameter("s", _authConfig.Salt);
        request.AddParameter("f", "json");
        return request;
    }
}