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

namespace StreamInsight.Samples.Austin.ServiceCommon
{
    using System;
    using System.Net;

    /// <summary>
    /// This class is a proxy for the various REST APIs exposed by the Austin Management Service.
    /// The methods of this class use only Http requests to communicate with the service.
    /// </summary>
    public class AustinManagementProxy
    {
        private readonly Uri _austinManagementUri;

        public AustinManagementProxy(Uri austinManagementUri)
        {
            _austinManagementUri = austinManagementUri;

            // TODO remove for official CMS
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
        }

        /// <summary>
        /// Provisions a new Austin service in the target hosted service
        /// It is a POST method that adds a new service.
        /// </summary>
        /// <param name="serviceName">Name of the Azure hosted service. This must be already created.</param>
        /// <param name="version">Version of Austin to install. If left empty or null, provisioning defaults to the latest version.</param>
        /// <param name="credentials">Credentials to access the user's hosted service and storage account.</param>
        /// <param name="computeVmSize">Size of the compute vm.</param>
        /// <param name="ingressConfig">Ingress pool configuration.</param>
        /// <returns>
        /// A provisioning response, containing information about how to access the new Austin instance.
        /// </returns>
        public CreateResponse Provision(string serviceName, string version, ServiceCredentials credentials, string computeVmSize, IngressConfiguration ingressConfig)
        {
            CreateRequest provRequest = new CreateRequest();

            provRequest.ServiceInstanceName = serviceName;
            provRequest.Credentials = credentials;
            provRequest.Version = version;
            provRequest.ComputeVmSize = computeVmSize;
            provRequest.IngressConfiguration = ingressConfig;

            Uri provisioningUri = new Uri(_austinManagementUri, "HostedServices");

            var apiProxy = new RestApiProxy<CreateRequest, CreateResponse>(provisioningUri);

            return apiProxy.Send(provRequest, HttpMethod.Post);
        }

        /// <summary>
        /// Retrieves the status of an ongoing Austin instance provisioning process.
        /// </summary>
        /// <param name="serviceName">Name of the Austin instance.</param>
        /// <returns>A provisioning status, containing information about the provisioning progress.</returns>
        public ProvisioningStatusResponse GetProvisioningStatus(string serviceName)
        {
            Uri provisioningStatusUri = new Uri(_austinManagementUri, string.Format("HostedServices/{0}/Provisioning", serviceName));

            var apiProxy = new RestApiProxy<Object, ProvisioningStatusResponse>(provisioningStatusUri);

            return apiProxy.Send(HttpMethod.Get);
        }

        /// <summary>
        /// Retrieves information about the Austin instance.
        /// </summary>
        /// <param name="serviceName">Name of the Austin instance.</param>
        /// <returns>Information about the Austin instance.</returns>
        public ServiceInformation GetServiceInformation(string serviceName)
        {
            Uri serviceStatusUri = new Uri(_austinManagementUri, string.Format("HostedServices/{0}", serviceName));

            var apiProxy = new RestApiProxy<Object, ServiceInformation>(serviceStatusUri);

            return apiProxy.Send(HttpMethod.Get);
        }

        /// <summary>
        /// Deletes an existing service.
        /// </summary>
        /// <param name="serviceName">The name of service to delete.</param>
        public void Delete(string serviceName)
        {
            Uri deleteUri = new Uri(_austinManagementUri, string.Format("HostedServices/{0}", serviceName));

            var apiProxy = new RestApiProxy<Object, DeleteResponse>(deleteUri);

            apiProxy.Send(HttpMethod.Delete);
        }

        /// <summary>
        /// Retrieves information about the progress of deleting an Austin instance.
        /// </summary>
        /// <param name="serviceName">Name of the instance being deleted.</param>
        /// <returns>Status information about the deletion process.</returns>
        public DeleteStatusResponse GetDeleteStatus(string serviceName)
        {
            Uri deleteStatusUri = new Uri(_austinManagementUri, string.Format("/HostedServices/{0}/Delete", serviceName));

            var apiProxy = new RestApiProxy<Object, DeleteStatusResponse>(deleteStatusUri);

            return apiProxy.Send(HttpMethod.Get);
        }

        /// <summary>
        /// Upgrades an existing service.
        /// </summary>
        /// <param name="serviceName">The name of service to upgrade.</param>
        /// <param name="version">The version to upgrade to.</param>
        public void Upgrade(string serviceName, string version)
        {
            Uri upgradeUri = new Uri(_austinManagementUri, string.Format("HostedServices/{0}/Upgrade", serviceName));

            var apiProxy = new RestApiProxy<Upgrade, UpgradeResponse>(upgradeUri);
            
            apiProxy.Send(new Upgrade { Version = version }, HttpMethod.Post);
        }

        /// <summary>
        /// Retrieves information about the progress of upgrading an Austin instance.
        /// </summary>
        /// <param name="serviceName">Name of the instance being upgraded.</param>
        /// <param name="version">Version that the instance is upgraded to.</param>
        /// <returns>Status information about the upgrade process.</returns>
        public UpgradeStatusResponse GetUpgradeStatus(string serviceName, string version)
        {
            Uri upgradeStatusUri = new Uri(_austinManagementUri, string.Format("/HostedServices/{0}/Upgrade/{1}", serviceName, version));

            var apiProxy = new RestApiProxy<Object, UpgradeStatusResponse>(upgradeStatusUri);

            return apiProxy.Send(HttpMethod.Get);
        }
    }
}
