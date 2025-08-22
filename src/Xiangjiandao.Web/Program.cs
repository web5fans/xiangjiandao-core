using System.Globalization;
using System.Reflection;
using System.Text.Json;
using AspNet.Security.OAuth.Feishu;
using Exceptionless;
using FastEndpoints;
using FastEndpoints.Swagger;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.Redis.StackExchange;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using NetCorePal.Context;
using NetCorePal.Extensions.DistributedLocks;
using NetCorePal.Extensions.Domain.Json;
using NetCorePal.Extensions.MultiEnv;
using NetCorePal.Extensions.NewtonsoftJson;
using NetCorePal.Extensions.Primitives;
using NetCorePal.Extensions.ServiceDiscovery;
using NetCorePal.Extensions.Snowflake;
using NetCorePal.SkyApm.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Polly.Caching;
using Polly.Caching.Memory;
using Prometheus;
using Refit;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using SkyApm.AspNetCore.Diagnostics;
using SkyApm.Tracing;
using StackExchange.Redis;
using Xiangjiandao.Web.Clients;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Logging;
using Xiangjiandao.Web.Middlewares;
using Xiangjiandao.Web.Options;
using Xiangjiandao.Web.Shared;
using Xiangjiandao.Web.Utils;
using Yarp.ReverseProxy.Forwarder;

IdentityModelEventSource.ShowPII = true;
Log.Logger = new LoggerConfiguration()
    .Enrich.WithClientIp()
    .Enrich.With(new TraceIdEnricher())
    .MinimumLevel.Override("Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware", LogEventLevel.Fatal)
    .WriteTo.Console(new CompactJsonFormatter())
    .CreateBootstrapLogger();
