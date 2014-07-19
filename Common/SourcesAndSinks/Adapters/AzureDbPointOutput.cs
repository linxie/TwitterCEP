using System;
using Microsoft.ComplexEventProcessing;
using Microsoft.ComplexEventProcessing.Adapters;

namespace StreamInsight.Demos.Twitter.Common
{
    /// <summary>
    /// Simple output adapter that writes quotes to console
    /// </summary>
    public class AzureDbPointOutput<T> : TypedPointOutputAdapter<T>
    {
        private DatabaseHelper _helper;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config">Adapter configuration</param>
        public AzureDbPointOutput(AzureDbConfig config)
        {
            _helper = new DatabaseHelper(config);
        }

        public override void Start()
        {
            ConsumeEvents();
        }

        public override void Resume()
        {
            ConsumeEvents();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        /// <summary>
        /// Main loop
        /// </summary>
        private void ConsumeEvents()
        {
            PointEvent<T> currEvent;
            DequeueOperationResult result;

            try
            {
                // Run until stop state
                while (AdapterState != AdapterState.Stopping)
                {
                    result = Dequeue(out currEvent);

                    // Take a break if queue is empty
                    if (result == DequeueOperationResult.Empty)
                    {
                        PrepareToResume();
                        Ready();
                        return;
                    }
                    else
                    {
                        // save to azure DB
                        if (currEvent.EventKind == EventKind.Insert)
                        {
                            Tweet t = currEvent.Payload as Tweet;
                                                                                   
                           
                             _helper.SaveTweet(t);
                            
                            Console.WriteLine("==================================");
                            Console.WriteLine(t.UserName);
                            Console.WriteLine("==================================");
                            Console.WriteLine();

                            
                        }

                        ReleaseEvent(ref currEvent);
                    }
                }

                result = Dequeue(out currEvent);
                PrepareToStop(currEvent, result);
                Stopped();
            }
            catch (AdapterException e)
            {
                Console.WriteLine(e.Message + e.StackTrace);
            }
        }

        private void PrepareToResume()
        {
        }

        private void PrepareToStop(PointEvent<T> currEvent, DequeueOperationResult result)
        {
            if (result == DequeueOperationResult.Success)
            {
                ReleaseEvent(ref currEvent);
            }
        }
    }
}
