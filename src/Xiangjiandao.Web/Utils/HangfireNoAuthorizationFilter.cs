using Hangfire.Dashboard;

namespace Xiangjiandao.Web.Utils;

public class HangfireNoAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        return true;
    }
}