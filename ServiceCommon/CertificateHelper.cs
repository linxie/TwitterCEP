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
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;

    /// <summary>
    /// Provides various methods to deal with certificates and cert stores.
    /// </summary>
    public class CertificateHelper
    {
        /// <summary>
        /// Encodes a certificate with base-64 digits.
        /// </summary>
        /// <param name="certFilePath">The file path to find the certificate file.</param>
        /// <returns>The encoded certificate.</returns>
        public static string GetBase64StringEncodedCertFromFilePath(String certFilePath)
        {
            using (FileStream fs = File.Open(certFilePath, FileMode.Open))
            {
                byte[] rawCert = new byte[fs.Length];
                fs.Read(rawCert, 0, (int)fs.Length);
                return Convert.ToBase64String(rawCert);
            }
        }

        /// <summary>
        /// Adds a certificate to a cert store in the local machine.
        /// </summary>
        /// <param name="certificate">The file path to find the certificate file.</param>
        /// <param name="storeName">Name of the certificate store.</param>
        /// <param name="storeLocation">Location of the certificate store.</param>
        public static void AddCertificate(X509Certificate2 certificate, StoreName storeName, StoreLocation storeLocation)
        {
            X509Store store = null;

            try
            {
                store = new X509Store(storeName, storeLocation);
                store.Open(OpenFlags.ReadOnly | OpenFlags.ReadWrite);

                var certificates = from cert in store.Certificates.OfType<X509Certificate2>()
                                   where cert.Thumbprint == certificate.Thumbprint
                                   select cert;

                if (certificates.FirstOrDefault() == null)
                {
                    store.Add(certificate);
                    Console.WriteLine(string.Format("Added certificate with thumbprint {0} to store '{1}', has private key: {2}.", certificate.Thumbprint, storeName.ToString(), certificate.HasPrivateKey));

                    store.Close();
                    store = null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("AddCert exception storeName={0} storeLocation={1}", storeName.ToString(), storeLocation.ToString()), ex);
            }
            finally
            {
                if (store != null)
                {
                    store.Close();
                }
            }
        }

        /// <summary>
        /// Removes a certificate from a cert store in the local machine.
        /// </summary>
        /// <param name="certName">The name of the certificate file to remove.</param>
        /// <param name="storeName">Name of the certificate store.</param>
        /// <param name="storeLocation">Location of the certificate store.</param>
        public static void RemoveCertificate(string certName, StoreName storeName, StoreLocation storeLocation)
        {
            X509Store store = null;

            string certIssuerName = string.Format("CN={0}, OU=streaminsight, O=microsoft, C=us", certName);

            try
            {
                store = new X509Store(storeName, storeLocation);
                store.Open(OpenFlags.ReadOnly | OpenFlags.ReadWrite);
                var allCertificates = from cert in store.Certificates.OfType<X509Certificate2>()
                                      where cert.Issuer.Equals(certIssuerName)
                                      select cert;

                foreach (X509Certificate2 cert in allCertificates)
                {
                    store.Remove(cert);
                    Console.WriteLine(string.Format("Removed certificate with thumbprint {0} from store '{1}', has private key: {2}.", cert.Thumbprint, storeName.ToString(), cert.HasPrivateKey));
                }

                store.Close();
                store = null;
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Remove certificate hit exception, storeName={0} storeLocation={1}.", storeName.ToString(), storeLocation.ToString()), ex);
            }
            finally
            {
                if (store != null)
                {
                    store.Close();
                }
            }
        }
    }
}
