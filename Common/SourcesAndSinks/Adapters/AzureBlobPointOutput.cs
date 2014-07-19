using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.ComplexEventProcessing;
using Microsoft.ComplexEventProcessing.Adapters;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System.IO.MemoryMappedFiles;

namespace StreamInsight.Demos.Twitter.Common
{
   public class AzureBlobPOutput<T> : TypedPointOutputAdapter<T>
    {
       private BlobHelper _helper;

          public AzureBlobPOutput(AzureBlobCofig config)
        {
           _helper = new BlobHelper(config);
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
        /// /// <summary>
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
                        // save to azure Blob
                        if (currEvent.EventKind == EventKind.Insert)
                        {
                            Tweet t = currEvent.Payload as Tweet;
                            


                            //Console.WriteLine("****************");
                            //Console.WriteLine(t.Text);
                            //Console.WriteLine("==================================");
                            //Console.WriteLine();

                           _helper.SaveTweet(t);  
                            
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











