using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Xiangjiandao.Web.Tests
{
    public abstract class MyWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private static readonly Extensions.TestContainerFixture _containers;

        private static bool _isInitialized = false;
        
        private static int index = 0;
        
        private readonly int i;

        static MyWebApplicationFactory()
        {
            _containers = new Extensions.TestContainerFixture();
        }

        public MyWebApplicationFactory()
        {
            lock (_containers)
            {
                i = index++;
            }
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            //builder.UseSetting("ConnectionStrings:PostgreSQL", postgreSqlContainer.GetConnectionString());
            builder.UseSetting("Redis:Port", _containers.RedisContainer.GetMappedPublicPort(6379).ToString());
            builder.UseSetting("Redis:Host", _containers.RedisContainer.Hostname);
            builder.UseSetting("Redis:Database", i.ToString()); 
            builder.UseSetting("ConnectionStrings:MySql",
                _containers.MySqlContainer.GetConnectionString()
                    .Replace("demo0", $"demo{i}") +
                ";ConnectionIdleTimeout=5;Pooling=true;"); //+";ConnectionIdleTimeout=5;Pooling=false;");
            builder.UseSetting("RabbitMQ:Port", _containers.RabbitMqContainer.GetMappedPublicPort(5672).ToString());
            builder.UseSetting("RabbitMQ:UserName", "guest");
            builder.UseSetting("RabbitMQ:Password", "guest");
            builder.UseSetting("RabbitMQ:VirtualHost", "/");
            builder.UseSetting("RabbitMQ:HostName", _containers.RabbitMqContainer.Hostname);
            
            builder.UseSetting("RegisterJobs", "false");
            builder.UseEnvironment("Development");
            
            SetupMockService(builder);
            base.ConfigureWebHost(builder);
        }
        public async Task InitializeAsync()
        {
            AsyncLock asyncLock = new AsyncLock();
            using (await asyncLock.LockAsync())
            {
                if (!_isInitialized)
                {
                    await _containers.InitializeAsync();
                    _isInitialized = true;
                }
            }

            await _containers.RabbitMqContainer.ExecAsync(new string[] { "rabbitmqctl", "add_vhost", $"v{i}" });
            await _containers.RabbitMqContainer.ExecAsync(new string[]
                { "rabbitmqctl", "set_permissions", "-p", $"v{i}", "guest", ".*", ".*", ".*" });
            await InitializeServiceAsync();
        }

        public new async  Task DisposeAsync()
        {
            await base.DisposeAsync();
        }
        
        protected abstract void SetupMockService(IWebHostBuilder builder);
        
        protected abstract Task InitializeServiceAsync();

    }
    
    public class AsyncLock
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public async Task<IDisposable> LockAsync()
        {
            await _semaphore.WaitAsync();
            return new Releaser(_semaphore);
        }

        private class Releaser : IDisposable
        {
            private readonly SemaphoreSlim _semaphore;

            public Releaser(SemaphoreSlim semaphore)
            {
                _semaphore = semaphore;
            }

            public void Dispose()
            {
                _semaphore.Release();
            }
        }
    }
}