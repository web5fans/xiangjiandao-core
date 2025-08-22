using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.ScoreDistributeRecordAggregate;
using Xiangjiandao.Infrastructure.Repositories;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Application.Commands;

/// <summary>
/// 批量发放稻米
/// </summary>
public record BatchScoreDistributionCommand : ICommand<bool>
{
    /// <summary>
    /// 发放稻米
    /// </summary> 
    public long Score { get; set; }
    
    /// <summary>
    /// 用户手机号或邮箱
    /// </summary>
    public List<string> UserPhoneOrEmails { get; set; } = new List<string>();
    
    /// <summary>
    /// 管理员手机号
    /// </summary>
    public string AdminPhone { get; set; } = string.Empty;
}

/// <summary>
/// 批量发放稻米
/// </summary>
public class BatchScoreDistributionCommandHandler(
    IScoreDistributeRecordRepository scoreDistributeRecordRepository,
    IUserRepository userRepository,
    IXiangjiandaoDistributedDisLock distributedLock,
    ILogger<BatchScoreDistributionCommandHandler> logger) : ICommandHandler<BatchScoreDistributionCommand, bool>
{
    public async Task<bool> Handle(BatchScoreDistributionCommand command, CancellationToken cancellationToken)
    { 
        logger.LogInformation("批量发放稻米");
        var users = await userRepository.GetByPhoneOrEmailAsync(command.UserPhoneOrEmails, cancellationToken);
        if (users.Count == 0)
        {
            throw new KnownException("用户不存在");
        }
        
        if (users.Count != command.UserPhoneOrEmails.Count)
        { 
            throw new KnownException("提交的手机号或邮箱不全是平台用户或是同一个用户");
        }
        
        // 获取用户锁
        var userLocks = await AcquireUserLocks(users.Select(u => u.Id.Id.ToString()), cancellationToken);
        try
        {
            var scoreDistributeRecords = users.Select(user => ScoreDistributeRecord.Create(
                user.Id,
                user.NickName,
                user.Phone,
                user.PhoneRegion,
                user.Email,
                command.Score,
                command.AdminPhone)).ToList();
            await scoreDistributeRecordRepository.AddRangeAsync(scoreDistributeRecords, cancellationToken);
            return true;
        }
        finally
        {
            await ReleaseUserLocks(userLocks, cancellationToken);
        }
        
    }
    
    /// <summary>
    /// 获取锁库存的分布式锁
    /// </summary>
    private async Task<ILockSynchronizationHandle?> AcquireUserLock4LockInventoryAsync(
        string userId,
        CancellationToken cancellationToken
    )
    {
        return await distributedLock.TryAcquireAsync($"score-distribution-lock:{userId}",
            TimeSpan.FromSeconds(5),
            cancellationToken);
    }
    
    private async Task<ILockSynchronizationHandle[]> AcquireUserLocks(IEnumerable<string> userIds, CancellationToken cancellationToken)
    {
        var lockTasks = userIds.Distinct().OrderBy(id => id).Select(id => AcquireUserLock4LockInventoryAsync(id, cancellationToken));
        // 等待所有锁任务完成
        var locks = await Task.WhenAll(lockTasks);
        if (locks.Any(l => l is null))
        {
            throw new KnownException("当前系统繁忙，请稍后再试");
        }
        return locks;
    }
    
    private async Task ReleaseUserLocks(IEnumerable<ILockSynchronizationHandle> locks, CancellationToken cancellationToken)
    {
        if (locks == null || !locks.Any())
        {
            return;
        }
        var releaseTasks = locks
            .Where(locker => locker != null)
            .Select(async locker =>
            {
                try
                {
                    await locker.DisposeAsync();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "释放锁失败");
                }
            });
        await Task.WhenAll(releaseTasks);
    }
}