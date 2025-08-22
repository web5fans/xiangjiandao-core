using Newtonsoft.Json;
using Refit;

namespace Xiangjiandao.Web.Clients;

/// <summary>
/// 贴文客户端
/// </summary>
public interface IPostClient
{
    /// <summary>
    /// 给帖子打标签
    /// </summary>
    [Post("/api/posts/label")]
    [Headers("Content-Type: application/json")]
    Task<LabelResp> Label(
        [Body] LabelReq req,
        [Header("Authorization")] string authorization
    );

    /// <summary>
    /// 查询帖子是否被下架
    /// </summary>
    [Post("/api/posts/is_banned")]
    [Headers("Content-Type: application/json")]
    Task<IsBannedResp> IsBanned(
        [Body] List<string> uriList,
        [Header("Authorization")] string authorization
    );
}

/// <summary>
/// 是否被下架响应
/// </summary>
public class IsBannedResp
{
    /// <summary>
    /// 响应状态码
    /// </summary>
    [JsonProperty("code")]
    public required int Code { get; set; }

    /// <summary>
    /// 响应数据
    /// </summary>
    [JsonProperty("data")]
    public required List<PostIsBannedInfo> Data { get; set; }

    /// <summary>
    /// 帖子是否被下架信息
    /// </summary>
    public class PostIsBannedInfo
    {
        /// <summary>
        /// 帖子连接
        /// </summary>
        [JsonProperty("uri")]
        public required string Uri { get; set; }

        /// <summary>
        /// 帖子是否已下架
        /// </summary>
        [JsonProperty("is_banned")]
        public required bool IsBanned { get; set; }

        /// <summary>
        /// 原因
        /// </summary>
        [JsonProperty("msg")]
        public required string Msg { get; set; }
    }
}

/// <summary>
/// 打标签请求
/// </summary>
public class LabelReq
{
    /// <summary>
    /// 帖子 Uri
    /// </summary>
    [JsonProperty("uri")]
    public required string Uri { get; set; }

    /// <summary>
    /// 标签列表
    /// </summary>
    [JsonProperty("labels")]
    public required List<string> Labels { get; set; }
}

/// <summary>
/// 打标签响应
/// </summary>
public class LabelResp
{
}