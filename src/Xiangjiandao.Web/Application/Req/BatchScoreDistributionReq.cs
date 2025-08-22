using FluentValidation;
using Xiangjiandao.Web.Application.Commands;

namespace Xiangjiandao.Web.Application.Req;

public record BatchScoreDistributionReq
{
    /// <summary>
    /// 获得用户文件 Id
    /// </summary> 
    public string FileId { get; set; } = string.Empty;
    
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
    /// <param name="userPhoneOrEmails"></param>
    /// <param name="phone"></param>
    /// <returns></returns>
    public BatchScoreDistributionCommand ToCommand(List<string> userPhoneOrEmails, string phone)
    {
        return new BatchScoreDistributionCommand()
        {
            Score = Score,
            UserPhoneOrEmails = userPhoneOrEmails,
            AdminPhone = phone
        };
    }
}

public class BatchScoreDistributionReqValidator : AbstractValidator<BatchScoreDistributionReq>
{
    public BatchScoreDistributionReqValidator()
    {
        RuleFor(x => x.FileId).NotEmpty().WithMessage("获得用户文件 Id不能为空");
        RuleFor(x => x.Score).GreaterThan(0).WithMessage("稻米值必须大于0");
        RuleFor(x => x.Code).NotEmpty().WithMessage("验证码不能为空");
    }
}