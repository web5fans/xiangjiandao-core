using Aliyun.Credentials.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Tea;
using UUIDNext;
using Xiangjiandao.Infrastructure;
using Xiangjiandao.Web.Application.Dto;
using Xiangjiandao.Web.Clients;
using Xiangjiandao.Web.Utils;
using Xunit.Abstractions;

namespace Xiangjiandao.Web.Tests;

public class AliyunSmsTestFactory : MyWebApplicationFactory
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


public class AliyunSmsTest: IClassFixture<JwtVerityFactory>
{

    readonly JwtVerityFactory _factory;

    readonly HttpClient _client;
    
    readonly IServiceScope _serviceScope;
    
    readonly ILogger<JwtVerityTest> logger;
    
    private readonly ITestOutputHelper _testOutputHelper;
    

    public AliyunSmsTest(ITestOutputHelper testOutputHelper, JwtVerityFactory factory)
    {
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            _testOutputHelper = testOutputHelper;
            // db.Database.Migrate();
        }

        _factory = factory;
        _client = factory.WithWebHostBuilder(builder => { builder.ConfigureServices(p => { }); }).CreateClient();
        _serviceScope = factory.Services.CreateScope();
        logger = factory.Services.GetRequiredService<ILogger<JwtVerityTest>>();
    }

    [Fact]
    public async Task GetIAliyunSmsClient()
    {
        var smsClient = _serviceScope.ServiceProvider.GetRequiredService<IAliYunSmsClient>();
        try
        {
            // 复制代码运行请自行打印 API 的返回值
            await smsClient.SendSmsAsync("13858136285", "1232456");
        }
        catch (TeaException error)
        {
            // 此处仅做打印展示，请谨慎对待异常处理，在工程项目中切勿直接忽略异常。
            // 错误 message
            Console.WriteLine(error.Message);
            // 诊断地址
            Console.WriteLine(error.Data["Recommend"]);
            AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
        }
        catch (Exception _error)
        {
            var error = new TeaException(new Dictionary<string, object>
            {
                { "message", _error.Message }
            });
            // 此处仅做打印展示，请谨慎对待异常处理，在工程项目中切勿直接忽略异常。
            // 错误 message
            _testOutputHelper.WriteLine(error.Message);
            // 诊断地址
            Console.WriteLine(error.Data["Recommend"]);
            AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
        }
    }


    [Fact]
    public async Task SendSms()
    {
        var client = CreateClient();
        
        var serializerSettings = new JsonSerializerSettings
        {
            // 设置为驼峰命名
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
        var sendSmsRequest = new AlibabaCloud.SDK.Dysmsapi20170525.Models.SendSmsRequest
        {
            PhoneNumbers = "13858136285",
            SignName = "溪塔科技Rivtower",
            TemplateCode = "SMS_173636116",
            TemplateParam = JsonConvert.SerializeObject(new AliyunSmsCodeDto
            {
                Code = "1232456"
            },serializerSettings),
            OutId = Uuid.NewRandom().ToString()
        };
        try
        {
            // 复制代码运行请自行打印 API 的返回值
            await client.SendSmsWithOptionsAsync(sendSmsRequest, new AlibabaCloud.TeaUtil.Models.RuntimeOptions());
        }
        catch (TeaException error)
        {
            // 此处仅做打印展示，请谨慎对待异常处理，在工程项目中切勿直接忽略异常。
            // 错误 message
            Console.WriteLine(error.Message);
            // 诊断地址
            Console.WriteLine(error.Data["Recommend"]);
            AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
        }
        catch (Exception _error)
        {
            var error = new TeaException(new Dictionary<string, object>
            {
                { "message", _error.Message }
            });
            // 此处仅做打印展示，请谨慎对待异常处理，在工程项目中切勿直接忽略异常。
            // 错误 message
            _testOutputHelper.WriteLine(error.Message);
            // 诊断地址
            Console.WriteLine(error.Data["Recommend"]);
            AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
        }
    }
    
    public static AlibabaCloud.SDK.Dysmsapi20170525.Client CreateClient()
    {
        AlibabaCloud.OpenApiClient.Models.Config config = new AlibabaCloud.OpenApiClient.Models.Config
        {
            // 必填，请确保代码运行环境设置了环境变量 ALIBABA_CLOUD_ACCESS_KEY_ID。
            AccessKeyId = "",
            // 必填，请确保代码运行环境设置了环境变量 ALIBABA_CLOUD_ACCESS_KEY_SECRET。
            AccessKeySecret = "",
        };
        config.Endpoint = "";
       return  new AlibabaCloud.SDK.Dysmsapi20170525.Client(config);
    }

    [Fact]
    public async Task GetSts()
    {
        AlibabaCloud.OpenApiClient.Models.Config config = new AlibabaCloud.OpenApiClient.Models.Config
        {
            // 必填，请确保代码运行环境设置了环境变量 ALIBABA_CLOUD_ACCESS_KEY_ID。
            AccessKeyId = "",
            // 必填，请确保代码运行环境设置了环境变量 ALIBABA_CLOUD_ACCESS_KEY_SECRET。
            AccessKeySecret = "",
        };
        config.Endpoint = "";
        var client =  new AlibabaCloud.SDK.Dysmsapi20170525.Client(config);
        Assert.NotNull(client);
    }
}
