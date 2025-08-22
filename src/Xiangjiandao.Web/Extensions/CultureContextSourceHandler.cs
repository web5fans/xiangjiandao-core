using System.Globalization;
using NetCorePal.Context;

namespace Xiangjiandao.Web.Extensions;

#pragma warning disable CS9113 // Parameter is unread.
public class CultureContextSourceHandler(IHttpContextAccessor httpContextAccessor) : IContextSourceHandler
#pragma warning restore CS9113 // Parameter is unread.
{

    public object? Extract(IContextSource source)
    {
        var cultureContext = source.Get(CultureContext.ContextKey);
        if (string.IsNullOrEmpty(cultureContext))
        {
            return null;
        }
        Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureContext);
        Thread.CurrentThread.CurrentUICulture = new CultureInfo(cultureContext);
        return  new CultureContext(cultureContext);
    }

    public Type ContextType => typeof(CultureContext);
}