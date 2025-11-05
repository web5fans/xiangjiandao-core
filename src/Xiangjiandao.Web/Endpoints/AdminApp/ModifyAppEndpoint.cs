using FastEndpoints;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Domain.AggregatesModel.AppAggregate;
using Xiangjiandao.Web.Application.Commands;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.AdminApp;

/// <summary>
/// 编辑应用 Endpoint
/// </summary>
[Tags("AdminApp")]
[HttpPost("/api/v1/admin/app/modify")]
[Authorize(PolicyNames.Admin)]
public class ModifyAppEndpoint(
    IMediator mediator
) : Endpoint<ModifyAppReq, ResponseData<bool>>
{
    public override async Task HandleAsync(ModifyAppReq req, CancellationToken ct)
    {
        var command = req.ToCommand();
        var result = await mediator.Send(command, ct);
        await SendAsync(response: result.AsSuccessResponseData(), cancellation: ct);
    }
}

/// <summary>
/// 编辑应用请求
/// </summary>
public class ModifyAppReq
{
    /// <summary>
    /// 应用 Id
    /// </summary>
    public required AppId AppId { get; set; }

    /// <summary>
    /// 应用名称
    /// </summary> 
    public required string Name { get; set; }

    /// <summary>
    /// 应用描述
    /// </summary> 
    public required string Desc { get; set; }

    /// <summary>
    /// 应用图标
    /// </summary> 
    public required string Logo { get; set; }

    /// <summary>
    /// 应用链接
    /// </summary> 
    public required string Link { get; set; }

    /// <summary>
    /// 请求转命令
    /// </summary>
    public ModifyAppCommand ToCommand()
    {
        return new ModifyAppCommand
        {
            AppId = AppId,
            Name = Name,
            Desc = Desc,
            Logo = Logo,
            Link = Link,
        };
    }
}

/// <summary>
/// 请求验证噐
/// </summary>
public class ModifyAppReqValidator : AbstractValidator<ModifyAppReq>
{
    public ModifyAppReqValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("应用名称不能为空");
        RuleFor(x => x.Desc)
            .NotEmpty()
            .Must(desc => desc.Length < 16)
            .WithMessage("应用描述不能为空且不能超过 16 个字符");
        RuleFor(x => x.Logo)
            .NotEmpty()
            .WithMessage("应用 Logo 不能为空");
        RuleFor(link => link.Link)
            .NotEmpty()
            .Must(link => Uri.IsWellFormedUriString(link, UriKind.Absolute))
            .WithMessage("应用链接不能为空且是有效的链接");
    }
}