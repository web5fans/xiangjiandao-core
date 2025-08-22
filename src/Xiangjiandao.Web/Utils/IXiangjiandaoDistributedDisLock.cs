using Medallion.Threading;
using Medallion.Threading.Redis;
using StackExchange.Redis;

namespace Xiangjiandao.Web.Utils;

public interface IXiangjiandaoDistributedDisLock
{
    ILockSynchronizationHandle? TryAcquire(
        string key,
        TimeSpan timeout = default(TimeSpan),
        CancellationToken cancellationToken = default(CancellationToken));

    ILockSynchronizationHandle Acquire(
        string key,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default(CancellationToken));

    ValueTask<ILockSynchronizationHandle?> TryAcquireAsync(string key,
        TimeSpan timeout = default(TimeSpan),
        CancellationToken cancellationToken = default(CancellationToken));

    ValueTask<ILockSynchronizationHandle> AcquireAsync(string key,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default(CancellationToken));
}

public class RedisLock(IDatabase database) : IXiangjiandaoDistributedDisLock
{
    public ILockSynchronizationHandle? TryAcquire(string key, TimeSpan timeout = default(TimeSpan),
        CancellationToken cancellationToken = default(CancellationToken))
    {
        var rd = new RedisDistributedLock(key, database);
        var handle = rd.TryAcquire(timeout, cancellationToken);
        return handle == null ? null : new RedisLockSynchronizationHandle(handle);
    }

    public ILockSynchronizationHandle Acquire(string key, TimeSpan? timeout = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        var rd = new RedisDistributedLock(key, database);
        var handle = rd.Acquire(timeout, cancellationToken);
        return new RedisLockSynchronizationHandle(handle);
    }

    public async ValueTask<ILockSynchronizationHandle?> TryAcquireAsync(string key,
        TimeSpan timeout = default(TimeSpan),
        CancellationToken cancellationToken = default(CancellationToken))
    {
        var rd = new RedisDistributedLock(key, database);
        var handle = await rd.TryAcquireAsync(timeout, cancellationToken);
        return handle == null ? null : new RedisLockSynchronizationHandle(handle);
    }

    public async ValueTask<ILockSynchronizationHandle> AcquireAsync(string key, TimeSpan? timeout = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        var rd = new RedisDistributedLock(key, database);
        var handle = await rd.AcquireAsync(timeout, cancellationToken);
        return new RedisLockSynchronizationHandle(handle);
    }
}

public interface ILockSynchronizationHandle : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Gets a <see cref="T:System.Threading.CancellationToken" /> instance which may be used to
    /// monitor whether the handle to the lock is lost before the handle is
    /// disposed.
    /// 
    /// For example, this could happen if the lock is backed by a
    /// database and the connection to the database is disrupted.
    /// 
    /// Not all lock types support this; those that don't will return <see cref="P:System.Threading.CancellationToken.None" />
    /// which can be detected by checking <see cref="P:System.Threading.CancellationToken.CanBeCanceled" />.
    /// 
    /// For lock types that do support this, accessing this property may incur additional
    /// costs, such as polling to detect connectivity loss.
    /// </summary>
    CancellationToken HandleLostToken { get; }
}

#pragma warning disable S3881
public class RedisLockSynchronizationHandle(IDistributedSynchronizationHandle handle) : ILockSynchronizationHandle
#pragma warning restore S3881
{
    public CancellationToken HandleLostToken => handle.HandleLostToken;

    public void Dispose()
    {
        handle.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        return handle.DisposeAsync();
    }
}

/// <summary>
/// 
/// </summary>
/// <param name="handles"></param>
#pragma warning disable S3881
public class RedisLockSynchronizationHandleList(IList<ILockSynchronizationHandle> handles):ILockSynchronizationHandle
#pragma warning restore S3881
{

    /// <summary>
    /// 
    /// </summary>
    public void Dispose()
    {
        foreach (var handle in handles)
        {
            handle.Dispose();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        foreach (var handle in handles)
        {
            await handle.DisposeAsync();
        }
    }

    /// <summary>
    /// 根据_handlers 集合的HandleLostToken属性集合，生成一个新的CancellationToken
    /// </summary>
    public CancellationToken HandleLostToken => CancellationTokenSource
        .CreateLinkedTokenSource(handles.Select(t => t.HandleLostToken).ToArray()).Token;
}


public class RedisListLock(IXiangjiandaoDistributedDisLock xiangjiandaoDistributedDisLock)
{
     public async ValueTask<ILockSynchronizationHandle> Acquire(
        IEnumerable<string> keys,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default(CancellationToken))
     {
         IList<ILockSynchronizationHandle> handles = new List<ILockSynchronizationHandle>();
         try
         {
             foreach (var key in keys.OrderBy(p => p))
             {
                 handles.Add(
                     await xiangjiandaoDistributedDisLock.AcquireAsync(key, TimeSpan.FromSeconds(30),
                         cancellationToken)
                     
                     );
             }

             return new RedisLockSynchronizationHandleList(handles);

         }
         catch
         {
             foreach (var handle in handles)
             {
                 await handle.DisposeAsync();
             }

             throw;
         }
         
     }
}

