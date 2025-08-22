using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.MedalAggregate;
using Xiangjiandao.Domain.Dto;
using Xiangjiandao.Infrastructure.Repositories;

namespace Xiangjiandao.Web.Application.Commands;

/// <summary>
/// 创建勋章
/// </summary>
/// <param name="AttachId"></param>
/// <param name="Name"></param>
/// <param name="FileId"></param>
/// <param name="Quantity"></param>
/// <param name="UserPhoneOrEmails"></param>
public record CreateMedalCommand(string AttachId, string Name, string FileId, long Quantity,List<string> UserPhoneOrEmails) : ICommand<MedalId>;

/// <summary>
/// 创建勋章处理
/// </summary>
/// <param name="medalRepository"></param>
/// <param name="logger"></param>
public class CreateMedalCommandHandler(
    IMedalRepository medalRepository,
    IUserRepository userRepository,
    ILogger<CreateMedalCommandHandler> logger) : ICommandHandler<CreateMedalCommand, MedalId>
{ 
    public async Task<MedalId> Handle(CreateMedalCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("创建勋章");
        
        var users = await userRepository.GetByPhoneOrEmailAsync(request.UserPhoneOrEmails, cancellationToken);
        if (users.Count == 0)
        {
            throw new KnownException("用户不存在");
        }

        if (users.Count != request.Quantity)
        { 
            throw new KnownException("提交的手机号或邮箱不全是平台用户或是同一个用户");
        }

        var userInfos = users.Select(user => new UserInfoDto
        {
            Id = user.Id,
            Email = user.Email,
            Phone = user.Phone,
            PhoneRegion = user.PhoneRegion,
            Avatar = user.Avatar,
            NickName = user.NickName
        }).ToList();
        
        var medal = Medal.Create(
            request.AttachId,
            request.Name,
            request.FileId,
            request.Quantity ,
            userInfos
        );
        await medalRepository.AddAsync(medal, cancellationToken);
        return medal.Id;
    }
}