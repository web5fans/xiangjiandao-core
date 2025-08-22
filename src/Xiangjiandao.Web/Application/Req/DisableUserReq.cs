using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Web.Application.Commands;

namespace Xiangjiandao.Web.Application.Req;

/// <summary>
/// 禁用用户请求
/// </summary>
public class DisableUserReq
{
    /// <summary>
    /// 用户Id
    /// </summary>
    public UserId Id { get; set; } = default!;
    
    /// <summary>
    /// 转换为命令
    /// </summary>
    public DisableUserCommand ToCommand()
    {
        return new DisableUserCommand(Id);
    }
}