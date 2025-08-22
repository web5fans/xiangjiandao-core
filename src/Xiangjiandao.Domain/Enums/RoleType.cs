namespace Xiangjiandao.Domain.Enums;

/// <summary>
/// 角色类型 0-未知；1-管理员；2-运营人员
/// </summary>
public enum RoleType{

    /// <summary>
    /// 未知
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// 管理员
    /// </summary>
    Admin = 1,

    /// <summary>
    /// 运营人员
    /// </summary>
    Operator = 2,
}


public static class RoleTypeExtensions
{
    public static string ToRoleType(this RoleType roleType)
    {
        return roleType switch
        {
            RoleType.Admin => "admin",
            RoleType.Operator => "operator",
            _ => "Unknown"
        };
    }
}
