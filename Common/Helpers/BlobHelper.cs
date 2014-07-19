using System;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System.IO;
using System.Threading;



namespace StreamInsight.Demos.Twitter.Common
{
    public class BlobHelper
    {
        AzureBlobCofig _config;

        public BlobHelper(AzureBlobCofig config)
        {
            if (null == config || string.IsNullOrEmpty(config.StorageAccount) || string.IsNullOrEmpty(config.ContainerName))
                throw new ArgumentNullException("Missing database connection settings.  Check app.config file.");

            _config = config;
        }


        //************************************************

        public void SaveTweet(Tweet t)
        {
            if (t == null)
                return;

            try
            {
                // Variables for the cloud storage objects.
                var StorageAccount = CloudStorageAccount.Parse(_config.StorageAccount);

                // Create the blob client, which provides
                // authenticated access to the Blob service.
                var blobClient = StorageAccount.CreateCloudBlobClient();

                // Get the container reference.
                var blobContainer = blobClient.GetContainerReference(_config.ContainerName);
                // Create the container if it does not exist.
                blobContainer.CreateIfNotExist();
                string BlobName = string.Format("{0}/{1}.txt", t.CreatedAt.ToString("yyyy-MM-dd"), t.UserName + "_" + t.CreatedAt.ToString("yyyyMMddHHmmss"));

                var blob = blobContainer.GetBlockBlobReference(BlobName);

                //char[] CharArray = t.RawJson.ToCharArray();
                byte[] ByteArray = System.Text.Encoding.UTF8.GetBytes(t.RawJson);

                MemoryStream memStream = new MemoryStream(0);

                // Write the JSON String to the stream.
                memStream.Write(ByteArray, 0, ByteArray.Length);

                // Set the position to the beginning of the stream
                memStream.Seek(0, SeekOrigin.Begin);

                //blobClient.ParallelOperationThreadCount = 1;

                blob.BeginUploadFromStream(memStream, ProcessComplete, memStream);

                //blob.UploadFromStream(memStream);



                // blob.PutBlock(BlobName, memStream);





            }


            catch (Exception ex)
            {
                throw ex;
            }

        }

        static void ProcessComplete(IAsyncResult result)
        {
           return;
        }
        static void DeleteMEM(MemoryStream memStream)
        {
            memStream.Dispose();
        }
    }
}

    
