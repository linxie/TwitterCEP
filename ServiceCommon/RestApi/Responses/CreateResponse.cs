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
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Response from the Austin management service to the provisioning API.
    /// </summary>
    [DataContract(Namespace = @"http://schemas.microsoft.com/ComplexEventProcessing/2010/12/Cloud/Provisioning", Name="CreateResult")]
    public class CreateResponse : IExtensibleDataObject
    {
        /// <summary>
        /// Service certificate that the client can use to authenticate the Austin instance.
        /// Contains the public key in the form of a X509 string encoded as base-64 digits.
        /// </summary>
        [DataMember(Order = 1)]
        public string ServiceCertificate { get; set; }

        /// <summary>
        /// Client certificate needed to connect to the Austin instance.
        /// Contains both public and private keys in the form of a X509 string encoded as base-64 digits.
        /// </summary>
        [DataMember(Order = 2)]
        public string ClientCertificate { get; set; }

        /// <summary>
        /// Client certificate password.
        /// </summary>
        [DataMember(Order = 3)]
        public string ClientCertificatePassword { get; set; }

        /// <summary>
        /// Collection of URIs for the new Austin instance.
        /// The key of each entity in the Uri describes the purpose of the URI while the value is the 
        /// URI itself.
        /// </summary>
        [DataMember(Order = 4)]
        public IDictionary<string, string> StreamInsightUris { get; set; }

        /// <summary>
        /// Data contract ExtensionData. If the service needs to return more data than the contract,
        /// it will return it in this property.
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }
   }
}
