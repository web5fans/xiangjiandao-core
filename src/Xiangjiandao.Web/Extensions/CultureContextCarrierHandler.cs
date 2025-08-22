using NetCorePal.Context;

namespace Xiangjiandao.Web.Extensions;

public class CultureContextCarrierHandler : IContextCarrierHandler
{

    public void Inject(IContextCarrier carrier, object? context)
    {
        if (context != null)
        {
            
            carrier.Set(CultureContext.ContextKey, ((CultureContext)context).Culture.ToString());
        }
    }

    public object? Initial()
    {
        return null;
    }

    public Type ContextType => typeof(CultureContext);
}