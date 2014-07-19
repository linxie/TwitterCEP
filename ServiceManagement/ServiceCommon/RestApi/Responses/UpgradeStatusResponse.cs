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
    using System.Runtime.Serialization;

    /// <summary>
    /// Response from the Austin management service to the upgrade status API.
    /// </summary>
    [DataContract(Namespace = CloudManagementServiceHelper.ServiceNamespace)]
    public class UpgradeStatusResponse
    {
        /// <summary>
        /// The name of the service.
        /// </summary>
        [DataMember(Order = 1)]
        public string ServiceName { get; set; }

        /// <summary>
        /// The current status of the upgrade process.
        /// </summary>
        [DataMember(Order = 2)]
        public string Status { get; set; }

        /// <summary>
        /// A message associated with the last status update.
        /// </summary>
        [DataMember(Order = 3)]
        public string Message { get; set; }

        /// <summary>
        /// The time when the status was last updated.
        /// </summary>
        [DataMember(Order = 4)]
        public DateTime LastUpdated { get; set; }
    }
}
