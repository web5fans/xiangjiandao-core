namespace Xiangjiandao.Web.Options;

public class AppOptions
{
    public bool UseEnvContext { get; set; } = false;
    public string Name { get; set; } = "xiangjiandao";
    public bool UseConsul { get; set; } = false;
    public bool UseSwagger { get; set; } = true;
    public bool UseHttpsRedirection { get; set; } = true;
    public bool UseDevTools { get; set; } = false;
    public bool UseK8sDiscovery { get; set; } = false;
    public bool UseSkyAPM { get; set; } = false;
    
    public bool UseFeiShu { get; set; } = true;
    public bool SkyAPMWriteData { get; set; } = false;
    public string ProductTenant { get; set; } = "xiangjiandao";
    
    public string DashBoardPathPrefix { get; set; } = "devops";
    public string PathPrefix { get; set; } = "xiangjiandao";
    public string ServiceNamespace { get; set; } = "xiangjiandao";

    public bool UseProxyProtocol { get; set; } = false;
    
    public string CurrentEnvironment { get; set; } = "dev";
    
    public string OfficialWebsite { get; set; } = string.Empty;
    
    public string OfficialEmail { get; set; } = string.Empty;
}