try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.WebHost.ConfigureKestrel(options => { options.AddServerHeader = false; });

    builder.Configuration.AddJsonFile("/app/appconfig.json", optional: true);


    #region config

    builder.Services.Configure<AppOptions>(builder.Configuration.GetSection("App"));
    var appOptions = new AppOptions();
    builder.Configuration.GetSection("App").Bind(appOptions);

    var jwtConfig = new JwtConfig();
    builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("Jwt"));
    builder.Configuration.GetSection("Jwt").Bind(jwtConfig);
    builder.Services.AddSingleton<JwtGenerator>();

    builder.Services.Configure<EmailOption>(builder.Configuration.GetSection("Email"));
    builder.Services.AddSingleton<EmailSender>();
    builder.Services.AddSingleton<IEmailVerifyCodeUtils, EmailVerifyCodeUtils>();

    builder.Services.Configure<TemplateOption>(builder.Configuration.GetSection("Template"));
    var templateOption = new TemplateOption();
    builder.Configuration.GetSection("Template").Bind(templateOption);

    builder.Services.AddSingleton<IAliYunSMSVerifyCodeUtils, AliYunSmsVerifyCodeUtils>();


    if (appOptions.UseSkyAPM)
    {
        builder.Services.AddSkyAPM(ext => ext.AddAspNetCoreHosting()
            .AddNetCorePal(options =>
            {
                options.WriteCommandData = appOptions.SkyAPMWriteData;
                options.WriteDomainEventData = appOptions.SkyAPMWriteData;
                options.WriteIntegrationEventData = appOptions.SkyAPMWriteData;
                options.JsonSerializerOptions.Converters.Add(new EntityIdJsonConverterFactory());
            }));
    }

    RedisOptions redisOptions = new();

    builder.Configuration.GetSection("Redis").Bind(redisOptions);
    var co = new ConfigurationOptions();
    co.EndPoints.Add(redisOptions.Host, redisOptions.Port);
    co.DefaultDatabase = redisOptions.Database;
    co.Password = redisOptions.Password;
    var redis = ConnectionMultiplexer.Connect(co);
    builder.Services.AddSingleton<IConnectionMultiplexer>(_ => redis);

    var envOption = new EnvOptions();
    builder.Configuration.GetSection("Env").Bind(envOption);
    var displayEnv = string.IsNullOrEmpty(envOption.ServiceEnv) ? "main" : envOption.ServiceEnv;

    var aliYunSmsOptions = new AliYunSmsOptions();
    builder.Services.Configure<AliYunSmsOptions>(builder.Configuration.GetSection("AliYunSms"));
    builder.Configuration.GetSection("AliYunSms").Bind(aliYunSmsOptions);

    var aliYunModerationOptions = new AliYunModerationOptions();
    builder.Services.Configure<AliYunModerationOptions>(builder.Configuration.GetSection("AliYunModeration"));
    builder.Configuration.GetSection("AliYunModeration").Bind(aliYunModerationOptions);

    var blueSkyOptions = new BlueSkyOptions();
    builder.Services.Configure<BlueSkyOptions>(builder.Configuration.GetSection("BlueSky"));
    builder.Configuration.GetSection("BlueSky").Bind(blueSkyOptions);

    var exceptionlessOptions = new ExceptionlessOptions();
    builder.Services.Configure<ExceptionlessOptions>(builder.Configuration.GetSection("Exceptionless"));
    builder.Configuration.GetSection("Exceptionless").Bind(exceptionlessOptions);

    #endregion

    ILoggerFactory factory;
    if (!builder.Environment.IsDevelopment())
    {
        builder.Logging.ClearProviders();
        if (exceptionlessOptions.Enable)
        {
            builder.Logging.AddExceptionless();

            factory = LoggerFactory.Create(builder => builder.AddExceptionless());
        }
        else
        {
            builder.Host.UseSerilog((context, services, configuration) => configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .Enrich.FromLogContext()
                    .Enrich.With(new TraceIdEnricher())
                    .Enrich.WithClientIp()
                    .MinimumLevel.Override(
                        source: "Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware",
                        LogEventLevel.Fatal
                    )
                    .WriteTo.Console(new CompactJsonFormatter()),
                writeToProviders: true);

            factory = LoggerFactory.Create(b => b.AddSerilog());
        }
    }
    else
    {
        factory = LoggerFactory.Create(b =>
        {
            b.AddConsole();
            b.AddConfiguration(builder.Configuration.GetSection("Logging"));
        });
    }


    #region 国际化

    builder.Services.AddLocalization();
    builder.Services.Configure<RequestLocalizationOptions>(options =>
    {
        var supportedCultures = new[]
        {
            new CultureInfo("zh"),
            new CultureInfo("en")
        };
        options.CultureInfoUseUserOverride = false;
        options.ApplyCurrentCultureToResponseHeaders = false;
        // State what the default culture for your application is. This will be used if no specific culture
        // can be determined for a given request.
        options.DefaultRequestCulture = new RequestCulture(culture: "zh", uiCulture: "zh");
        var culture = new CultureInfo("zh");
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;

        // You must explicitly state which cultures your application supports.
        // These are the cultures the app supports for formatting numbers, dates, etc.
        options.SupportedCultures = supportedCultures;

        // These are the cultures the app supports for UI strings, i.e. we have localized resources for.
        options.SupportedUICultures = supportedCultures;
    });

    #endregion

    #region SignalR

    builder.Services.AddHealthChecks();
    builder.Services.AddMvc()
        .AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.Converters.Add(new NewtonsoftEntityIdJsonConverter());
        });
    builder.Services.AddSignalR();

    #endregion

    #region Prometheus监控

    builder.Services.AddHealthChecks().ForwardToPrometheus();
    builder.Services.AddHttpClient(Options.DefaultName)
        .UseHttpClientMetrics();

    #endregion

    // Add services to the container.


    #region 身份认证

    builder.Services.AddDataProtection()
        .PersistKeysToDbContext<ApplicationDbContext>();
    JsonWebKeysOptions certsOptions = new();
    builder.Configuration.Bind(certsOptions);
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddCookie()
        .AddJwtBearer(jwtBearerOptions =>
        {
            jwtBearerOptions.MapInboundClaims = false;
            jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                TryAllIssuerSigningKeys = true,
                ValidAudience = jwtConfig.Audience,
                ValidIssuer = jwtConfig.Issuer,
            };
            jwtBearerOptions.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    // If the request is for our hub...
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) &&
                        (path.StartsWithSegments("/hubs/notify")
                         ||
                         path.StartsWithSegments("/mapteam")))
                    {
                        // Read the token out of the query string
                        context.Token = accessToken;
                    }

                    return Task.CompletedTask;
                }
            };
        }).AddFeishu(options =>
        {
            options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.ClientId = builder.Configuration.GetValue<string>("FeiShu:ClientId") ?? string.Empty;
            options.ClientSecret = builder.Configuration.GetValue<string>("FeiShu:ClientSecret") ?? string.Empty;
            options.CallbackPath = builder.Configuration.GetValue<string>("FeiShu:CallbackPath", "/api/auth/callback");
            options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
            options.CorrelationCookie.SameSite = SameSiteMode.Lax;
        });
    // 添加Redis连接
    builder.Services.AddNetCorePalJwt().AddRedisStore(); // 使用Redis存储密钥

    builder.Services.AddAuthorizationBuilder()
        .AddPolicy(PolicyNames.Client, policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim("type", TokenType.Client);
            policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
        })
        .AddPolicy(PolicyNames.Admin, policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim("type", TokenType.Admin);
            policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
        }).AddPolicy(PolicyNames.AdminOnly, policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim("type", TokenType.Admin);
            policy.RequireRole("admin");
            policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
        });


    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy(FeishuAuthenticationDefaults.AuthenticationScheme.ToLower(), policy =>
        {
            policy.AuthenticationSchemes.Clear();
            policy.AuthenticationSchemes.Add(FeishuAuthenticationDefaults.AuthenticationScheme);
            policy.RequireAuthenticatedUser();
        });
    });

    #endregion

    #region Fast Endpoints

    builder.Services.AddFastEndpoints();
    builder.Services.Configure<JsonOptions>(o =>
        o.SerializerOptions.AddNetCorePalJsonConverters());
    //缓存
    builder.Services.AddResponseCaching();
    builder.Services.SwaggerDocument(o =>
    {
        o.DocumentSettings = s =>
        {
            s.Version = "v1"; //must match what's being passed in to the map method below
            s.Title = $"{appOptions.Name} [env={displayEnv}]";
        };
        //自动Tag路径
        o.AutoTagPathSegmentIndex = 0;
    });
    // builder.Services.AddSwaggerGenNewtonsoftSupport();

    #endregion

    #region Controller

    #endregion


    #region HybridCache 混合缓存

    builder.Services.AddHybridCache(option =>
    {
        option.MaximumPayloadBytes = 1024 * 1024;
        option.MaximumKeyLength = 1024;
        option.DefaultEntryOptions = new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromMinutes(5),
            LocalCacheExpiration = TimeSpan.FromMinutes(5)
        };
    });

    #endregion

    #region 公共服务

    builder.Services.AddSingleton<IClock, SystemClock>();

    builder.Services.AddHttpContextAccessor();
    builder.Services.AddRequestCancellationToken();
    builder.Services.AddLoginUser();
    builder.Services.AddExcelExporter();
    builder.Services.AddDistributedDisLock();
    if (!builder.Environment.IsDevelopment())
    {
        //ADD background service 
        // builder.Services.AddHostedService<TaskSendTxJob>();
    }

    #endregion

    #region 模型验证器

    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    builder.Services.AddKnownExceptionErrorModelInterceptor();

    #endregion


    #region Mapper Provider

    builder.Services.AddMapperPrivider(Assembly.GetExecutingAssembly());

    #endregion

    #region Query

    builder.Services.AddAllQueryService();

    #endregion

    #region RabbitMQ

    builder.Services.AddRabbitMQ(builder.Configuration.GetSection("RabbitMQ"));

    #endregion


    #region 服务注册发现

    builder.Services.AddAllContext();
    builder.Services.AddNetCorePalServiceDiscoveryClient();
    builder.Services.Configure<EnvOptions>(builder.Configuration.GetSection("Env"));
    if (builder.Environment.IsDevelopment())
    {
        builder.Services.AddSingleton<IServiceSelector, DevServiceSelector>();
    }
    else
    {
        if (appOptions.UseK8sDiscovery)
        {
            builder.Services.AddK8SServiceDiscovery(p =>
            {
                p.LabelKeyOfServiceName = "app";
                p.ServiceNamespace = appOptions.ServiceNamespace;
            });
        }
        else
        {
            builder.Services.AddSingleton<IServiceSelector, DevServiceSelector>();
        }
    }

    #endregion


    #region 反向代理

    builder.Services.Replace(
        ServiceDescriptor.Singleton<IForwarderHttpClientFactory, CustomForwarderHttpClientFactory>());
    builder.Services.AddReverseProxy()
        .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

    #endregion


    #region 基础设施

    builder.Services.AddRedisLocks();

    builder.Services.AddCommandLocks(typeof(Program).Assembly);

    builder.Services.AddRepositories(typeof(ApplicationDbContext).Assembly);

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseMySql(builder.Configuration.GetConnectionString("MySql"),
                new MySqlServerVersion(new Version(8, 0, 34)),
                b =>
                {
                    b.MigrationsAssembly(typeof(Program).Assembly.FullName);
                    b.UseNewtonsoftJson();
                })
            .UseLoggerFactory(factory)
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors();
    });
    builder.Services.AddUnitOfWork<ApplicationDbContext>();
    builder.Services.AddIntegrationEvents(typeof(Program)).UseCap<ApplicationDbContext>(b =>
    {
        b.RegisterServicesFromAssemblies(typeof(Program));
        b.AddContextIntegrationFilters();
        // b.UseMySql();
    });

    //配置多环境Options
    builder.Services.Configure<EnvOptions>(envOptions => builder.Configuration.GetSection("Env").Bind(envOptions));
    builder.Services.AddContext().AddEnvContext().AddCapContextProcessor();
    builder.Services.AddSingleton<IContextCarrierHandler, CultureContextCarrierHandler>();
    builder.Services.AddSingleton<IContextSourceHandler, CultureContextSourceHandler>();
    var rabbitMqOptions = new RabbitMQOptions();
    var rabbitmpSection = builder.Configuration.GetSection("RabbitMQ");
    rabbitmpSection.Bind(rabbitMqOptions);
    builder.Services.AddCap(x =>
    {
        x.TopicNamePrefix = "xiangjiandao";
        x.DefaultGroupName = appOptions.Name;
        x.Version = displayEnv;
        x.FailedRetryCount = 3;
        x.UseNetCorePalStorage<ApplicationDbContext>();
        x.UseRabbitMQ(p =>
        {
            p.HostName = rabbitMqOptions.HostName;
            p.UserName = rabbitMqOptions.Username;
            p.Password = rabbitMqOptions.Password;
            p.Port = rabbitMqOptions.Port;
            p.VirtualHost = rabbitMqOptions.VirtualHost;
            p.ExchangeName = "xiangjiandao";
        });
        x.UseDashboard(options =>
        {
            options.PathMatch = $"/{appOptions.DashBoardPathPrefix}/cap";
        }); //CAP Dashboard  path：  /cap
    });
    builder.Services.AddEnvFixedConnectionChannelPool();

    #endregion

    builder.Services.AddMediatR(cfg =>
        cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly())
            .AddCommandLockBehavior()
            .AddKnownExceptionValidationBehavior()
            .AddUnitOfWorkBehaviors());

    #region 多环境支持与服务注册发现

    builder.Services.AddMultiEnv(envOption => envOption.ServiceName = "Abc.Template")
        .UseMicrosoftServiceDiscovery();
    builder.Services.AddConfigurationServiceEndpointProvider();
    builder.Services.AddEnvFixedConnectionChannelPool();

    builder.Services.AddClients(aliYunSmsOptions, aliYunModerationOptions, blueSkyOptions);

    #endregion


    #region Polly

    builder.Services.AddMemoryCache();
    builder.Services.AddSingleton<IAsyncCacheProvider, MemoryCacheProvider>();

    #endregion

    #region 远程服务客户端配置

    var jsonSerializerSettings = new JsonSerializerSettings
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        NullValueHandling = NullValueHandling.Ignore,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
    };
    jsonSerializerSettings.AddNetCorePalJsonConverters();
    var ser = new NewtonsoftJsonContentSerializer(jsonSerializerSettings);
    var settings = new RefitSettings(ser);

    #endregion

    builder.Services.AddSingleton<ISnowflakeIdGenerator>(p => new WarpSnowflakeIdGenerator());


    #region Jobs

    builder.Services.AddJobs();
    builder.Services.AddHangfire(x =>
    {
        var hangfireRedis = builder.Configuration.GetConnectionString("hangfireRedis");
        x.UseFilter(new AutomaticRetryAttribute { Attempts = 1 });
        x.UseRedisStorage(!string.IsNullOrEmpty(hangfireRedis) ? ConnectionMultiplexer.Connect(hangfireRedis) : redis,
            new RedisStorageOptions
            {
                Prefix = string.IsNullOrEmpty(envOption.ServiceEnv)
                    ? "hangfire:Xiangjiandao:"
                    : $"hangfire:Xiangjiandao-{envOption.ServiceEnv}:",
                Db = !string.IsNullOrEmpty(hangfireRedis) ? 0 : redisOptions.Database
            });
    });
    builder.Services.AddHangfireServer(option =>
        option.SchedulePollingInterval = TimeSpan.FromSeconds(1)); //hangfire dashboard  path：  /hangfire

    #endregion


    #region Forward

    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.All;
        options.ForwardLimit = 2;
        options.KnownNetworks.Clear();
        options.KnownProxies.Clear();
    });

    #endregion

    #region 异常日志收集

    if (exceptionlessOptions.Enable)
    {
        builder.Services.AddExceptionless(config =>
        {
            config.ApiKey = exceptionlessOptions.ApiKey;
            config.ServerUrl = exceptionlessOptions.ServerUrl;
        });
    }

    #endregion


    var app = builder.Build();
    if (app.Environment.IsDevelopment())
    {
        using var l = app.Services.GetRequiredService<IDistributedLock>()
            .Acquire("init-db", TimeSpan.FromSeconds(10));
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        // dbContext.Database.EnsureCreated();
    }

    if (appOptions.UseSkyAPM)
    {
        //初始化TraceIdEnricher
        TraceIdEnricher.SetEntrySegmentContextAccessor(
            app.Services.GetRequiredService<IEntrySegmentContextAccessor>());
    }

    app.UseForwardedHeaders();

    #region 国际化

    var locOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
    if (locOptions != default!)
    {
        app.UseRequestLocalization(locOptions.Value);
    }

    #endregion

    app.UseContext();
    app.Use(async (context, next) =>
    {
        var contextAccessor = context.RequestServices.GetRequiredService<IContextAccessor>();
        contextAccessor.SetContext(new CultureContext(Thread.CurrentThread.CurrentCulture.Name));
        await next(context);
    });
    app.UseAuthentication();

    # region Middleware

    if (appOptions.UseFeiShu)
    {
        app.UseMiddleware<FeishuMiddleware>();
    }


    app.UseMiddleware<LoginUserMiddleware>();
    app.UseKnownExceptionHandler();
    app.UseMiddleware<UserDisableCheckMiddleware>();
    app.UseMiddleware<PostTakeOffCheckMiddleware>();

    if (aliYunModerationOptions.EnableTextModeration)
    {
        app.UseMiddleware<TextModerationMiddleware>();
    }

    if (aliYunModerationOptions.EnableImageModeration)
    {
        app.UseMiddleware<ImageUploadModerationMiddleware>();
    }

    app.UseMiddleware<NodeUserCheckMiddleware>();

    # endregion

    app.UseStaticFiles();
    app.UseHttpsRedirection();
    app.UseRouting();
    app.UseAuthorization();

    app.UseResponseCaching()
        .UseFastEndpoints()
        .UseSwaggerGen(
            config: c =>
            {
                c.Path = $"/{appOptions.DashBoardPathPrefix}/swagger/{{documentName}}/swagger.{{json|yaml}}";
            }, uiConfig: u =>
            {
                u.Path = $"/{appOptions.DashBoardPathPrefix}/swagger";
                u.DocumentPath =
                    $"/{appOptions.DashBoardPathPrefix}/swagger/{{documentName}}/swagger.{{json|yaml}}";
            });
    /*if (appOptions.UseSwagger)
    {
        app.UseSwagger(options =>
        {
            options.RouteTemplate =
                appOptions.DashBoardPathPrefix + "/swagger/{documentName}/swagger.{json|yaml}";
        });
        app.UseSwaggerUI(options => { options.RoutePrefix = appOptions.DashBoardPathPrefix + "/swagger"; });
    }*/
    // app.MapControllers();


    #region SignalR

    // app.MapHub<Xiangjiandao.Web.Application.Hubs.ChatHub>("/chat");

    #endregion


    #region 异常日志收集

    if (exceptionlessOptions.Enable)
    {
        app.UseExceptionless();
    }

    #endregion

    app.UseHttpMetrics();
    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        Predicate = healthCheck => healthCheck.Tags.Contains("base")
    });
    if (!app.Environment.IsDevelopment())
    {
        app.MapHealthChecks("/devops/health-all", new HealthCheckOptions
        {
            Predicate = healthCheck => healthCheck.Tags.Contains("all")
        }).RequireAuthorization("feishu");
    }

    app.MapMetrics("/metrics"); // 通过   /metrics  访问指标
    app.UseHangfireDashboard($"/{appOptions.DashBoardPathPrefix}/hangfire", new DashboardOptions()
    {
        Authorization = new[] { new HangfireNoAuthorizationFilter() }
    });
    if (app.Configuration.GetValue("RegisterJobs", true))
    {
        app.RegisterJobs();
    }

    app.MapReverseProxy();

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}

#pragma warning disable S1118
public partial class Program
#pragma warning restore S1118
{
}