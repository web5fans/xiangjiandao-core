using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Moq;
using Xiangjiandao.Infrastructure;
using Xiangjiandao.Web.Utils;
using Xunit.Abstractions;

namespace Xiangjiandao.Web.Tests;

public class JwtVerityFactory : MyWebApplicationFactory
{
    protected override void SetupMockService(IWebHostBuilder builder)
    {
        var mockOption = new Mock<IOptions<JwtConfig>>();
        mockOption.Setup(x => x.Value).Returns(new JwtConfig()
        {
            Issuer = "Issuer",
            Audience = "Audience",
            ExpirationInMinutes = 10
        });
        builder.ConfigureServices(services => { 
            services.Replace(ServiceDescriptor.Singleton<IOptions<JwtConfig>>(p => mockOption.Object));});
    }

    protected override Task InitializeServiceAsync()
    {
        return Task.CompletedTask;
    }
}

[Collection("api")]
public class JwtVerityTest : IClassFixture<JwtVerityFactory>
{

    readonly JwtVerityFactory _factory;

    readonly HttpClient _client;
    
    readonly IServiceScope _serviceScope;
    
    readonly ILogger<JwtVerityTest> logger;
    

    public JwtVerityTest(ITestOutputHelper testOutputHelper, JwtVerityFactory factory)
    {
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            // db.Database.Migrate();
        }

        _factory = factory;
        _client = factory.WithWebHostBuilder(builder => { builder.ConfigureServices(p => { }); }).CreateClient();
        _serviceScope = factory.Services.CreateScope();
        logger = factory.Services.GetRequiredService<ILogger<JwtVerityTest>>();
    }
    
    [Fact]
    public async Task VerityJwt()
    {
        var guid = Guid.NewGuid();
        var userData = new UserData
        {
            Id = Guid.Parse(guid.ToString()),
            Email = "zhengzhou@rivtower.com",
            Phone = string.Empty,
            PhoneRegion = string.Empty,
            DomainName = string.Empty,
        };
        var jwtGenerator = _factory.Services.GetRequiredService<JwtGenerator>();
        var generate = await jwtGenerator.Generate(userData);
        var httpRequestMessage = new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            Headers =
            {
                Authorization = new AuthenticationHeaderValue("Bearer", generate.AccessToken.Substring(7))
            },
            RequestUri = new Uri("/api/v1/user/jwt", UriKind.Relative)
        };
        var httpResponseMessage = await _client.SendAsync(httpRequestMessage);
        Assert.True(httpResponseMessage.IsSuccessStatusCode);
    }
    
    [Fact]
    public async Task VerityAdminJwt()
    {
        var adminUserData = new AdminUserData
        {
            Id = Guid.NewGuid(),
            Email = "zhengzhou@rivtower.com",
            Phone = "12345678901",
            PhoneRegion = "86",
            Role = "admin"
        };
        var jwtGenerator = _serviceScope.ServiceProvider.GetRequiredService<JwtGenerator>();
        var generate = await jwtGenerator.Generate(adminUserData);
        Assert.NotEmpty(generate.AccessToken);
    }
}