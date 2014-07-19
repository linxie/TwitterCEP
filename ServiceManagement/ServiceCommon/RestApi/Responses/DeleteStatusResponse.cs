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
    /// Response from the Austin management service to the delete status API.
    /// </summary>
    [DataContract(Namespace = CloudManagementServiceHelper.ServiceNamespace)]
    public class DeleteStatusResponse
    {
        /// <summary>
        /// The name of the service.
        /// </summary>
        [DataMember]
        public string ServiceName { get; set; }

        /// <summary>
        /// The current status of the deletion process.
        /// </summary>
        [DataMember]
        public string Status { get; set; }

        /// <summary>
        /// A message associated with the last status update.
        /// </summary>
        [DataMember]
        public string Message { get; set; }

        /// <summary>
        /// The time when the status was last updated.
        /// </summary>
        [DataMember]
        public DateTime LastUpdated { get; set; }
    }
}
