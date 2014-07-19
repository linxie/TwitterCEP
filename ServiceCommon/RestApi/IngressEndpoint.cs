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
    /// The class specifies an ingress endpoint.
    /// </summary>
    [DataContract(Namespace = CloudManagementServiceHelper.ServiceNamespace)]
    public class IngressEndpoint
    {
        /// <summary>
        /// Gets or sets the protocol of the endpoint. Only 'http' is currently supported.
        /// </summary>
        /// <value>
        /// The protocol.
        /// </value>
        [DataMember(Order = 1, IsRequired = true)]
        public string Protocol { get; set; }

        /// <summary>
        /// Gets or sets the port of the endpoint.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        [DataMember(Order = 2, IsRequired = true)]
        public int Port { get; set; }
    }
}
