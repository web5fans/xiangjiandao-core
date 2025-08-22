using Testcontainers.MySql;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;

namespace Xiangjiandao.Web.Tests.Extensions;

public class TestContainerFixture : IAsyncLifetime
{
    public RedisContainer RedisContainer { get; } =
        new RedisBuilder()
            .WithCommand("--databases","1024")
            .Build();

    public RabbitMqContainer RabbitMqContainer { get; } = new RabbitMqBuilder()
        .WithPrivileged(true)
        .WithUsername("guest").WithPassword("guest").Build();

    public MySqlContainer MySqlContainer { get; } = new MySqlBuilder()
        .WithImage("mysql:8.0.35")
        .WithUsername("root").WithPassword("123456")
        .WithEnvironment("TZ", "Asia/Shanghai")
        .WithDatabase("demo").Build();

    public Task InitializeAsync()
    {
        return Task.WhenAll(RedisContainer.StartAsync(),
            RabbitMqContainer.StartAsync(),
            MySqlContainer.StartAsync());
    }

    public Task DisposeAsync()
    {
        return Task.WhenAll(RedisContainer.StopAsync(),
            RabbitMqContainer.StopAsync(),
            MySqlContainer.StopAsync());
    }
}