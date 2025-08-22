using Microsoft.EntityFrameworkCore;
using Testcontainers.MySql;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Infrastructure;
using Xiangjiandao.Web.Endpoints.UserMedal;

namespace Xiangjiandao.Web.Tests;

public class UserMedalQueryTests
{
    private MySqlContainer MySqlContainer { get; } = new MySqlBuilder()
        .WithImage("mysql:8.0.35")
        .WithUsername("root").WithPassword("123456")
        .WithEnvironment("TZ", "Asia/Shanghai")
        .WithDatabase("demo").Build();
    private readonly IServiceCollection _services;
    
    public UserMedalQueryTests()
    {
        MySqlContainer.StartAsync().GetAwaiter().GetResult();

        _services = new ServiceCollection();
        _services.AddMediatR(c => c.RegisterServicesFromAssemblies(typeof(UserMedalQueryTests).Assembly));
        _services.AddLogging();
        _services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySql(
                connectionString: MySqlContainer.GetConnectionString(),
                serverVersion: new MySqlServerVersion(new Version(8, 0, 35)),
                mySqlOptionsAction: builder =>
                {
                    builder.MigrationsAssembly(typeof(Program).Assembly.FullName);
                    builder.UseNewtonsoftJson();
                })
        );
    }
    
    [Fact]
    public void GetQueryStringTest()
    {
        var provider = _services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.EnsureCreated();

        var userId = new UserId(Guid.NewGuid());
        var query = from medal in dbContext.Medals
            select new UserMedalPageVo
            {
                MedalId = medal.Id,
                AttachId = medal.AttachId,
                Name = medal.Name,
                GetTime = dbContext.UserMedals
                    .Where(um => um.UserId == userId && um.MedalId == medal.Id)
                    .OrderByDescending(um => um.GetTime)
                    .Select(um => (DateTimeOffset?)um.GetTime)
                    .FirstOrDefault()
            };
        // 如果需要按 GetTime 排序后再分页
        var orderedQuery = query
            .OrderByDescending(x => x.GetTime)
            .ThenBy(x => x.MedalId); // 加一个稳定排序字段
        Console.WriteLine(orderedQuery.ToQueryString());
        
    }
}