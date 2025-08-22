using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.AdminUserAggregate;
using Xiangjiandao.Infrastructure.Repositories;

namespace Xiangjiandao.Web.Application.Commands;

/// <summary>
/// 删除后台账号
/// </summary>
/// <param name="Id"></param>
public record DeleteAdminUserCommand(AdminUserId Id): ICommand<bool>{}

/// <summary>
/// 删除后台账号
/// </summary>
public class DeleteAdminUserCommandHandler(
    IAdminUserRepository adminUserRepository,
    ILogger<DeleteAdminUserCommandHandler> logger): ICommandHandler<DeleteAdminUserCommand, bool>
{
    public async Task<bool> Handle(DeleteAdminUserCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("删除后台账号 Id:{Id}", request.Id);
        var adminUser = await adminUserRepository.GetAsync(request.Id, cancellationToken);
        if (adminUser == null || adminUser.Deleted)
        {
            throw new KnownException("管理员不存在");
        }
        if (adminUser.Special)
        {
            throw new KnownException("超级管理员账号不能被删除");
        }

        return adminUser.Delete();
    }
}