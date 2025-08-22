using FastEndpoints;
using Microsoft.Extensions.Options;
using NetCorePal.Extensions.Dto;
using NetCorePal.Extensions.Primitives;
using Refit;
using Xiangjiandao.Web.Clients;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Options;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.AdminPost;

/// <summary>
/// 下架贴文
/// </summary>
[Tags("AdminPost")]
[HttpPost("/api/v1/admin/post/take-off-post")]
[Microsoft.AspNetCore.Authorization.Authorize(PolicyNames.Admin)]
public class AdminTakeOffPostEndpoint(
    IPostClient postClient,
    IOptions<BlueSkyOptions> blueSkyOption
) : Endpoint<TakeOffPostReq, ResponseData<bool>>
{
    public override async Task HandleAsync(TakeOffPostReq req, CancellationToken ct)
    {
        try
        {
            var labelReq = new LabelReq
            {
                Uri = req.Uri,
                Labels = ["blacklist"]
            };

            await postClient.Label(labelReq, blueSkyOption.Value.AdminToken);

            await SendAsync(
                response: true.AsSuccessResponseData(),
                cancellation: ct
            );
        }
        catch (ApiException ex)
        {
            throw new KnownException($"Post 客户端请求错误，Message: {ex.Message}", ex);
        }
    }
}

/// <summary>
/// 下架贴文请求
/// </summary>
public class TakeOffPostReq
{
    /// <summary>
    /// Uri
    /// </summary>
    public required string Uri { get; set; }

    /// <summary>
    /// Cid
    /// </summary>
    public required string Cid { get; set; }
}