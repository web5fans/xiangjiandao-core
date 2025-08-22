using FluentValidation;
using Xiangjiandao.Domain.AggregatesModel.BannerAggregate;
using Xiangjiandao.Web.Application.Commands;

namespace Xiangjiandao.Web.Application.Req;

/// <summary>
/// Banner排序请求
/// </summary>
public class BannerSortReq
{
    /// <summary>
    /// BannerId集合
    /// </summary>
    public List<BannerId> BannerIds { get; set; } = new();
    
    /// <summary>
    /// 转换为命令
    /// </summary>
    /// <returns></returns>
    public SortBannerCommand ToCommand()
    {
        return new SortBannerCommand(
            BannerIds
        );
    }
}

/// <summary>
/// Banner排序请求验证器
/// </summary>
public class BannerSortReqValidator : AbstractValidator<BannerSortReq>
{
    public BannerSortReqValidator()
    {
        RuleFor(x => x.BannerIds)
            .NotEmpty()
            .WithMessage("横幅Id不能为空");
    }
}