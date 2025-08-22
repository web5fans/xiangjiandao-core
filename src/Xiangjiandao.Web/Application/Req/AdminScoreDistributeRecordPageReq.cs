namespace Xiangjiandao.Web.Application.Req;

public record AdminScoreDistributeRecordPageReq
{
    /// <summary>
    /// 用户昵称
    /// </summary> 
    public string NickName { get; set; } = string.Empty;
    
    /// <summary>
    /// 手机号或邮箱
    /// </summary>
    public string PhoneOrEmail { get; set; } = string.Empty;
    
    /// <summary>
    /// 发放开始时间 yyyy-MM-dd
    /// </summary>
    public DateOnly? StartTime { get; set; } 

    /// <summary>
    /// 发放结束时间 yyyy-MM-dd
    /// </summary>
    public DateOnly? EndTime { get; set; } 
    
    /// <summary>
    ///  分页页码
    /// </summary>
    public int PageNum { get; set; } = 1;

    /// <summary>
    /// 分页大小
    /// </summary>
    public int PageSize { get; set; } = 10;
}