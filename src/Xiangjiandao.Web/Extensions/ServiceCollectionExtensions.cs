using NetCorePal.Extensions.NewtonsoftJson;
using NetCorePal.Extensions.Primitives;
using NetCorePal.Extensions.ServiceDiscovery;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RabbitMQ.Client;
using Refit;
using StackExchange.Redis;
using Xiangjiandao.Web.Clients;
using Xiangjiandao.Web.Middlewares;
using Xiangjiandao.Web.Options;
using Xiangjiandao.Web.Services;
using Xiangjiandao.Web.Shared;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRabbitMQ(this IServiceCollection services,
        IConfigurationSection configurationSection)
    {
        RabbitMQOptions options = new();
        configurationSection.Bind(options);
        services.AddSingleton<IConnectionFactory>(new ConnectionFactory()
        {
            HostName = options.HostName,
            Port = options.Port,
            VirtualHost = options.VirtualHost,
            UserName = options.Username,
            Password = options.Password,
            ClientProvidedName = "RivTower.RivDap.Web".ToLower(),
            DispatchConsumersAsync = true
        });

        return services;
    }

    public static IServiceCollection AddClients(this IServiceCollection services, AliYunSmsOptions aliYunSmsOptions,
        AliYunModerationOptions aliYunModerationOptions, BlueSkyOptions blueSkyOptions)
    {
        var jsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        jsonSerializerSettings.Converters.Add(new NewtonsoftEntityIdJsonConverter());

        var ser = new NewtonsoftJsonContentSerializer(jsonSerializerSettings);

#pragma warning disable S1481
        var settings = new RefitSettings(ser);
        services.AddSingleton<AlibabaCloud.SDK.Dysmsapi20170525.Client, AlibabaCloud.SDK.Dysmsapi20170525.Client>(_ =>
        {
            var config = new AlibabaCloud.OpenApiClient.Models.Config
            {
                AccessKeyId = aliYunSmsOptions.AccessKeyId,
                AccessKeySecret = aliYunSmsOptions.AccessKeySecret,
                Endpoint = aliYunSmsOptions.EndPoint
            };
            return new AlibabaCloud.SDK.Dysmsapi20170525.Client(config);
        });
        services.AddSingleton<AlibabaCloud.SDK.Green20220302.Client, AlibabaCloud.SDK.Green20220302.Client>(_ =>
        {
            var config = new AlibabaCloud.OpenApiClient.Models.Config
            {
                AccessKeyId = aliYunModerationOptions.AccessKeyId,
                AccessKeySecret = aliYunModerationOptions.AccessKeySecret,
                RegionId = aliYunModerationOptions.RegionId,
                Endpoint = aliYunModerationOptions.Endpoint,
                ReadTimeout = 6000,
                ConnectTimeout = 3000,
            };
            return new AlibabaCloud.SDK.Green20220302.Client(config);
        });
#pragma warning restore S1481
        services.AddSingleton<IAliYunSmsClient, AliYunSmsClient>();
        services.AddSingleton<IModerationClient, ModerationClient>();
        services.AddRefitClient<IBlueSkyClient>(_ => settings)
            .ConfigureHttpClient(client => client.BaseAddress = new Uri(blueSkyOptions.PdsDomain));
        services.AddRefitClient<IPostClient>(_ => settings)
            .ConfigureHttpClient(client => client.BaseAddress = new Uri(blueSkyOptions.PostDomain));
        return services;
    }


    public static IHttpClientBuilder AddServiceSelector(this IHttpClientBuilder builder, string serviceName)
    {
        return builder.ConfigureHttpClient((serviceProvider, httpClient) =>
        {
            var serviceSelector = serviceProvider.GetRequiredService<IServiceSelector>();
            var service = serviceSelector.Find(serviceName);
            ArgumentNullException.ThrowIfNull(service);
            httpClient.BaseAddress = new Uri(service.Address);
        });
    }

    public static IServiceCollection AddAllContext(this IServiceCollection services)
    {
        services.AddContext().AddEnvContext(envContextKey: "env").AddCapContextProcessor();
        return services;
    }


    public static IServiceCollection AddDistributedDisLock(this IServiceCollection services)
    {
        services.AddSingleton<IXiangjiandaoDistributedDisLock>(p =>
            new RedisLock(p.GetRequiredService<IConnectionMultiplexer>().GetDatabase()));

        return services;
    }


    /// <summary>
    /// 注册命名空间RivTower.RivDap.Web.Application.Queries下面的所有Query服务
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddAllQueryService(this IServiceCollection services)
    {
        typeof(Program).Assembly.GetTypes()
            .Where(p => p.Namespace == "Xiangjiandao.Web.Application.Queries" && p.IsClass && !p.IsAbstract).ToList()
            .ForEach(p => { services.AddScoped(p); });
        return services;
    }

    /// <summary>
    /// 注册 LoginUser
    /// </summary>
    public static IServiceCollection AddLoginUser(this IServiceCollection services)
    {
        services.AddSingleton<ILoginUserService, LoginUserService>();
        services.AddScoped<ILoginUser>(p =>
        {
            var httpContextAccessor = p.GetRequiredService<IHttpContextAccessor>();
            if (httpContextAccessor.HttpContext is null)
            {
                return new LoginUser(Guid.Empty, string.Empty, string.Empty, false, string.Empty);
            }

            var user = httpContextAccessor.HttpContext?.Items[LoginUserMiddleware.LoginUserKey];
            if (user is not ILoginUser loginUser)
            {
                throw new KnownException("无效的登录信息");
            }

            return loginUser;
        });
        return services;
    }

    /// <summary>
    /// 将 Excel 导出器注入到容器中
    /// </summary>
    public static IServiceCollection AddExcelExporter(this IServiceCollection services)
    {
        services.AddSingleton<IExcelExporter, ExcelExporter>();
        return services;
    }
}