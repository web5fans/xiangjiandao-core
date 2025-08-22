using Xiangjiandao.Domain.AggregatesModel.MedalAggregate;

namespace Xiangjiandao.Web.Application.Req;

public record AdminUserMedalPageReq
{
    /// <summary>
    /// 勋章Id
    /// </summary>
    public MedalId MedalId { get; set; } = default!;

    /// <summary>
    /// 手机号或邮箱
    /// </summary>
    public string PhoneOrEmail { get; set; } = string.Empty;
    
    /// <summary>
    ///  分页页码
    /// </summary>
    public int PageNum { get; set; } = 1;

    /// <summary>
    /// 分页大小
    /// </summary>
    public int PageSize { get; set; } = 10;
}