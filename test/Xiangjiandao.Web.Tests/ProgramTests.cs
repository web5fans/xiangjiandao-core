using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Moq;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Tests
{
    
    public class ProgramTestsFactory : MyWebApplicationFactory
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
                services.Replace(ServiceDescriptor.Singleton<IOptions<JwtConfig>>(p => mockOption.Object));
            });
        }

        protected override Task InitializeServiceAsync()
        {
            return Task.CompletedTask;
        }
    }
    
    [Collection("web")]
    public class ProgramTests : IClassFixture<ProgramTestsFactory>
    {
        private readonly ProgramTestsFactory _factory;

        private readonly HttpClient _client;

        public ProgramTests(ProgramTestsFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }


        [Fact]
        public async Task HealthCheckTest()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/health");
            Assert.True(response.IsSuccessStatusCode);
        }
    }
}