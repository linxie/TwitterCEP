﻿//*********************************************************
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
    /// Body for the Upgrade POST API.
    /// </summary>
    [DataContract(Namespace = CloudManagementServiceHelper.ServiceNamespace, Name = "Upgrade")]
    public class Upgrade
    {
        /// <summary>
        /// Austin version to upgrade to.
        /// </summary>
        [DataMember(Name = "version")]
        public string Version { get; set; }
    }
}
