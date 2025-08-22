using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.ProposalAggregate;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Infrastructure.Repositories;

namespace Xiangjiandao.Web.Application.Commands;

/// <summary>
/// 创建提案命令
/// </summary>
public class CreateProposalCommand : ICommand<ProposalId>
{
    /// <summary>
    /// 提案名称
    /// </summary> 
    public required string Name { get; set; }

    /// <summary>
    /// 发起方 Id
    /// </summary> 
    public required UserId InitiatorId { get; set; }

    /// <summary>
    /// 投票截至时间
    /// </summary> 
    public required DateTimeOffset EndAt { get; set; }

    /// <summary>
    /// 附件 Id
    /// </summary> 
    public required string AttachId { get; set; }
}

/// <summary>
/// 命令处理器
/// </summary>
public class CreateProposalCommandHandler(
    IProposalRepository repository,
    IUserRepository userRepository,
    ILogger<CreateProposalCommandHandler> logger
) : ICommandHandler<CreateProposalCommand, ProposalId>
{
    public async Task<ProposalId> Handle(CreateProposalCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("CreateProposalCommand Handling");
        var user = await userRepository.GetAsync(command.InitiatorId, cancellationToken);
        if (user is null)
        {
            throw new KnownException("未找到该用户");
        }

        if (user.Disable)
        {
            throw new KnownException("该用户已被禁用");
        }

        if (!user.NodeUser)
        {
            throw new KnownException("非节点用户不能提案");
        }


        var proposal = Proposal.Create(
            name: command.Name,
            initiatorId: command.InitiatorId,
            initiatorDid: user.Did,
            initiatorDomainName: user.DomainName,
            initiatorName: user.NickName,
            initiatorEmail: user.Email,
            initiatorAvatar: user.Avatar,
            endAt: command.EndAt,
            attachId: command.AttachId
        );
        await repository.AddAsync(proposal, cancellationToken);
        return proposal.Id;
    }
}