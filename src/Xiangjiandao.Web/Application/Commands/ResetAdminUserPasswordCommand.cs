using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Infrastructure.Repositories;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Application.Commands;

public record ResetAdminUserPasswordCommand(string PhoneRegion, string Phone, string Password): ICommand<bool>{}

/// <summary>
/// 管理员重置密码
/// </summary>
/// <param name="adminUserRepository"></param>
/// <param name="logger"></param>
public class ResetAdminUserPasswordCommandHandler(
    IAdminUserRepository adminUserRepository,
    ILogger<ResetAdminUserPasswordCommandHandler> logger
): ICommandHandler<ResetAdminUserPasswordCommand, bool>
{
    public async Task<bool> Handle(ResetAdminUserPasswordCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("管理员重置密码 Phone:{Phone}", request.Phone);
        // 根据邮箱号查管理员
        var adminUser = await adminUserRepository.FindByPhoneAsync(request.Phone, request.PhoneRegion, cancellationToken);
        if (adminUser is null)
        {
            throw new KnownException("管理员不存在");
        }
        
        var secretData = PasswordHashGenerator.Hash(request.Password);
         adminUser.ModifyPassword(
             secretData: secretData
         );
         
         return true;
    }
}