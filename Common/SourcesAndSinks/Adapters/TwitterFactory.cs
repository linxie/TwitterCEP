using System;
using Microsoft.ComplexEventProcessing;
using Microsoft.ComplexEventProcessing.Adapters;

namespace StreamInsight.Demos.Twitter.Common
{
    /// <summary>
    /// Factory for instantiating of input adapter
    /// </summary>
    public class TwitterFactory : ITypedInputAdapterFactory<TwitterConfig>
    {
        public InputAdapterBase Create<TPayload>(TwitterConfig config, EventShape eventShape)
        {
            // Only support the point event model
            if (eventShape == EventShape.Point)
                return new TwitterInput(config);
            else if (eventShape == EventShape.Interval)
                throw new NotImplementedException("Adapters for other event shapes haven't been implemented");
            else if (eventShape == EventShape.Edge)
                throw new NotImplementedException("Adapters for other event shapes haven't been implemented");
            else
                return default(InputAdapterBase);
        }

        public void Dispose()
        {
        }
    }
}
