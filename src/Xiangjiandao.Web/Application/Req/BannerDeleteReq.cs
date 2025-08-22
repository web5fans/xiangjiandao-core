using Xiangjiandao.Domain.AggregatesModel.BannerAggregate;
using Xiangjiandao.Web.Application.Commands;

namespace Xiangjiandao.Web.Application.Req;

/// <summary>
/// Banner删除请求
/// </summary>
public class BannerDeleteReq
{
    /// <summary>
    /// BannerId
    /// </summary>
    public BannerId Id { get; set; } = default!;
    
    /// <summary>
    /// 转换为命令
    /// </summary>
    /// <returns></returns>
    public DeleteBannerCommand ToCommand()
    {
        return new DeleteBannerCommand(Id);
    }
}