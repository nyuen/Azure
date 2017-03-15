using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BlobDemo
{

    
    public class BlopOps
    {
        const string storageAccountName = "azureenablement";
        const string storageAccountKey = "CQyibzFhGKmzu+IGALo/+jQQcquZwf0rvxwm/v/xYwuQ+zg08O5NmZBTjwqnfVtumHSMB+9FTTQUIACFspHLow==";
        string containerName = "blobops";
        const string localPicsToUploadPath = @"C:\AzureDemo";
        string localDownloadPath;
        const string pseudoFolder = "images/";

        const string image1Name = "image1.png";
        const string image2Name = "image2.png";
        const string image3Name = "image3.jpg";
        const string image4Name = "image4.jpg";
        const string textFileName = "text.txt";

        string ConnectionString { get; set; }
        CloudStorageAccount cloudStorageAccount { get; set; }
        CloudBlobClient cloudBlobClient { get; set; }
        CloudBlobContainer cloudBlobContainer { get; set; }
        CloudBlockBlob cloudBlockBlob { get; set; }

        public void SetUpObjects()
        {

            //make the container name unique so if you run this over and over, you won't
            //  have any problems with the latency of deleting a container
            containerName = containerName + "-" + System.Guid.NewGuid().ToString().Substring(0, 12);

            //set the connection string
            ConnectionString =
              string.Format(@"DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}",
              storageAccountName, storageAccountKey);

            //get reference to storage account
            cloudStorageAccount = CloudStorageAccount.Parse(ConnectionString);

            //get reference to the cloud blob client
            //this is used to access the storage account blobs and containers
            cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();

            //get reference to the container
            cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);

            //create the container if it doesn't exist
            cloudBlobContainer.CreateIfNotExistsAsync().Wait();

            //set the permissions so the blobs are public
            BlobContainerPermissions permissions = new BlobContainerPermissions();
            permissions.PublicAccess = BlobContainerPublicAccessType.Blob;
            cloudBlobContainer.SetPermissionsAsync(permissions).Wait();
        }

        public void BasicBlobOps()
        {
            //set up the objects
            SetUpObjects();
            UploadBlobs();
            GetListOfBlobsAsync().Wait();
        }

        public void UploadBlobs()
        {
            //get a reference to where the block blob will go, then upload the local file
            cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(image1Name);
            cloudBlockBlob.UploadFromFileAsync(Path.Combine(localPicsToUploadPath, image1Name)).Wait();

            cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(image2Name);
            cloudBlockBlob.UploadFromFileAsync(Path.Combine(localPicsToUploadPath, image2Name)).Wait();

            cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(image3Name);
            cloudBlockBlob.UploadFromFileAsync(Path.Combine(localPicsToUploadPath, image3Name)).Wait();

            cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(pseudoFolder + image4Name);
            cloudBlockBlob.UploadFromFileAsync(Path.Combine(localPicsToUploadPath, image4Name)).Wait();

            string textToUpload = "Text Demo";
            cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(pseudoFolder + textFileName);
            cloudBlockBlob.UploadTextAsync(textToUpload).Wait();
        }

        private string GetFileNameFromBlobURI(Uri theUri, string containerName)
        {
            string theFile = theUri.ToString();
            int dirIndex = theFile.IndexOf(containerName);
            string oneFile = theFile.Substring(dirIndex + containerName.Length + 1,
                theFile.Length - (dirIndex + containerName.Length + 1));
            return oneFile;
        }

        public async Task GetListOfBlobsAsync()
        {
            BlobContinuationToken token = null;
            do
            {
                BlobResultSegment resultSegment = await cloudBlobContainer.ListBlobsSegmentedAsync(token);
                token = resultSegment.ContinuationToken;

                foreach (IListBlobItem item in resultSegment.Results)
                {
                    if (item.GetType() == typeof(CloudBlockBlob))
                    {
                        CloudBlockBlob blob = (CloudBlockBlob)item;
                        Console.WriteLine("Block blob of length {0}: {1}", blob.Properties.Length, blob.Uri);
                    }

                    else if (item.GetType() == typeof(CloudPageBlob))
                    {
                        CloudPageBlob pageBlob = (CloudPageBlob)item;

                        Console.WriteLine("Page blob of length {0}: {1}", pageBlob.Properties.Length, pageBlob.Uri);
                    }

                    else if (item.GetType() == typeof(CloudBlobDirectory))
                    {
                        CloudBlobDirectory directory = (CloudBlobDirectory)item;

                        Console.WriteLine("Directory: {0}", directory.Uri);
                    }
                }
            } while (token != null);
        }

        public void DownloadBlob()
        {
            Console.WriteLine("Enter blob name to download");
            string blobName = Console.ReadLine();
            CloudBlockBlob blockBlob = cloudBlobContainer.GetBlockBlobReference(blobName);

            // Save the blob contents to a file named “myfile”.
            string localPath = String.Format("C:\\AzureDemo\\download-{0}", blobName);
            using (var fileStream = System.IO.File.OpenWrite(@localPath))
            {
                blockBlob.DownloadToStreamAsync(fileStream).Wait();
            }
        }

    }
}
