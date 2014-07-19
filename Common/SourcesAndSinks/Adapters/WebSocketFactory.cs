using System;
using Microsoft.ComplexEventProcessing;
using Microsoft.ComplexEventProcessing.Adapters;

namespace StreamInsight.Demos.Twitter.Common
{
    /// <summary>
    /// Factory for instantiating of output adapter
    /// </summary>
    public class WebSocketFactory : ITypedOutputAdapterFactory<WebSocketConfig>
    {
        public OutputAdapterBase Create<TPayload>(WebSocketConfig configInfo, EventShape eventShape)
        {
            if (eventShape == EventShape.Point)
                return new WebSocketPointOutput<TPayload>(configInfo);
            else if (eventShape == EventShape.Interval)
                throw new NotImplementedException("Adapters for other event shapes haven't been implemented");
            else if (eventShape == EventShape.Edge)
                throw new NotImplementedException("Adapters for other event shapes haven't been implemented");
            else
                return default(OutputAdapterBase);
        }

        public void Dispose()
        {
        }
    }
}
