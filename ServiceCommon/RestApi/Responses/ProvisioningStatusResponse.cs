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

    /// <summary>
    /// Response from the Austin management service to the provisioning status API.
    /// </summary>
    [DataContract(Namespace = @"http://schemas.microsoft.com/ComplexEventProcessing/2010/12/Cloud/Provisioning")]
    public class ProvisioningStatusResponse : IExtensibleDataObject
    {
        /// <summary>
        /// Name of the deployment created for the service.
        /// </summary>
        [DataMember(Order = 1)]
        public string DeploymentName { get; set; }

        /// <summary>
        /// Label for the deployment created.
        /// </summary>
        [DataMember(Order = 2)]
        public string DeploymentLabel { get; set; }

        /// <summary>
        /// Status code for the current provisioning state of the Austin service.
        /// </summary>
        [DataMember(Order = 3)]
        public string StatusCode { get; set; }

        /// <summary>
        /// A message providing additional information about the current provisioning state of the Austin service.
        /// </summary>
        [DataMember(Order = 4)]
        public string StatusMessage { get; set; }

        /// <summary>
        /// Data contract ExtensionData. If the service needs to return more data than the contract,
        /// it will return it in this property.
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }
    }
}
