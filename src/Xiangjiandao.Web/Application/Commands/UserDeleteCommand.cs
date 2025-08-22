using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Infrastructure.Repositories;

namespace Xiangjiandao.Web.Application.Commands;

/// <summary>
/// 删除用户命令
/// </summary>
public class UserDeleteCommand : ICommand<bool>
{
    /// <summary>
    /// 手机号
    /// </summary>
    public required string Phone { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    public required string Email { get; set; }
}

/// <summary>
/// 命令处理器
/// </summary>
public class UserDeleteCommandHandler(
    IUserRepository userRepository,
    ILogger<UserDeleteCommandHandler> logger
) : ICommandHandler<UserDeleteCommand, bool>
{
    public async Task<bool> Handle(UserDeleteCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("UserDeleteCommand Handling");
        var toDeleteUser = await userRepository.FindByPhoneOrEmail(
            email: command.Email,
            phone: command.Phone,
            cancellationToken: cancellationToken
        );

        if (toDeleteUser is null)
        {
            throw new KnownException("用户未找到");
        }

        toDeleteUser.Delete();

        return true;
    }
}