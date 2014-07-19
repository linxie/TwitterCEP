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
    /// Configuration of the ingress layer.
    /// </summary>
    [DataContract(Namespace = CloudManagementServiceHelper.ServiceNamespace)]
    public class IngressConfiguration
    {
        /// <summary>
        /// Gets or sets the number of instances of ingress role.
        /// </summary>
        /// <value>
        /// The number of instances.
        /// </value>
        [DataMember(Order = 1, IsRequired = true)]
        public int NumberOfInstances { get; set; }

        /// <summary>
        /// Gets or sets the size of the vm of ingress role.
        /// </summary>
        /// <value>
        /// The size of the vm.
        /// </value>
        [DataMember(Order = 2, IsRequired = true)]
        public string VmSize { get; set; }

        /// <summary>
        /// Gets or sets the data endpoints of ingress role.
        /// </summary>
        /// <value>
        /// The endpoints.
        /// </value>
        [DataMember(Order = 3, IsRequired = true)]
        public ICollection<IngressEndpoint> Endpoints { get; set; }
    }
}
