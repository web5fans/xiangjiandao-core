using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.ScoreRecordAggregate;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Domain.Enums;
using Xiangjiandao.Infrastructure.Repositories;

namespace Xiangjiandao.Web.Application.Commands;

/// <summary>
/// 创建稻米记录
/// </summary>
public record CreateScoreRecordCommand:  ICommand<ScoreRecordId>
{
    /// <summary>
    /// 所属用户
    /// </summary> 
    public UserId UserId { get; set; } = null!;

    /// <summary>
    /// 稻米来源类型
    /// </summary> 
    public ScoreSourceType Type { get; set; } = ScoreSourceType.Unknown;

    /// <summary>
    /// 获得原因
    /// </summary> 
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// 稻米数量
    /// </summary> 
    public long Score { get; set; }
}

public class CreateScoreRecordCommandHandler(
    IScoreRecordRepository scoreRecordRepository,
    ILogger<CreateScoreRecordCommandHandler> logger) : ICommandHandler<CreateScoreRecordCommand, ScoreRecordId>
{ 
    public async Task<ScoreRecordId> Handle(CreateScoreRecordCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("创建稻米明细");
        var scoreRecord = ScoreRecord.Create(
            command.UserId,
            command.Type,
            command.Reason,
            command.Score
        );
        await scoreRecordRepository.AddAsync(scoreRecord, cancellationToken);
        return scoreRecord.Id;
    }
}