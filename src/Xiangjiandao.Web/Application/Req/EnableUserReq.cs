using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Web.Application.Commands;

namespace Xiangjiandao.Web.Application.Req;

/// <summary>
/// 启用用户
/// </summary>
public class EnableUserReq
{
    /// <summary>
    /// 用户Id
    /// </summary>
    public UserId Id { get; set; } = default!;
    
    public EnableUserCommand ToCommand()
    {
        return new EnableUserCommand(Id);
    }
}