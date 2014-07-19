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
    /// Empty response object returned from a successful delete request.
    /// Note that the success of the request does not guarantee the success of the deletion itself.
    /// </summary>
    [DataContract(Namespace = CloudManagementServiceHelper.ServiceNamespace, Name = "DeleteResponse")]
    public class DeleteResponse
    {  }
}
