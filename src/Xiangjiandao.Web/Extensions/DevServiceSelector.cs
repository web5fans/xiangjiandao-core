using NetCorePal.Extensions.Primitives;
using NetCorePal.Extensions.ServiceDiscovery;

namespace Xiangjiandao.Web.Extensions;
/// <summary>
/// 服务选择器
/// </summary>
public class DevServiceSelector : IServiceSelector
{
    /// <summary>
    /// 根据服务名选择服务
    /// </summary>
    /// <param name="serviceName"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public IDestination? Find(string serviceName)
    {
        switch (serviceName)
        {
            case "users-core":
                return new Destination("users-core", "users-core", "https://localhost:7059", new Dictionary<string, string>());
            default:
                break;
        }

        throw new KnownException($"未配置名为 {serviceName} 的服务");
    }
}