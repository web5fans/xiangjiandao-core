namespace Xiangjiandao.Web.Shared;

/// <summary>
/// 当前登录用户
/// </summary>
public interface ILoginUser
{
    /// <summary>
    /// 是否通过认证
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// 游客默认为 0
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// 邮箱
    /// </summary>
    public string Email { get; }

    /// <summary>
    /// Token
    /// </summary>
    public string Token { get; }
    
    /// <summary>
    /// Token 类型, admin 管理员， client 用户
    /// </summary>
    public string Type { get; }

    public string TokenWithPrefix()
    {
        return Token.StartsWith("Bearer ") ? Token : $"Bearer {Token}";
    }
}

public record LoginUser(Guid Id, string Email, string Token, bool IsAuthenticated, string Type) : ILoginUser;