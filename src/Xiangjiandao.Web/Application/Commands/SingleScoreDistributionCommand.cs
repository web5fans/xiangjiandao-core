using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.ScoreDistributeRecordAggregate;
using Xiangjiandao.Infrastructure.Repositories;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Application.Commands;

public record SingleScoreDistributionCommand :  ICommand<ScoreDistributeRecordId>
{
    /// <summary>
    /// 手机号或邮箱
    /// </summary>
    public string PhoneOrEmail { get; set; } = string.Empty;
    
    /// <summary>
    /// 发放稻米
    /// </summary> 
    public long Score { get; set; }
    
    /// <summary>
    /// 管理员手机号
    /// </summary>
    public string AdminPhone { get; set; } = string.Empty;
}

public class SingleScoreDistributionCommandHandler(
    IScoreDistributeRecordRepository scoreDistributeRecordRepository,
    IUserRepository userRepository,
    IXiangjiandaoDistributedDisLock distributedLock,
    ILogger<SingleScoreDistributionCommandHandler> logger) : ICommandHandler<SingleScoreDistributionCommand, ScoreDistributeRecordId>
{ 
    public async Task<ScoreDistributeRecordId> Handle(SingleScoreDistributionCommand command, CancellationToken cancellationToken)
    { 
        logger.LogInformation("单个发放稻米");
        var user = await userRepository.FindByPhoneOrEmail(command.PhoneOrEmail, cancellationToken);
        
        if (user == null)
        {
            throw new KnownException("手机号对应的用户不存在");
        }
        
        var key = $"score-distribution-lock:{user.Id.Id.ToString()}";
        await using var synchronizationHandle = await distributedLock.TryAcquireAsync(key,  TimeSpan.FromSeconds(5), cancellationToken);
        if (synchronizationHandle == null)
        {
            throw new KnownException("当前系统繁忙，请稍后再试");
        }
        var scoreDistributeRecord = ScoreDistributeRecord.Create(
            user.Id,
            user.NickName,
            user.Phone,
            user.PhoneRegion,
            user.Email,
            command.Score,
            command.AdminPhone);
        
        await scoreDistributeRecordRepository.AddAsync(scoreDistributeRecord, cancellationToken);
        return scoreDistributeRecord.Id;
    }
}