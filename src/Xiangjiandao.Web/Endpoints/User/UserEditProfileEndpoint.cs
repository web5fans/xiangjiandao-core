using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Web.Application.Commands;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Shared;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.User;

/// <summary>
/// 编辑个人资料
/// </summary>
[Tags("User")]
[HttpPost("/api/v1/user/edit-profile")]
[Authorize(PolicyNames.Client)]
public class UserEditProfileEndpoint(
    IMediator mediator,
    ILoginUser loginUser
) : Endpoint<UserEditProfileReq, ResponseData<bool>>
{
    public override async Task HandleAsync(UserEditProfileReq req, CancellationToken ct)
    { 
        var userId = new UserId(loginUser.Id);
        var command = req.ToCommand(userId);
        await SendAsync(await mediator.Send(command, ct)
            .AsSuccessResponseData(), cancellation: ct);
    }
}

/// <summary>
/// 编辑个人资料请求 支持保存成空
/// </summary>
public record UserEditProfileReq
{ 
    /// <summary>
    /// 用户头像
    /// </summary> 
    public string Avatar { get; set; } = string.Empty;

    /// <summary>
    /// 用户昵称
    /// </summary> 
    public string NickName { get; set; } = string.Empty;

    /// <summary>
    /// 简介
    /// </summary> 
    public string Introduction { get; set; } = string.Empty;
    
    public EditProfileCommand ToCommand(UserId userId)
    {
        return new EditProfileCommand
        {
            Id = userId,
            Avatar = Avatar,
            NickName = NickName,
            Introduction = Introduction
        };
    }
}