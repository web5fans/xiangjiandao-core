namespace Xiangjiandao.Domain;

public interface IUpdateAt
{
    /// <summary>
    /// 更新时间
    /// </summary> 
    public DateTimeOffset UpdatedAt { get; set; }
}