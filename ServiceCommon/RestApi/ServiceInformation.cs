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
    /// Represents the information about a Austin service provided by the Master Service.
    /// </summary>
    [DataContract(Namespace = CloudManagementServiceHelper.ServiceNamespace)]
    public class ServiceInformation
    {
        /// <summary>
        /// Returns the current Austin version in use on the service.
        /// </summary>
        [DataMember(Order = 1)]
        public string StreamInsightVersion { get; set; }

        /// <summary>
        /// Returns the current user resources version in use on the service.
        /// </summary>
        [DataMember(Order = 2)]
        public string UserResourcesVersion { get; set; }

        /// <summary>
        /// Collection of URIs for the Austin service. The key describes the endpoint 
        /// while the value is the endpoint address (URI).
        /// </summary>
        [DataMember(Order = 3)]
        public IDictionary<string, string> StreamInsightUris { get; set; }
    }
}
