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

namespace StreamInsight.Samples.Austin.ServiceProvisioning
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Security.Cryptography.X509Certificates;
    using System.ServiceModel;
    using StreamInsight.Samples.Austin.ServiceCommon;

    /// <summary>
    /// Sample for Provisioning an Austin service using the Austin management web service REST API.
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

            // The provisioning service uses a REST API, which we are wrapping in this sample in a proxy class for convenience.
            var austinManagementService = new AustinManagementProxy(new Uri(ConfigurationManager.AppSettings["AustinManagementUrl"]));

            var beginTime = DateTime.Now;

            try
            {
                string hostedService = ConfigurationManager.AppSettings["HostedServiceName"];

                ServiceInformation serviceInfo = null;

                // The Austin instance will have the same name as the Azure hosted service it is hosted in.
                // Check if an Austin instance with this name already exists
                Console.WriteLine("Checking for existence of Austin instance '{0}'...", hostedService);

                try
                {
                    // If the service doesn't exist, we get an 'endpoint not found' exception.
                    // Bubble up all other exceptions.
                    serviceInfo = austinManagementService.GetServiceInformation(hostedService);
                }
                catch (EndpointNotFoundException) { }

                // If no instance with this name exists, provision it.
                if (serviceInfo == null)
                {
                    Console.WriteLine("Austin instance {0} does not exist. Proceeding with provisioning.", hostedService);
                    Console.WriteLine();

                    Provision(ref austinManagementService, hostedService);
                }
                else
                {
                    Console.WriteLine("Austin instance {0} already exists.", hostedService);
                    Console.WriteLine("Version of current service: {0}", serviceInfo.StreamInsightVersion);
                    Console.WriteLine("User resources version: {0}", serviceInfo.UserResourcesVersion);
                    Console.WriteLine("Service endpoints:");

                    foreach (KeyValuePair<string, string> item in serviceInfo.StreamInsightUris)
                    {
                        Console.WriteLine("  {0}: {1}", item.Key, item.Value);
                    }

                    if (String.Compare(serviceInfo.StreamInsightVersion, "20120622221345.0382744") < 0)
                    {
                        Console.WriteLine();
                        Console.WriteLine("A more recent Austin version is available. Please delete your service and re-provision.");
                    }
                }

                Console.WriteLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            var elapsedTime = DateTime.Now - beginTime;

            Console.WriteLine("The process took {0}h {1}m {2}s to finish.", elapsedTime.Hours, elapsedTime.Minutes, elapsedTime.Seconds);
            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();
        }

        /// <summary>
        /// Provisioning of a new Austin instance in Windows Azure.
        /// </summary>
        /// <param name="proxy">Austin management web service proxy.</param>
        /// <param name="serviceInstanceName">Name of the Azure hosted service to host Austin.</param>
        private static void Provision(ref AustinManagementProxy proxy, string serviceInstanceName)
        {
            #region hosted service credentials
            // Credentials needed to access the hosted service in the user's subscription.
            // note that this is the provisioning model in the Austin CTP. Eventually, this will go away when Austin
            // becomes a multi-tenancy service.
            ServiceCredentials siCred = new ServiceCredentials();
            siCred.StorageAccountKey = ConfigurationManager.AppSettings["StorageAccountKey"];
            siCred.StorageAccountName = ConfigurationManager.AppSettings["StorageAccountName"];
            siCred.SubscriptionId = ConfigurationManager.AppSettings["SubscriptionId"];
            siCred.Certificate = CertificateHelper.GetBase64StringEncodedCertFromFilePath(ConfigurationManager.AppSettings["ServiceManagementCertificateFilePath"]);
            siCred.CertificatePassword = ConfigurationManager.AppSettings["ServiceManagementCertificatePassword"];
            #endregion

            string computeVmSize = ConfigurationManager.AppSettings["ComputeVmSize"];
            var ingressConfig = new IngressConfiguration
                {
                    VmSize = ConfigurationManager.AppSettings["IngressVmSize"],
                    NumberOfInstances = int.Parse(ConfigurationManager.AppSettings["NumberOfIngressInstances"]),
                    Endpoints = new List<IngressEndpoint>
                        {
                            new IngressEndpoint
                                {
                                    Protocol = ConfigurationManager.AppSettings["IngressProtocol"],
                                    Port = int.Parse(ConfigurationManager.AppSettings["IngressEndpointPort"])
                                }
                        }
                };

            Console.WriteLine("Starting the Austin service provisioning process.");
            Console.WriteLine("This step wraps an Azure hosted service deployment and can therefore take around 15min.");
            Console.WriteLine("You can also check the provisioning status on the Azure management portal.");
            Console.WriteLine("If the provisioning process does not exit even after the instance shows \"Ready\" in the management portal, you can safely break.");
            Console.WriteLine();

            // Create the Austin Instance. This is a REST API and all communication happens over HTTP
            // The API is async and will return after validating the input and copying some files
            // The response contains the status after validation and the URI of the Austin instance 
            // that will be provisioned. 
            CreateResponse response = proxy.Provision(serviceInstanceName, null, siCred, computeVmSize, ingressConfig);

            Console.WriteLine("Provisioning of Austin into Azure Hosted Service '{0}' kicked off.", serviceInstanceName);

            Console.Write("The provisioning call returned client and service certificates. ");
            Console.Write("They will now be installed into the machine's store, in order to enable clients to connect to the Austin instance. ");
            Console.WriteLine("This only succeeds if the app is run as admin!");
            AddServiceAndClientCertsToStore(response);

            // Get the status of the provisioning operation
            string newStatusCode = proxy.GetProvisioningStatus(serviceInstanceName).StatusCode;
            string statusCode = null;

            // Check status until provisioning succeeded.
            while ((newStatusCode != "Ready") && (newStatusCode != "Failed"))
            {
                if (newStatusCode != statusCode)
                {
                    Console.WriteLine();
                    Console.Write(newStatusCode + " ");
                    statusCode = newStatusCode;
                }
                else
                {
                    Console.Write(".");
                }

                // Azure takes around 15 minutes to provision and deploy any new service. 
                // So keep checking until the provisioned Austin instances is ready for connection
                System.Threading.Thread.Sleep(5000);
                newStatusCode = proxy.GetProvisioningStatus(serviceInstanceName).StatusCode;
            }

            Console.WriteLine();

            if (newStatusCode == "Ready")
            {
                //This is the URI for the Austin instance that was provisioned by the provisioning service
                string siInstanceUri = response.StreamInsightUris["StreamInsightManagementEndpoint"];

                Console.WriteLine("Provisioning succeeded.");
                Console.WriteLine();

                Console.WriteLine("Service endpoints:");

                var serviceInfo = proxy.GetServiceInformation(serviceInstanceName);

                foreach (KeyValuePair<string, string> item in serviceInfo.StreamInsightUris)
                {
                    Console.WriteLine("  {0}: {1}", item.Key, item.Value);
                }
            }
            else
            {
                Console.WriteLine("Provisioning Failed!");
            }
        }

        /// <summary>
        /// Adds the client and service certificate that the provisioning call returned to the local machine's certificate store.
        /// Also, the certificates will be saved as files in the executable's folder, so that they can be installed
        /// on other machines that need to connect to the newly provisioned Austin instance.
        /// </summary>
        /// <param name="response">Provisioning response including the certificates.</param>
        private static void AddServiceAndClientCertsToStore(CreateResponse response)
        {
            // Remove existing certificates from store
            string serviceName = ConfigurationManager.AppSettings["HostedServiceName"];
            string clientCertName = string.Format("StreamInsight Client ({0})", serviceName);
            string serviceCertName = string.Format("{0}.cloudapp.net", serviceName);

            Console.WriteLine("Removing old certificates from local store...");
            CertificateHelper.RemoveCertificate(clientCertName, StoreName.My, StoreLocation.CurrentUser);
            CertificateHelper.RemoveCertificate(serviceCertName, StoreName.TrustedPeople, StoreLocation.CurrentUser);

            // Add new certificates
            Console.WriteLine("Adding certificates to local store...");
            byte[] clientRawCert = Convert.FromBase64String(response.ClientCertificate);
            byte[] serviceRawCert = Convert.FromBase64String(response.ServiceCertificate);
            X509Certificate2 clientCert = new X509Certificate2(clientRawCert, response.ClientCertificatePassword, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);
            X509Certificate2 serviceCert = new X509Certificate2(serviceRawCert);
            CertificateHelper.AddCertificate(clientCert, StoreName.My, StoreLocation.CurrentUser);
            CertificateHelper.AddCertificate(serviceCert, StoreName.TrustedPeople, StoreLocation.CurrentUser);

            // Save certificates to file
            string clientCertFileName = serviceName + "_client.pfx";
            string serviceCertFileName = serviceName + "_service.cer";
            File.WriteAllBytes(clientCertFileName, clientCert.Export(X509ContentType.Pfx, ConfigurationManager.AppSettings["ClientCertificatePassword"]));
            File.WriteAllBytes(serviceCertFileName, serviceCert.Export(X509ContentType.Cert));
            Console.WriteLine("Client Certificate also saved as {0}.", clientCertFileName);
            Console.WriteLine("Service Certificate also saved as {0}.", serviceCertFileName);
        }

        /// <summary>
        /// Check whether the app.config has been edited.
        /// </summary>
        /// <returns>False if config parameters are left empty, true otherwise.</returns>
        private static bool CheckIfConfigsAreReplaced()
        {
            List<string> appConfigKeyList = new List<string>()
            {
                "SubscriptionId", 
                "HostedServiceName", 
                "StorageAccountName",
                "StorageAccountKey",
                "ServiceManagementCertificateFilePath",
                "ServiceManagementCertificatePassword",
                "ClientCertificatePassword",
                "ComputeVmSize",
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
