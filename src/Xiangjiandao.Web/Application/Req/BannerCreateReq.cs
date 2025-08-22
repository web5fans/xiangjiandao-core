using FluentValidation;
using Xiangjiandao.Web.Application.Commands;

namespace Xiangjiandao.Web.Application.Req;

/// <summary>
/// Banner 创建请求
/// </summary>
public class BannerCreateReq
{
    /// <summary>
    /// Banner 文件 Id
    /// </summary> 
    public string BannerFileId { get;  set; } = string.Empty;

    /// <summary>
    /// 链接地址
    /// </summary> 
    public string LinkAddress { get;  set; } = string.Empty;

    /// <summary>
    /// 转换为命令
    /// </summary>
    /// <returns></returns>
    public CreateBannerCommand ToCommand()
    {
        return new CreateBannerCommand()
        {
            BannerFileId = BannerFileId,
            LinkAddress = LinkAddress
        };
    }
}

/// <summary>
/// Banner 创建请求验证器
/// </summary>
public class BannerCreateReqValidator : AbstractValidator<BannerCreateReq>
{
    public BannerCreateReqValidator()
    {
        RuleFor(x => x.BannerFileId)
            .NotEmpty()
            .WithMessage("横幅文件Id不能为空");
    }
}