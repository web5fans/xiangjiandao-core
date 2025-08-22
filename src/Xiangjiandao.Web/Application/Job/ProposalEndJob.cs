using MediatR;
using Xiangjiandao.Web.Application.Commands;
using Xiangjiandao.Web.Application.Queries;

namespace Xiangjiandao.Web.Application.Job;

/// <summary>
/// 提案结束任务
/// </summary>
public class ProposalEndJob(
    IMediator mediator,
    ProposalQuery query,
    ILogger<ProposalEndJob> logger
)
{
    public async Task RunAsync()
    {
        logger.LogInformation("ProposalEndJob start");
        var now = DateTimeOffset.Now;
        var toEndProposalIds = await query.ToEndProposalIds(endAt: now);
        var commands = toEndProposalIds
            .Select(id => new EndProposalCommand
            {
                ProposalId = id,
            }).ToList();
        var tasks = commands.Select(command => mediator.Send(command));
        await Task.WhenAll(tasks);
    }
}