namespace Xiangjiandao.Web.Extensions;

public class CultureContext
{
    public const string ContextKey = "XiangjiandaoCulture";

    public CultureContext(string culture)
    {
        Culture = culture;
    }
    
    public string Culture { get; private set; }
}