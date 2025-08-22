using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Infrastructure.Repositories;

namespace Xiangjiandao.Web.Application.Commands;

/// <summary>
/// 修改用户手机号
/// </summary>
public record ModifyUserPhoneCommand: ICommand<bool>
{
    /// <summary>
    /// 用户Id
    /// </summary>
    public UserId Id { get; set; } = default!;
    
    /// <summary>
    /// 手机区号, 默认86
    /// </summary>
    public string PhoneRegion { get; set; } = "86";
    
    /// <summary>
    /// 手机号
    /// </summary>
    public string Phone { get; set; } = string.Empty;
}

/// <summary>
/// 修改用户手机号
/// </summary>
/// <param name="userRepository"></param>
/// <param name="logger"></param>
public class ModifyUserPhoneCommandCommandHandler(
    IUserRepository userRepository,
    ILogger<ModifyUserEmailAddressCommandHandler> logger
    ) : ICommandHandler<ModifyUserPhoneCommand, bool>
{ 
    public async Task<bool> Handle(ModifyUserPhoneCommand request, CancellationToken cancellationToken)
    { 
        logger.LogInformation("修改用户手机 Id:{Id}, Phone:{Phone}, PhoneRegion:{PhoneRegion}", request.Id, request.Phone, request.PhoneRegion);
        var user = await userRepository.GetAsync(request.Id, cancellationToken);
        if (user == null || user.Deleted)
        {
            throw new KnownException("用户不存在");
        }
        if (user.Disable)
        {
            throw new KnownException("用户已被禁用",401);
        }
        user.UpdatePhone(request.Phone, request.PhoneRegion);
        return true;
    }
}