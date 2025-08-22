using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Infrastructure.Repositories;

namespace Xiangjiandao.Web.Application.Commands;

/// <summary>
/// 编辑个人资料
/// </summary>
public record EditProfileCommand : ICommand<bool>
{
    /// <summary>
    /// 用户Id
    /// </summary>
    public UserId Id { get; set; } = null!;
    
    /// <summary>
    /// 用户头像
    /// </summary> 
    public string Avatar { get; set; } = string.Empty;

    /// <summary>
    /// 用户昵称
    /// </summary> 
    public string NickName { get; set; } = string.Empty;

    /// <summary>
    /// 简介
    /// </summary> 
    public string Introduction { get; set; } = string.Empty;
}

public class EditProfileCommandHandler(
    IUserRepository userRepository,
    ILogger<EditProfileCommandHandler> logger
    ) : ICommandHandler<EditProfileCommand, bool>
{ 
    public async Task<bool> Handle(EditProfileCommand command, CancellationToken cancellationToken)
    { 
        logger.LogInformation("EditProfileCommand Handling");
        var user = await userRepository.GetAsync(command.Id, cancellationToken);
        if (user == null || user.Deleted)
        {
            throw new KnownException("用户不存在");
        }

        if (user.Disable)
        {
            throw new KnownException("用户已被禁用", 401);
        }
        user.UpdateProfile(command.Avatar, command.NickName, command.Introduction);
        return true;   
    }
}