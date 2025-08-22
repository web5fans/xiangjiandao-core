using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.AdminUserAggregate;
using Xiangjiandao.Infrastructure.Repositories;

namespace Xiangjiandao.Web.Application.Commands;

/// <summary>
/// 修改后台用户头像
/// </summary>
public record ModifyAdminUserAvatarCommand(AdminUserId AdminUserId, string Avatar) : ICommand<bool>;

/// <summary>
/// 修改后台用户头像
/// </summary>
public class ModifyAdminUserAvatarCommandHandler(
    IAdminUserRepository adminUserRepository,
    ILogger<ModifyAdminUserAvatarCommandHandler> logger
) : ICommandHandler<ModifyAdminUserAvatarCommand, bool>
{
    public async Task<bool> Handle(ModifyAdminUserAvatarCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("修改管理员用户头像 Id:{Id}", request.AdminUserId);
        var adminUser = await adminUserRepository.GetAsync(request.AdminUserId, cancellationToken);
        if (adminUser == null || adminUser.Deleted)
        {
            throw new KnownException("管理员不存在");
        }
        return adminUser.ModifyAvatar(request.Avatar);
    }
}