using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using System.Diagnostics;
using System.Configuration;

namespace Contoso.Apps.SportsLeague.WorkerRole
{
    public class AzureStorageMethods
    {
        private const string receiptBlobName = "receipts";
        private CloudBlobClient blobClient;
        private CloudBlobContainer blobContainer;
        CloudStorageAccount storageAccount;

        public AzureStorageMethods()
        {
            storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["BlobConnectionString"]);
        }

        /// <summary>
        /// Upload the generated receipt Pdf to Blob storage.
        /// </summary>
        /// <param name="file">Byte array containig the Pdf file contents to be uploaded.</param>
        /// <param name="fileName">The desired filename of the uploaded file.</param>
        /// <returns></returns>
        public string UploadPdfToBlob(byte[] file, string fileName)
        {
            // Create the blob client.
            blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            blobContainer = blobClient.GetContainerReference(receiptBlobName);

            // Create the container if it doesn't already exist.
            blobContainer.CreateIfNotExists(BlobContainerPublicAccessType.Blob);

            string fileUri = string.Empty;

            CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(fileName);

            using (var stream = new MemoryStream(file))
            {
                // Upload the in-memory Pdf file to blob storage.
                blockBlob.UploadFromStream(stream);
            }

            fileUri = blockBlob.Uri.ToString();

            return fileUri;
        }

        /// <summary>
        /// Grabs the next pending queue message containing the next order
        /// whose receipt we need to generate.
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetOrderIdFromQueue()
        {
            int orderId = 0;

            // Create the queue client.
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue.
            CloudQueue queue = queueClient.GetQueueReference("receiptgenerator");

            // Create the queue if it doesn't already exist.
            if (await queue.CreateIfNotExistsAsync())
            {
                Console.WriteLine("Queue '{0}' Created", queue.Name);
            }
            else
            {
                Console.WriteLine("Queue '{0}' Exists", queue.Name);
            }

            // Get the next message.
            CloudQueueMessage retrievedMessage = await queue.GetMessageAsync();

            if (retrievedMessage != null)
            {
                Trace.TraceInformation("Retrieved Queue Message: " + retrievedMessage.AsString);
                // Process the message in less than 30 seconds, and then delete the message.
                await queue.DeleteMessageAsync(retrievedMessage);
                int.TryParse(retrievedMessage.AsString, out orderId);
            }

            return orderId;
        }
    }
}
