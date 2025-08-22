using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Infrastructure.Repositories;

namespace Xiangjiandao.Web.Application.Commands;

/// <summary>
/// 修改用户邮箱
/// </summary>
public record ModifyUserEmailAddressCommand : ICommand<bool>
{
    /// <summary>
    /// 用户Id
    /// </summary>
    public UserId Id { get; set; } = default!;
    
    /// <summary>
    /// 邮箱地址
    /// </summary>
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// 修改用户邮箱
/// </summary>
public class ModifyUserEmailAddressCommandHandler(
    IUserRepository userRepository,
    ILogger<ModifyUserEmailAddressCommandHandler> logger
) : ICommandHandler<ModifyUserEmailAddressCommand, bool>
{ 
    public async Task<bool> Handle(ModifyUserEmailAddressCommand request, CancellationToken cancellationToken)
    { 
        logger.LogInformation("用户修改邮箱地址 Id:{Id}, Email:{Email}", request.Id, request.Email);
        var user = await userRepository.GetAsync(request.Id, cancellationToken);
        if (user == null || user.Deleted)
        {
            throw new KnownException("用户不存在");
        }
        if (user.Disable)
        {
            throw new KnownException("用户已被禁用",401);
        }
        user.UpdateEmail(request.Email);
        return true;
    }
}

