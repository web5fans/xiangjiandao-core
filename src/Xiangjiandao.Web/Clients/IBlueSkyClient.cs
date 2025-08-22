using Newtonsoft.Json;
using Refit;

namespace Xiangjiandao.Web.Clients;

/// <summary>
/// BlueSky 客户端
/// </summary>
public interface IBlueSkyClient
{
    /// <summary>
    /// 创建账户
    /// </summary>
    [Post("/xrpc/com.atproto.server.createAccount")]
    [Headers("Content-Type: application/json")]
    Task<CreateAccountResp> CreateAccount(
        [Body] CreateAccountReq req
    );

    /// <summary>
    /// 获取访问 Token
    /// </summary>
    [Post("/xrpc/com.atproto.server.createSession")]
    [Headers("Content-Type: application/json")]
    Task<CreateSessionResp> CreateSession(
        [Body] CreateSessionReq req
    );

    /// <summary>
    /// 删除账户
    /// </summary>
    [Post("/xrpc/com.atproto.admin.deleteAccount")]
    [Headers("Content-Type: application/json")]
    Task DeleteAccount(
        [Body] DeleteAccountReq req,
        [Header("Authorization")] string authorization
    );

    /// <summary>
    /// 重置账户密码
    /// </summary>
    [Post("/xrpc/com.atproto.admin.updateAccountPassword")]
    [Headers("Content-Type: application/json")]
    Task UpdateAccountPassword(
        [Body] UpdateAccountPasswordReq req,
        [Header("Authorization")] string authorization
    );

    /// <summary>
    /// 给贴文打上标签
    /// </summary>
    [Post("/xrpc/tools.ozone.moderation.emitEvent")]
    [Headers("Content-Type: application/json")]
    Task<EmitEventResp> EmitEvent(
        [Body] EmitEventReq req,
        [Header("Authorization")] string authorization,
        [Header("Atproto-Proxy")] string atprotoProxy,
        [Header("atproto-accept-labelers")] string atprotoAcceptLabelers
    );
}

/// <summary>
/// EmitEvent 请求
/// </summary>
public class EmitEventReq
{
    /// <summary>
    /// Subject
    /// </summary>
    [JsonProperty("subject")]
    public required SubjectModel Subject { get; set; }

    /// <summary>
    /// 创建着
    /// </summary>
    [JsonProperty("createdBy")]
    public required string CreatedBy { get; set; }

    /// <summary>
    /// SubjectBlobCids
    /// </summary>
    [JsonProperty("subjectBlobCids")]
    public List<string> SubjectBlobCids { get; set; } = [];

    /// <summary>
    /// Event
    /// </summary>
    [JsonProperty("event")]
    public required EventModel Event { get; set; }
}

/// <summary>
/// EmitEvent 响应
/// </summary>
public class EmitEventResp
{
    /// <summary>
    /// Id
    /// </summary>
    [JsonProperty("id")]
    public int Id { get; set; }

    /// <summary>
    /// Subject
    /// </summary>
    [JsonProperty("subject")]
    public required SubjectModel Subject { get; set; }

    /// <summary>
    /// 创建着
    /// </summary>
    [JsonProperty("createdBy")]
    public required string CreatedBy { get; set; }

    /// <summary>
    /// SubjectBlobCids
    /// </summary>
    [JsonProperty("subjectBlobCids")]
    public List<string> SubjectBlobCids { get; set; } = [];

    /// <summary>
    /// Event
    /// </summary>
    [JsonProperty("event")]
    public required EventModel Event { get; set; }
}

/// <summary>
/// SubjectModeType
/// </summary>
public static class SubjectModeType
{
    /// <summary>
    /// RepoRef 常量
    /// </summary>
    public static readonly string RepoRef = "com.atproto.admin.defs#repoRef";

    /// <summary>
    /// StrongRef 常量
    /// </summary>
    public static readonly string StrongRef = "com.atproto.repo.strongRef";
}

/// <summary>
/// Subject 模型
/// </summary>
public class SubjectModel
{
    /// <summary>
    /// 类型
    /// </summary>
    [JsonProperty("$type")]
    public required string Type { get; set; }

    /// <summary>
    /// Did
    /// </summary>
    [JsonProperty("did")]
    public string? Did { get; set; }
    
    /// <summary>
    /// Uri
    /// </summary>
    [JsonProperty("uri")]
    public string? Uri { get; set; }
    
    /// <summary>
    /// Cid
    /// </summary>
    [JsonProperty("cid")]
    public string? Cid { get; set; }
}

/// <summary>
/// EventModelType
/// </summary>
public static class EventModelType
{
    /// <summary>
    /// ModEventLabel
    /// </summary>
    public static readonly string ModEventLabel = "tools.ozone.moderation.defs#modEventLabel";
}

/// <summary>
/// Event 模型
/// </summary>
public class EventModel
{
    /// <summary>
    /// 类型
    /// </summary>
    [JsonProperty("$type")]
    public required string Type { get; set; }

    /// <summary>
    /// 说明
    /// </summary>
    [JsonProperty("comment")]
    public string Comment { get; set; } = string.Empty;

    /// <summary>
    /// CreateLabelVals
    /// </summary>
    [JsonProperty("createLabelVals")]
    public List<string> CreateLabelVals { get; set; } = [];

    /// <summary>
    /// NegateLabelVals
    /// </summary>
    [JsonProperty("negateLabelVals")]
    public List<string> NegateLabelVals { get; set; } = ["impersonation"];

