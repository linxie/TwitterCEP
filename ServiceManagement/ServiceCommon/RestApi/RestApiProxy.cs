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
    using System.Configuration;
    using System.IO;
    using System.Net;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.Text;
    using System.Xml;

    /// <summary>
    /// Represents a proxy for a REST http API. The URI provided to the constructor determines the URI of 
    /// a RESTful web service API to be called.
    /// </summary>
    /// <typeparam name="TRequest">Type of the request object.</typeparam>
    /// <typeparam name="TResponse">Type of the response object.</typeparam>
    public class RestApiProxy<TRequest, TResponse>
    {
        private readonly Uri _apiUri;

        public RestApiProxy(Uri apiUri)
        {
            _apiUri = apiUri;
        }

        /// <summary>
        /// Calls the respective API by submitting the serialized request object using the specified http method.
        /// </summary>
        /// <param name="request">The request object. Ignored for GET requests</param>
        /// <param name="httpRequestMethod">The http method for this API.</param>
        /// <returns>The response object for the respective API.</returns>
        public virtual TResponse Send(TRequest request, HttpMethod httpRequestMethod)
        {
            string serializedRequest = SerializeRequest(request);
            HttpWebResponse webResponse = SendRequest(serializedRequest, _apiUri, httpRequestMethod);
            return DeserializeResponse(webResponse);
        }

        /// <summary>
        /// Calls the respective API without a body using the specified http method.
        /// </summary>
        /// <param name="httpRequestMethod">The http method for this API.</param>
        /// <returns>The response object for the respective API.</returns>
        public virtual TResponse Send(HttpMethod httpRequestMethod)
        {
            HttpWebResponse webResponse = SendRequest(null, _apiUri, httpRequestMethod);
            return DeserializeResponse(webResponse);
        }

        private string SerializeRequest(TRequest request)
        {
            byte[] array;

            using (MemoryStream ms = new MemoryStream())
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(TRequest));
                serializer.WriteObject(ms, request);
                array = ms.ToArray();
            }

            return Encoding.UTF8.GetString(array, 0, array.Length);
        }

        private HttpWebResponse SendRequest(string requestBody, Uri apiUri, HttpMethod requestMethod)
        {
            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(apiUri);

            webRequest.KeepAlive = true;
            webRequest.Method = requestMethod.ToString().ToUpperInvariant();
            webRequest.Headers.Add("x-ms-streaminsight-version", "2010-12-31");
            webRequest.Headers.Add("x-ms-si-mgmtcertcontent", CertificateHelper.GetBase64StringEncodedCertFromFilePath(ConfigurationManager.AppSettings["ServiceManagementCertificateFilePath"]));
            webRequest.Headers.Add("x-ms-si-mgmtcertpassword", ConfigurationManager.AppSettings["ServiceManagementCertificatePassword"]);

            if (webRequest.Method == "POST" || webRequest.Method == "PUT")
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(requestBody); 
                webRequest.ContentType = @"application/xml";
                webRequest.ContentLength = byteArray.Length;

                using (Stream dataStream = webRequest.GetRequestStream())
                {
                    //write the data to the request stream. 
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();
                }
            }

            try
            {
                HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
                return response;
            }
            catch (WebException we)
            {
                string errorResponse;

                if (we.Response != null)
                {
                    using (StreamReader reader = new StreamReader(((HttpWebResponse)we.Response).GetResponseStream()))
                    {
                        // Extract the underlying cause of the exception that contains the error message
                        errorResponse = reader.ReadToEnd();
                    }

                    if (errorResponse.Length > 0)
                    {
                        if (we.Status == WebExceptionStatus.ProtocolError)
                        {
                            if (((System.Net.HttpWebResponse)(we.Response)).StatusCode == HttpStatusCode.NotFound)
                            {
                                throw new EndpointNotFoundException(we.ToString());
                            }
                        }
                        
                        throw new Exception(errorResponse);
                    }
                }

                throw;
            }
        }

        private TResponse DeserializeResponse(HttpWebResponse responseFromRestApi)
        {
            using (XmlReader reader = XmlReader.Create(responseFromRestApi.GetResponseStream()))
            {
                reader.MoveToContent();

                if (reader.IsEmptyElement)
                {
                    return default(TResponse);
                }

                string content = reader.ReadInnerXml();
                DataContractSerializer serializer = new DataContractSerializer(typeof(TResponse));
                TResponse response;
                
                using (XmlReader contentReader = XmlReader.Create(new StringReader(content)))
                {
                    response = (TResponse)serializer.ReadObject(contentReader, false);
                }

                return response;
            }
        }
    }
}
