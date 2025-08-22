using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Web.Application.Commands;

namespace Xiangjiandao.Web.Application.Req;

/// <summary>
/// 设置节点用户请求
/// </summary>
public class SetNodeUserReq
{
    /// <summary>
    /// 用户Id
    /// </summary>
    public UserId Id { get; set; } = default!;
    
    /// <summary>
    /// 转换为命令
    /// </summary>
    public SetNodeUserCommand ToCommand()
    {
        return new SetNodeUserCommand(Id);
    }
}