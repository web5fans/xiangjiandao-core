using Serilog.Core;
using Serilog.Events;
using SkyApm.Tracing;

namespace Xiangjiandao.Web.Logging;

public sealed class TraceIdEnricher : ILogEventEnricher
{
    public static void SetEntrySegmentContextAccessor(IEntrySegmentContextAccessor entrySegmentContextAccessor)
    {
        _entrySegmentContextAccessor = entrySegmentContextAccessor;
    }

    const string TraceIdPropertyName = "TraceId";
    static IEntrySegmentContextAccessor? _entrySegmentContextAccessor = null;

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (_entrySegmentContextAccessor != null)
        {
            var traceIdProperty = propertyFactory.CreateProperty(TraceIdPropertyName,
                _entrySegmentContextAccessor?.Context?.TraceId ?? string.Empty);
            logEvent.AddPropertyIfAbsent(traceIdProperty);
        }
    }
}