using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.AdminUserAggregate;
using Xiangjiandao.Domain.Enums;
using Xiangjiandao.Infrastructure.Repositories;
using Xiangjiandao.Web.Application.Vo;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Application.Commands;

/// <summary>
/// 新增后台账号
/// </summary>
/// <param name="Role"></param>
/// <param name="Phone"></param>
/// <param name="PhoneRegion"></param>
public record CreateAdminUserCommand(RoleType Role, string Phone, string PhoneRegion): ICommand<AdminUserCreateVo>{}

/// <summary>
/// 新增后台账号
/// </summary>
public class CreateAdminUserCommandHandler(
    IAdminUserRepository adminUserRepository,
    ILogger<CreateAdminUserCommandHandler> logger) : ICommandHandler<CreateAdminUserCommand, AdminUserCreateVo>
{
    public async Task<AdminUserCreateVo> Handle(CreateAdminUserCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("创建后台账号");
        var password = StringUtils.GenRandomString(8);
        var secretData = PasswordHashGenerator.Hash(password);
        var adminUser = request.Role == RoleType.Admin?AdminUser.CreateAdmin(string.Empty, request.Phone,request.PhoneRegion,"", secretData):
                AdminUser.CreateOperator(string.Empty, request.Phone,request.PhoneRegion, "", secretData);
        await adminUserRepository.AddAsync(adminUser, cancellationToken);
        return new AdminUserCreateVo()
        {
            Id = adminUser.Id,
            Password = password
        };
    }
}