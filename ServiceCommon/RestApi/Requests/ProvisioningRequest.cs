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
    using System.Runtime.Serialization;

    public class CloudManagementServiceHelper
    {
        public const string ServiceNamespace = @"http://schemas.microsoft.com/ComplexEventProcessing/2010/12/Cloud";
    }

    /// <summary>
    /// Body for the Create POST API.
    /// </summary>
    [DataContract(Namespace = CloudManagementServiceHelper.ServiceNamespace, Name = "Create")]
    public class CreateRequest
    {
        /// <summary>
        /// Name of the Azure hosted service to host the Austin instance.
        /// </summary>
        [DataMember(Order=1, Name="serviceName")]
        public string ServiceInstanceName { get; set; }

        /// <summary>
        /// Austin version to provision.
        /// </summary>
        [DataMember(Order = 2, Name = "version")]
        public string Version { get; set; }

        /// <summary>
        /// Credentials for the Azure hosted service and storage account.
        /// </summary>
        [DataMember(Order = 3, Name = "credentials")]
        public ServiceCredentials Credentials { get; set; }

        /// <summary>
        /// Size of the Austin compute VM.
        /// </summary>
        [DataMember(Order = 4, Name = "computeVmSize")]
        public string ComputeVmSize { get; set; }

        /// <summary>
        /// Austin ingress configuration.
        /// </summary>
        [DataMember(Order = 5, Name = "ingress")]
        public IngressConfiguration IngressConfiguration { get; set; }
    }
}
