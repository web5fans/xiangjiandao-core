using System.Diagnostics;
using System.Net;
using Yarp.ReverseProxy.Forwarder;

namespace Xiangjiandao.Web.Utils;

public class CustomForwarderHttpClientFactory(ILogger<CustomForwarderHttpClientFactory> logger)
    : IForwarderHttpClientFactory
{
    public HttpMessageInvoker CreateClient(ForwarderHttpClientContext context)
    {
        var handler = new SocketsHttpHandler
        {
            UseProxy = false,
            AllowAutoRedirect = false,
            AutomaticDecompression = DecompressionMethods.None,
            UseCookies = false,
            EnableMultipleHttp2Connections = true,
            ActivityHeadersPropagator = new ReverseProxyPropagator(DistributedContextPropagator.Current),
            ConnectTimeout = TimeSpan.FromSeconds(15),
        };

        return new LogHttpMessageHandler(handler, disposeHandler: true, logger);
    }
}

public class LogHttpMessageHandler(HttpMessageHandler handler, bool disposeHandler, ILogger logger)
    : HttpMessageInvoker(handler, disposeHandler)
{
    public override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var message = await base.SendAsync(request, cancellationToken);
        var log = await message.Content.ReadAsStringAsync(cancellationToken);
        logger.LogInformation("LogHttpMessageHandler: Response:{url},Content:{body}", request.RequestUri, log);
        return message;
    }
}