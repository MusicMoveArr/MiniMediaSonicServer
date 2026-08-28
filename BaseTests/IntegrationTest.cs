using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RestSharp.Serializers.Json;
using BaseTests.Configurations;
using RestSharp;
using Xunit;

namespace BaseTests;

[Trait("Category", "Integration")]
public abstract class IntegrationTest : IClassFixture<ApiWebApplicationFactory>
{
    protected readonly ApiWebApplicationFactory _factory;
    public readonly RestClient Client;
    protected readonly AuthenticationConfiguration? _authConfig;
 
    public IntegrationTest(ApiWebApplicationFactory fixture)
    {
        _factory = fixture;
        
        var options = new JsonSerializerOptions
        {
            TypeInfoResolver = new DefaultJsonTypeInfoResolver
            {
                Modifiers = { ti =>
                {
                    foreach (var prop in ti.Properties)
                        prop.IsRequired = false;
                }}
            }
        };
        
        Client = new RestClient(_factory.CreateClient(), configureSerialization: s => s.UseSystemTextJson(options));
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