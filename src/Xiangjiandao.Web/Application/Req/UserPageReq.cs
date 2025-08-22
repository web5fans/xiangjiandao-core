
namespace Xiangjiandao.Web.Application.Req;

/// <summary>
/// 用户分页请求
/// </summary>
public record UserPageReq
{
    /// <summary>
    /// 用户昵称
    /// </summary> 
    public string NickName { get; set; } = string.Empty;

    /// <summary>
    /// DID
    /// </summary> 
    public string Did { get; set; } = string.Empty;
    
    
    /// <summary>
    /// 完整用户名，域名
    /// </summary> 
    public string DomainName { get; set; } = string.Empty;
    
    /// <summary>
    /// 邮箱
    /// </summary> 
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 手机号
    /// </summary> 
    public string Phone { get; set; } = string.Empty;
    
    /// <summary>
    ///  分页页码
    /// </summary>
    public int PageNum { get; set; } = 1;

    /// <summary>
    /// 分页大小
    /// </summary>
    public int PageSize { get; set; } = 10;
}