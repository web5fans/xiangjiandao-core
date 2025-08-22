using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.MedalAggregate;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Domain.Dto;
using Xiangjiandao.Infrastructure.Repositories;

namespace Xiangjiandao.Web.Application.Commands;

/// <summary>
/// 批量创建用户勋章
/// </summary>
public record CreateBatchUserMedalCommand : ICommand<bool>
{
    /// <summary>
    /// 勋章Id
    /// </summary>
    public MedalId MedalId { get; set; } = default!;
    
    /// <summary>
    /// 勋章封面Id
    /// </summary>
    public string AttachId { get; set; } = string.Empty;
    
    /// <summary>
    /// 勋章名称
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 勋章数量
    /// </summary>
    public long Quantity { get; set; }
    
    /// <summary>
    /// 用户手机号或邮箱
    /// </summary>
    public List<UserInfoDto> UserInfoDtos { get; set; } = new List<UserInfoDto>();
}

/// <summary>
/// 批量创建用户勋章
/// </summary>
public class CreateUserMedalCommandHandler(
    IUserMedalRepository medalRepository,
    ILogger<CreateUserMedalCommandHandler> logger) : ICommandHandler<CreateBatchUserMedalCommand, bool>
{ 
    public async Task<bool> Handle(CreateBatchUserMedalCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("创建用户勋章");
        var userMedals = command.UserInfoDtos.Select(user => UserMedal.Create(
            user.Id,
            command.MedalId,
            user.NickName,
            user.Avatar,
            user.Phone,
            user.PhoneRegion,
            user.Email,
            command.AttachId,
            command.Name,
            DateTimeOffset.Now)).ToList();
        
        await medalRepository.AddRangeAsync(userMedals, cancellationToken);

        return true;
    }
}