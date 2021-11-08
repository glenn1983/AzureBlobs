using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using System;
using System.IO;

namespace AzureBlobs
{
    class Program
    {
       static string connectionstring = "DefaultEndpointsProtocol=https;AccountName=storeoneaccount1000;AccountKey=fiawCMpwwPFYFCUGeE/y7/3gyLUKT7fpPcnQ9xMdjbzyIgLgmJHx0FkRk93tsMssGCWULeBSaDUi+/z1m26T3Q==;EndpointSuffix=core.windows.net";
        static void Main(string[] args)
        {
             //AddBlob();
              //ReadAllBlobs();
            //DownloadBlob();
           // WorkingWithLease();
        }


        public static void ReadAllBlobs()
        {
            //Connect to the blob Service Client using Access Keys
            BlobServiceClient client = new BlobServiceClient(connectionstring);
            //Connect to the container
            var blobContainerClient = client.GetBlobContainerClient("data");
            //Connect to the blob objects
            foreach (BlobItem blob in blobContainerClient.GetBlobs())
            {
                Console.WriteLine(blob.Name);
            }
            Console.ReadLine();
        }

        public static void AddBlob()
        {
            //Connect to the blob Service Client using Access Keys
            BlobServiceClient client = new BlobServiceClient(connectionstring);
            //Connect to the container
            var blobContainerClient = client.GetBlobContainerClient("data");
            //Connect to the blob objects
            BlobClient blob_client = blobContainerClient.GetBlobClient("Test.txt");
            blob_client.Upload(@"E:\AzureLearning\AzureBlobs\AzureBlobs\Upload\Test.txt");
            Console.ReadLine();
        }


        /// <summary>
        /// Generate Shared AccessSignature (SAS) URI for the BlobClient
        /// </summary>
        /// <returns></returns>
        public  static Uri GenerateSAS()
        {
            BlobServiceClient client = new BlobServiceClient(connectionstring);
            BlobContainerClient blob_ContainerClient = client.GetBlobContainerClient("data");
            BlobClient blob_client= blob_ContainerClient.GetBlobClient("background-image.png");

            BlobSasBuilder sas_builder = new BlobSasBuilder()
            {
                BlobContainerName = blob_ContainerClient.Name,
                BlobName = blob_client.Name,
                Resource="b"
            };
            sas_builder.SetPermissions(BlobAccountSasPermissions.Read | BlobAccountSasPermissions.List);
            sas_builder.ExpiresOn = DateTimeOffset.UtcNow.AddDays(1);
            return blob_client.GenerateSasUri(sas_builder);
        }

        public static void DownloadBlob()
        {
            string downloadpath = @"E:\AzureLearning\AzureBlobs\AzureBlobs\Downloads\Test.txt";
            Uri blob_uri = GenerateSAS();
            BlobClient blobClient = new BlobClient(blob_uri);
            blobClient.DownloadTo(downloadpath);
        }

        public static void WorkingWithLease()
        {
            BlobServiceClient client = new BlobServiceClient(connectionstring);
            BlobContainerClient blobContainerClient = client.GetBlobContainerClient("data");
            BlobClient blobClient = blobContainerClient.GetBlobClient("Test.txt");
            BlobLeaseClient blobLeaseClient = blobClient.GetBlobLeaseClient();
          
            BlobLease blobLease = blobLeaseClient.Acquire(TimeSpan.FromSeconds(30));

            BlobUploadOptions blobUploadOptions = new BlobUploadOptions()
            {
                Conditions = new BlobRequestConditions() { LeaseId = blobLease.LeaseId }
            };


            MemoryStream memoryStream = new MemoryStream();
            blobClient.DownloadTo(memoryStream);
            memoryStream.Position = 0;
            StreamReader streamReader = new StreamReader(memoryStream);
            Console.WriteLine(streamReader.ReadToEnd());
            StreamWriter streamWriter = new StreamWriter(memoryStream);
            streamWriter.WriteLine("Good Day");
            streamWriter.Flush();
            memoryStream.Position = 0;
            // blobClient.Upload(memoryStream, true);
            blobClient.Upload(memoryStream, blobUploadOptions);
            blobLeaseClient.Release();
            Console.ReadLine();
        }
    }
}
