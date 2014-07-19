//*********************************************************
//
//  Copyright (c) Microsoft. All rights reserved.
//  This code is licensed under the Apache 2.0 License.
//  THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OR
//  CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED,
//  INCLUDING, WITHOUT LIMITATION, ANY IMPLIED WARRANTIES
//  OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR
//  PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

namespace StreamInsight.Samples.Austin.Delete
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Threading;
    using StreamInsight.Samples.Austin.ServiceCommon;

    /// <summary>
    /// Demonstrates the usage of the Delete API of the Austin management service.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            if (!CheckIfConfigsAreReplaced())
            {
                Console.WriteLine("Press Enter to exit.");
                Console.ReadLine();
                return;
            }

            var austinManagementService = new AustinManagementProxy(new Uri(ConfigurationManager.AppSettings["AustinManagementUrl"]));

            string serviceInstanceName = ConfigurationManager.AppSettings["HostedServiceName"];

            Console.WriteLine("Starting the StreamInsight service delete process for service '{0}'.", serviceInstanceName);
            Console.WriteLine("This can take around 5min. ");
            Console.WriteLine();

            try
            {
                austinManagementService.Delete(serviceInstanceName);

                // Wait for delete to finish
                Console.Write("Delete request submitted, waiting for it to finish.");
                DeleteStatusResponse deleteStatus = austinManagementService.GetDeleteStatus(serviceInstanceName);
                while (deleteStatus.Status != "Completed" && deleteStatus.Status != "Failed")
                {
                    Console.Write(".");
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                    deleteStatus = austinManagementService.GetDeleteStatus(serviceInstanceName);
                }

                Console.WriteLine();

                if (deleteStatus.Status == "Completed")
                {
                    Console.WriteLine("Delete succeeded.");
                }
                else
                {
                    Console.WriteLine("Delete service failed. Error message: {0}", deleteStatus.Message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine();
            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();
        }

        /// <summary>
        /// Check whether the app.config has been edited.
        /// </summary>
        /// <returns>False if config parameters are left empty, true otherwise.</returns>
        private static bool CheckIfConfigsAreReplaced()
        {
            List<string> appConfigKeyList = new List<string>()
            {
                "HostedServiceName",
                "AustinManagementUrl",
            };

            foreach (string key in appConfigKeyList)
            {
                if (ConfigurationManager.AppSettings[key].Length == 0)
                {
                    Console.WriteLine("Please assign the configuration {0} in app.config with your configuration value.", key);
                    return false;
                }
            }

            return true;
        }
    }
}
