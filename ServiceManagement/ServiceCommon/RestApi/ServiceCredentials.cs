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
    /// Credentials required to provision a service in Azure. 
    /// </summary>
    [DataContract(Namespace = @"http://schemas.microsoft.com/ComplexEventProcessing/2010/12/Cloud")]
    public class ServiceCredentials : IExtensibleDataObject
    {
        /// <summary>
        /// Storage account name used for the service to be created.
        /// </summary>
        [DataMember(Order = 1, IsRequired = true)]
        public string StorageAccountName { get; set; }

        /// <summary>
        /// Storage account key used for the service to be created.
        /// </summary>
        [DataMember(Order = 2, IsRequired = true)]
        public string StorageAccountKey { get; set; }

        /// <summary>
        /// Id of the subscription to host the service to be created.
        /// </summary>
        [DataMember(Order = 3, IsRequired = true)]
        public string SubscriptionId { get; set; }

        /// <summary>
        /// Azure service management certificate for the hosted service.
        /// </summary>
        [DataMember(Order = 4, IsRequired = true)]
        public string Certificate { get; set; }

        /// <summary>
        /// Azure service management certificate password for the hosted service.
        /// </summary>
        [DataMember(Order = 5, IsRequired = true)]
        public string CertificatePassword { get; set; }

        /// <summary>
        /// Data contract ExtensionData. If the service needs to return more data than the contract,
        /// it will return it in this property.
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }
    }
}


