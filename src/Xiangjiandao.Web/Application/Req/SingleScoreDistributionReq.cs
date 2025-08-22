using FluentValidation;
using Xiangjiandao.Web.Application.Commands;

namespace Xiangjiandao.Web.Application.Req;

/// <summary>
/// 单个发放稻米
/// </summary>
public record SingleScoreDistributionReq
{
    /// <summary>
    /// 手机号或邮箱
    /// </summary>
    public string PhoneOrEmail { get; set; } = string.Empty;
    
    /// <summary>
    /// 发放稻米
    /// </summary> 
    public long Score { get; set; }
    
    /// <summary>
    /// 验证码
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 转换为命令
    /// </summary>
    /// <returns></returns>
    public SingleScoreDistributionCommand ToCommand(string phone)
    {
        return new SingleScoreDistributionCommand
        {
            PhoneOrEmail = PhoneOrEmail,
            Score = Score,
            AdminPhone = phone
        };
    }
}

public class SingleScoreDistributionReqValidator : AbstractValidator<SingleScoreDistributionReq>
{
    public SingleScoreDistributionReqValidator()
    {
        RuleFor(x => x.PhoneOrEmail).NotEmpty().WithMessage("手机号或邮箱不能为空");
        RuleFor(x => x.Score).GreaterThan(0).WithMessage("稻米值必须大于0");
        RuleFor(x => x.Code).NotEmpty().WithMessage("验证码不能为空");
    }
}