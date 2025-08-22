using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Infrastructure.Repositories;

namespace Xiangjiandao.Web.Application.Commands;

/// <summary>
/// 用户注册命令
/// </summary>
public class RegisterCommand : ICommand<UserId>
{
    /// <summary>
    /// 邮箱
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 手机号
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// 如果是手机号，需要传区号
    /// </summary>
    public string PhoneRegion { get; set; } = "86";

    /// <summary>
    /// 用户域名
    /// </summary>
    public required string DomainName { get; set; }

    /// <summary>
    /// BlueSky 生成的用户 Did
    /// </summary>
    public required string Did { get; set; }
}

public class RegisterCommandHandler(
    IUserRepository repository,
    ILogger<RegisterCommandHandler> logger
) : ICommandHandler<RegisterCommand, UserId>
{
    public async Task<UserId> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("RegisterCommand Handling");
        var existUser = await repository.FindByPhoneOrEmail(
            email: command.Email,
            phone: command.Phone,
            cancellationToken: cancellationToken
        );

        if (existUser is not null)
        {
            throw new KnownException("该手机号或邮箱已被使用");
        }

        var newUser = User.Register(
            email: command.Email,
            phone: command.Phone,
            phoneRegion: command.PhoneRegion,
            domainName: command.DomainName,
            did: command.Did
        );

        await repository.AddAsync(newUser, cancellationToken);
        return newUser.Id;
    }
}