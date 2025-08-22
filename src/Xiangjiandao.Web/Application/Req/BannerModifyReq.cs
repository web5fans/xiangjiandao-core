using FluentValidation;
using Xiangjiandao.Domain.AggregatesModel.BannerAggregate;
using Xiangjiandao.Web.Application.Commands;

namespace Xiangjiandao.Web.Application.Req;

/// <summary>
/// Banner 编辑请求
/// </summary>
public class BannerModifyReq
{
    /// <summary>
    /// BannerId
    /// </summary>
    public BannerId Id { get; set; } = default!;
    
    /// <summary>
    /// Banner 文件 Id
    /// </summary> 
    public string BannerFileId { get;  set; } = string.Empty;

    /// <summary>
    /// 链接地址
    /// </summary> 
    public string LinkAddress { get;  set; } = string.Empty;
    
    public ModifyBannerCommand ToCommand()
    {
        return new ModifyBannerCommand(
            Id,
            BannerFileId,
            LinkAddress
        );
    }
}

/// <summary>
/// Banner 编辑请求验证器
/// </summary>
public class BannerModifyReqValidator : AbstractValidator<BannerModifyReq>
{
    public BannerModifyReqValidator()
    {
        RuleFor(x => x.BannerFileId)
            .NotEmpty()
            .WithMessage("横幅文件Id不能为空");
    }
}