using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Xiangjiandao.Web.Tests.Extensions;

public static class JsonDefaults
{
    static JsonDefaults()
    {
        var p = new JsonSerializerSettings();
        p.Converters.Add(new NewtonsoftEntityIdJsonConverter());
        DefaultOptions = p;
    }

    public static JsonSerializerSettings DefaultOptions { get; }

    public static MediaTypeHeaderValue JsonMediaType() => new("application/json")
    {
        CharSet = "utf-8"
    };
}