    /// <summary>
    /// DurationInHours
    /// </summary>
    [JsonProperty("durationInHours")]
    public int DurationInHours { get; set; } = 0;
}

/// <summary>
/// 请求对象：用于创建 AT Protocol 账户
/// </summary>
public class CreateAccountReq
{
    /// <summary>
    /// 用户注册邮箱地址
    /// 示例: "zhushuliang@rivtower.com"
    /// </summary>
    [JsonProperty("email")]
    public required string Email { get; set; }

    /// <summary>
    /// 用户设置的密码
    /// 示例: "12345678"
    /// </summary>
    [JsonProperty("password")]
    public required string Password { get; set; }

    /// <summary>
    /// 用户自定义的 handle，全局唯一标识符
    /// 示例: "yunus3.web5.rivtower.cc"
    /// </summary>
    [JsonProperty("handle")]
    public required string Handle { get; set; }

    /// <summary>
    /// 邀请码（可选，空字符串表示无邀请码）
    /// 示例: ""
    /// </summary>
    [JsonProperty("inviteCode")]
    public string InviteCode { get; set; } = string.Empty;
}

/// <summary>
/// 请求对象：用于创建会话（登录）
/// </summary>
public class CreateSessionReq
{
    /// <summary>
    /// 用户标识符，可以是 handle 或 email
    /// 示例: "yunus.web5.rivtower.cc"
    /// </summary>
    [JsonProperty("identifier")]
    public required string Identifier { get; set; }

    /// <summary>
    /// 用户密码
    /// 示例: "@WorkXT0621"
    /// </summary>
    [JsonProperty("password")]
    public required string Password { get; set; }

    /// <summary>
    /// 多因素认证 Token（可为空）
    /// 示例: ""
    /// </summary>
    [JsonProperty("authFactorToken")]
    public required string AuthFactorToken { get; set; }

    /// <summary>
    /// 是否允许账户处于被封禁状态登录
    /// 示例: true
    /// </summary>
    [JsonProperty("allowTakendown")]
    public required bool AllowTakendown { get; set; }
}

/// <summary>
/// 更新账户密码请求
/// </summary>
public class UpdateAccountPasswordReq
{
    /// <summary>
    /// 用户 Did
    /// </summary>
    [JsonProperty("did")]
    public required string Did { get; set; }

    /// <summary>
    /// 设置的新密码
    /// </summary>
    [JsonProperty("password")]
    public required string Password { get; set; }
}

/// <summary>
/// 删除账户请求
/// </summary>
public class DeleteAccountReq
{
    /// <summary>
    /// 用户 Did
    /// </summary>
    public required string Did { get; set; }
}

/// <summary>
/// 登录会话返回结果
/// </summary>
public class CreateSessionResp
{
    /// <summary>
    /// 访问令牌（JWT 格式）
    /// </summary>
    [JsonProperty("accessJwt")]
    public required string AccessJwt { get; set; }

    /// <summary>
    /// 刷新令牌（JWT 格式），用于获取新的访问令牌
    /// </summary>
    [JsonProperty("refreshJwt")]
    public required string RefreshJwt { get; set; }

    /// <summary>
    /// 用户句柄（handle），唯一标识符
    /// 示例: "yunus.web5.rivtower.cc"
    /// </summary>
    [JsonProperty("handle")]
    public required string Handle { get; set; }

    /// <summary>
    /// 分布式身份标识（DID）
    /// </summary>
    [JsonProperty("did")]
    public required string Did { get; set; }

    /// <summary>
    /// DID 文档，描述分布式身份的元信息（可为空）
    /// </summary>
    [JsonProperty("didDoc")]
    public object DidDoc { get; set; } = new();

    /// <summary>
    /// 用户注册邮箱
    /// 示例: "yunus@web5.rivtower.cc"
    /// </summary>
    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 邮箱是否已验证
    /// </summary>
    [JsonProperty("emailConfirmed")]
    public bool EmailConfirmed { get; set; }

    /// <summary>
    /// 是否启用邮箱作为多因素认证方式
    /// </summary>
    [JsonProperty("emailAuthFactor")]
    public bool EmailAuthFactor { get; set; }

    /// <summary>
    /// 账户是否处于激活状态
    /// </summary>
    [JsonProperty("active")]
    public bool Active { get; set; }

    /// <summary>
    /// 账户当前状态
    /// </summary>
    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// 创建账户接口返回结果
/// </summary>
public class CreateAccountResp
{
    /// <summary>
    /// 访问令牌（JWT 格式）
    /// </summary>
    [JsonProperty("accessJwt")]
    public required string AccessJwt { get; set; }

    /// <summary>
    /// 刷新令牌（JWT 格式），用于获取新的访问令牌
    /// </summary>
    [JsonProperty("refreshJwt")]
    public required string RefreshJwt { get; set; }

    /// <summary>
    /// 用户句柄（handle），唯一标识符
    /// 示例: "yunus3.web5.rivtower.cc"
    /// </summary>
    [JsonProperty("handle")]
    public required string Handle { get; set; }

    /// <summary>
    /// 分布式身份标识（DID）
    /// </summary>
    [JsonProperty("did")]
    public required string Did { get; set; }

    /// <summary>
    /// DID 文档，描述分布式身份的元信息（可为空）
    /// </summary>
    [JsonProperty("didDoc")]
    public object DidDoc { get; set; } = new();
}