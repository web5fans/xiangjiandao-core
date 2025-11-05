using NetCorePal.Extensions.Primitives;

namespace Xiangjiandao.Domain.Enums;

/// <summary>
/// 稻米来源类型 0-未知 1-打赏 2-赠送 3-后台发放
/// </summary>
public enum ScoreSourceType{

    /// <summary>
    /// 未知
    /// </summary>
    Unknown = 0,
    
    /// <summary>
    /// 打赏
    /// </summary>
    Reward = 1,
    
    /// <summary>
    /// 赠送
    /// </summary>
    Send = 2,
    
    /// <summary>
    /// 后台发放
    /// </summary>
    AdminDistribution = 3,
}

public static class ScoreSourceTypeExtension{
    
    /// <summary>
    /// 获取描述
    /// </summary>
    /// <param name="scoreSourceType"></param>
    /// <returns></returns>
    /// <exception cref="KnownException"></exception>
    public static string GetDesc(this ScoreSourceType scoreSourceType)
    {
        return scoreSourceType switch
        {
            ScoreSourceType.Unknown => "未知",
            ScoreSourceType.Reward => "赞赏",
            ScoreSourceType.Send => "赠送",
            ScoreSourceType.AdminDistribution => "后台发放",
            _ => throw new KnownException("非法的稻米来源类型")

        };
    }
}