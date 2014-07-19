using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fleck;
using Microsoft.ComplexEventProcessing;
using Microsoft.ComplexEventProcessing.Adapters;
using Newtonsoft.Json;

namespace StreamInsight.Demos.Twitter.Common
{
    public class WebSocketPointOutput<T> : TypedPointOutputAdapter<T>
    {
        private WebSocketServer server;
        private List<IWebSocketConnection> allSockets;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config">Adapter configuration</param>
        public WebSocketPointOutput(WebSocketConfig config)
        {
            FleckLog.Level = LogLevel.Debug;

            allSockets = new List<IWebSocketConnection>();
            server = new WebSocketServer(string.Format("ws://{0}:{1}", config.URL, config.SocketPort));

            server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    Console.WriteLine(String.Format("A web socket client at {0} has connected to this server ({1}).", socket.ConnectionInfo.ClientIpAddress, server.Location));
                    allSockets.Add(socket);
                };
                socket.OnClose = () =>
                {
                    allSockets.Remove(socket);
                };
                socket.OnMessage = message =>
                {
                    allSockets.ToList().ForEach(s => s.Send("Echo: " + message));
                };

            });
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
                        // this is where you do something with the event
                        if (currEvent.EventKind == EventKind.Insert)
                        {
                            var data = currEvent.Payload;
                            var json = JsonConvert.SerializeObject(data);
                            foreach (var socket in allSockets.ToList())
                            {
                                socket.Send(json);
                            }

                            //Console.WriteLine("==================================");
                            //Console.WriteLine(json);
                            //Console.WriteLine("==================================");
                            //Console.WriteLine();
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
                Console.WriteLine("Encountered exception - " + e.Message + e.StackTrace);
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
