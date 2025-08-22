using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.ScoreRecordAggregate;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Domain.Enums;
using Xiangjiandao.Infrastructure.Repositories;

namespace Xiangjiandao.Web.Application.Commands;

/// <summary>
/// 创建转赠打赏的稻米记录
/// </summary>
public record CreateSendRewardScoreRecordCommand:  ICommand<bool>
{
    /// <summary>
    /// 所属用户
    /// </summary> 
    public UserId FromUserId { get; set; } = null!;
    
    /// <summary>
    /// 接收用户
    /// </summary> 
    public UserId ToUserId { get; set; } = null!;

    /// <summary>
    /// 稻米数量
    /// </summary> 
    public long Score { get; set; }
    
    /// <summary>
    /// 原因
    /// </summary>
    public string Reason { get; set; } = string.Empty;
    
    /// <summary>
    /// 类型
    /// </summary>
    public ScoreSourceType Type { get; set; } = ScoreSourceType.Unknown;
}

/// <summary>
/// 创建转赠打赏的稻米记录
/// </summary>
public class CreateSendRewardScoreRecordCommandHandler(
    IScoreRecordRepository scoreRecordRepository,
    IUserRepository userRepository,
    ILogger<CreateSendRewardScoreRecordCommandHandler> logger) : ICommandHandler<CreateSendRewardScoreRecordCommand, bool>
{ 
    public async Task<bool> Handle(CreateSendRewardScoreRecordCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("创建转赠打赏稻米明细");
        var fromUser = await userRepository.GetAsync(command.FromUserId, cancellationToken);
        if (fromUser == null || fromUser.Deleted)
        {
            throw new KnownException("当前用户不存在");
        }
        if (fromUser.Score < command.Score)
        {
            throw new KnownException("当前用户稻米不足");
        }
        var toUser = await userRepository.GetAsync(command.ToUserId, cancellationToken);
        if (toUser == null || toUser.Deleted)
        {
            throw new KnownException("接收用户不存在");
        }

        var record = new List<ScoreRecord>();
        record.Add(ScoreRecord.Create(
            fromUser.Id,
            command.Type,
            $"{command.Reason}用户「 {toUser.DomainName} 」",
            -1 * command.Score
        ));
        record.Add(ScoreRecord.Create(
            toUser.Id,
            command.Type,
            $"用户「 {fromUser.DomainName} 」{command.Reason}",
            command.Score
        ));
        await scoreRecordRepository.AddRangeAsync(record, cancellationToken);
        return true;
    }
}