using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Infrastructure.Repositories;

namespace Xiangjiandao.Web.Application.Commands;

/// <summary>
/// 批量更新徽章用户信心
/// </summary>
public class BatchUpdateMedalUserInfoCommand : ICommand<bool>
{
    /// <summary>
    /// 用户 Id
    /// </summary>
    public required UserId UserId { get; set; }
}

/// <summary>
/// 命令处理器
/// </summary>
public class BatchUpdateMedalUserInfoCommandHandler(
    IUserRepository userRepository,
    IUserMedalRepository userMedalRepository,
    ILogger<BatchUpdateMedalUserInfoCommandHandler> logger
) : ICommandHandler<BatchUpdateMedalUserInfoCommand, bool>
{
    public async Task<bool> Handle(BatchUpdateMedalUserInfoCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("BatchUpdateMedalUserInfoCommand Handling");
        var user = await userRepository.GetAsync(command.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogError("User not found, UserId: {UserId}", command.UserId);
            throw new KnownException("用户未找到");
        }
        
        var userMedals = await userMedalRepository.GetByUserId(userId: command.UserId, cancellationToken);
        foreach (var userMedal in userMedals)
        {
            userMedal.ModifyUserInfo(
                nickName: user.NickName,
                avatar:  user.Avatar,
                phone: user.Phone,
                phoneRegion: user.PhoneRegion
            );
        }

        return true;
    }
}