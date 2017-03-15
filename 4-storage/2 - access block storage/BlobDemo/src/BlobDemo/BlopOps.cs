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
            CopyBlob();
            DownloadBlob();
            BlobProperties();

            // perform blob cleanup
            Console.WriteLine("Perform Clean up?: Y/n");
            String cleanup = Console.ReadLine();
            if (cleanup.ToLower() == "y")
            {
                CleanUp();
            }
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

        public void CopyBlob()
        {
            //copy from image1name
            cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(image1Name);

            //get a reference to the destination blob.
            CloudBlockBlob destBlob = cloudBlobContainer.GetBlockBlobReference("copyof_" + image1Name);

            //start the copy from the source blob (cloudBlockBlob) to the destination blob.
            destBlob.StartCopyAsync(cloudBlockBlob).Wait();
            
            //List blobs
            GetListOfBlobsAsync().Wait();

            //display blob properties
            BlobProperties();
        }

        public void BlobProperties()
        {
            cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(pseudoFolder + image4Name);
            Console.WriteLine(string.Empty);

            //display some of the blob properties
            Console.WriteLine("blob type = " + cloudBlockBlob.BlobType);
            Console.WriteLine("blob name = " + cloudBlockBlob.Name);
            Console.WriteLine("blob URI = " + cloudBlockBlob.Uri);
            //update the system properties on the object and display a couple of them
            cloudBlockBlob.FetchAttributesAsync().Wait();

            Console.WriteLine("content type = " + cloudBlockBlob.Properties.ContentType);
            Console.WriteLine("size = " + cloudBlockBlob.Properties.Length);
            //change the content type from 'application/octet stream' to 'image/jpg'
            cloudBlockBlob.Properties.ContentType = "image/jpg";
            cloudBlockBlob.SetPropertiesAsync().Wait();


            //refresh the attributes and write out the content type again
            cloudBlockBlob.FetchAttributesAsync().Wait();
            Console.WriteLine("content type = " + cloudBlockBlob.Properties.ContentType);

            //Displaying Blob Metadata
            PrintMetadata();

            //set some metadata and save it
            cloudBlockBlob.Metadata["First"] = "number one";
            cloudBlockBlob.Metadata["Second"] = "number two";
            cloudBlockBlob.Metadata["Three"] = "number three";
            cloudBlockBlob.SetMetadataAsync().Wait();

            //now clear the metadata, save the change,
            //  and then print it again (empty list)
            cloudBlockBlob.Metadata.Clear();
            cloudBlockBlob.SetMetadataAsync().Wait();
            PrintMetadata();

            //print it out again
            PrintMetadata();

        }

        public void PrintMetadata()
        {
            //fetch the attributes of the blob to make sure they are current
            cloudBlockBlob.FetchAttributesAsync().Wait();
            //if there is metaata, loop throught he dictionary and print it out
            int index = 0;
            if (cloudBlockBlob.Metadata.Count > 0)
            {
                IDictionary<string, string> metadata = cloudBlockBlob.Metadata;
                foreach (KeyValuePair<string, string> oneMetadata in metadata)
                {
                    index++;
                    Console.WriteLine("metadata {0} = {1}, {2}", index,
                        oneMetadata.Key.ToString(), oneMetadata.Value.ToString());
                }
            }
            else
            {
                Console.WriteLine("No metadata found.");
            }
        }

        public async Task DeleteAllBlobsAsync()
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
                        Console.WriteLine("Deleting: {0}", blob.Properties.Length, blob.Uri);
                        blob.DeleteAsync().Wait();
                    }
                }
            } while (token != null);
         }

        public void CleanUp()
        {
            DeleteAllBlobsAsync().Wait();
            GetListOfBlobsAsync().Wait();
            cloudBlobContainer.DeleteAsync().Wait();
        }
    }
}
