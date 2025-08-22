using FluentValidation;
using Xiangjiandao.Web.Application.Commands;

namespace Xiangjiandao.Web.Application.Req;

/// <summary>
/// 创建勋章
/// </summary>
public class CreateMedalReq
{
    /// <summary>
    /// 封面Id
    /// </summary>
    public string AttachId { get; set; } = string.Empty;
    
    /// <summary>
    /// 勋章名称
    /// </summary> 
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 获得用户文件 Id
    /// </summary> 
    public string FileId { get; set; } = string.Empty;

    public CreateMedalCommand ToCommand(List<string> userPhoneOrEmails)
    {
        return new CreateMedalCommand(AttachId, Name, FileId, userPhoneOrEmails.Count, userPhoneOrEmails);
    }

}

public class CreateMedalReqValidator : AbstractValidator<CreateMedalReq>
{
    public CreateMedalReqValidator()
    {
        RuleFor(x => x.AttachId).NotEmpty().WithMessage("封面不能为空");
        RuleFor(x => x.Name).NotEmpty().WithMessage("名称不能为空");
        RuleFor(x => x.FileId).NotEmpty().WithMessage("文件Id不能为空");
    }
}