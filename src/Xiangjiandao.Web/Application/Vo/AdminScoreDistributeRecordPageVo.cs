namespace Xiangjiandao.Web.Application.Vo;

public class AdminScoreDistributeRecordPageVo
{
    /// <summary>
    /// 用户昵称
    /// </summary> 
    public string NickName { get; set; } = string.Empty;
    
    /// <summary>
    /// 手机号
    /// </summary>
    public string Phone { get; set; } = string.Empty;
    
    /// <summary>
    /// 用户手机区号
    /// </summary> 
    public string PhoneRegion { get; set; } = string.Empty;
    
    /// <summary>
    /// 用户邮箱
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// 发放时间
    /// </summary>
    public DateTimeOffset GetTime { get; set; } = DateTimeOffset.MinValue;
    
    /// <summary>
    /// 发放稻米
    /// </summary> 
    public long Score { get; set; }
    
    /// <summary>
    /// 创建人
    /// </summary>
    public string CreatedBy { get; set; } = string.Empty;
}