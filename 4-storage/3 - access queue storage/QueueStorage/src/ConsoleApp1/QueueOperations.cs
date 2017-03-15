using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace QueueStorage
{
    public class QueueOperations
    {

        string storageAccountName = "azureenablement";
        string storageAccountKey = "ACCESSKEY";
        string queueName = "testqueue";
        //set the invisibility time to 10 seconds
        int queueMessageInvisibilityTime = 10;

        string ConnectionString { get; set; }
        CloudStorageAccount cloudStorageAccount { get; set; }
        CloudQueueClient cloudQueueClient { get; set; }
        CloudQueue cloudQueue { get; set; }

        public void SetUpObjects()
        {
            //set up a unique queue name
            queueName = queueName + "-" + System.Guid.NewGuid().ToString().Substring(0, 12);
            Console.WriteLine("QueueName = {0}", queueName);

            //set up the connection string
            ConnectionString =
              string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}",
              storageAccountName, storageAccountKey);

            //heirarchy of objects. Get a reference to the storage account
            cloudStorageAccount = CloudStorageAccount.Parse(ConnectionString);

            //create the cloudqueueclient object
            cloudQueueClient = cloudStorageAccount.CreateCloudQueueClient();

            //get a reference to the queue, then create it if it doesn't exist
            cloudQueue = cloudQueueClient.GetQueueReference(queueName);
            cloudQueue.CreateIfNotExistsAsync().Wait();
        }

        public void QueueOps()
        {
            Console.WriteLine("SetUpQueue.");
            SetUpObjects();

            //write a message to the queue
            AddToQueue("First message");
            CloudQueueMessage cloudQueueMessage = cloudQueue.PeekMessageAsync().Result;
            OutputQueueMessage(cloudQueueMessage, "Peek");

            //read the message from the queue
            //this makes it invisible for 10 seconds
            cloudQueueMessage = ReadMessage();
            OutputQueueMessage(cloudQueueMessage, "Read");

            //now try to peek the message
            //you shouldn't see anything because the one message is invisible
            cloudQueueMessage = cloudQueue.PeekMessageAsync().Result;
            OutputQueueMessage(cloudQueueMessage, "Peek");

            //now wait for the invisibility to timeout and peek again
            //you should be able to see the message because it wasn't deleted,
            //  and the invisibility timeout was reached
            //  so it became visible again
            Console.WriteLine(Environment.NewLine + "Wait for the invisibility time expire (10 seconds).");
            System.Threading.Thread.Sleep(10000);

            //now peek again; you should see it now
            cloudQueueMessage = cloudQueue.PeekMessageAsync().Result;
            OutputQueueMessage(cloudQueueMessage, "Peek");

            // change the message
            Console.WriteLine("{0}Changing the message content and invisibility time.",
               Environment.NewLine);
            ChangeMessage("totally change the message message");

            //this shouldn't work, because it's still invisible
            cloudQueueMessage = cloudQueue.PeekMessageAsync().Result;
            OutputQueueMessage(cloudQueueMessage, "After update, peek");

            //wait 5 seconds and try again
            Console.WriteLine(Environment.NewLine + "Wait for the invisibility time expire (5 seconds).");
            System.Threading.Thread.Sleep(5000);
            cloudQueueMessage = cloudQueue.PeekMessageAsync().Result;
            OutputQueueMessage(cloudQueueMessage, "After update, peek");

            //now delete the message
            cloudQueueMessage = ReadMessage();
            OutputQueueMessage(cloudQueueMessage, "Delete");
            Console.WriteLine("{0}Deleting queue message [{1}]", Environment.NewLine, cloudQueueMessage);
            cloudQueue.DeleteMessageAsync(cloudQueueMessage).Wait();

            Console.WriteLine(Environment.NewLine + "Wait for the invisibility time expire (10 seconds).");
            System.Threading.Thread.Sleep(10000);

            //now peek again; you shouldn't see any messages
            cloudQueueMessage = cloudQueue.PeekMessageAsync().Result;
            OutputQueueMessage(cloudQueueMessage, "Peek");

            Console.WriteLine("adding 50 messages and reading in batch");
            Console.ReadLine();
            //add a bunch of messages
            int msgCount = 50;
            for (int i = 0; i < msgCount; i++)
            {
                AddToQueue("Message " + (i + 1));
            }
            //read the messages
            GetApproxMsgCount();

            //read 20 of the messages and delete them
            ReadMultipleMessages(20, true);

            //get the message count
            GetApproxMsgCount();

            //Clear everything;
            ClearQueue();
            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();

        }

        public void AddToQueue(string messageString)
        {
            Console.WriteLine("{0}Add message [{1}] to queue.", Environment.NewLine, messageString);

            CloudQueueMessage cloudQueueMessage = new CloudQueueMessage(messageString);
            cloudQueue.AddMessageAsync(cloudQueueMessage).Wait();
        }

        public void OutputQueueMessage(CloudQueueMessage cloudQueueMessage, string action)
        {
            //this will be null if it tried to read the queue and there were no messages
            if (cloudQueueMessage != null)
            {
                //if you read the message and set the invisibility time, this will have a value
                if (cloudQueueMessage.NextVisibleTime.HasValue)
                {
                    Console.WriteLine("{0}{1} message = [{2}], time = {3}, next visible time = {4}",
                        Environment.NewLine, action,
                        cloudQueueMessage.AsString, DateTime.UtcNow,
                        cloudQueueMessage.NextVisibleTime.Value.ToString());
                }
                else
                {
                    Console.WriteLine("{0}{1} message = [{2}], time = {3}",
                        Environment.NewLine, action,
                        cloudQueueMessage.AsString, DateTime.UtcNow);
                }
            }
            else
            {
                Console.WriteLine("{0}{1} --> No messages found.",
                    Environment.NewLine, action);
            }
        }

        public CloudQueueMessage ReadMessage()
        {
            CloudQueueMessage cloudQueueMessage = null;

            //set the invisibility time to 10 seconds
            TimeSpan visTimeout = new TimeSpan(0, 0, queueMessageInvisibilityTime);

            //read the actual message from the queue
            cloudQueueMessage = cloudQueue.GetMessageAsync(visTimeout, new QueueRequestOptions(), new OperationContext()).Result;
            return cloudQueueMessage;
        }

        public void ChangeMessage(string newMessage)
        {
            // Get the message from the queue
            CloudQueueMessage cloudQueueMessage = cloudQueue.GetMessageAsync().Result;
            if (cloudQueueMessage != null)
            {
                //let's change the invisibility time to 5 seconds
                int newInvisibilityTime = 5;

                //add the new string to the old message and update the message
                cloudQueueMessage.SetMessageContent(newMessage + ", Updated contents.");

                //call to do the update, passing in the new invisibility time of 5 seconds
                cloudQueue.UpdateMessageAsync(cloudQueueMessage,
                    TimeSpan.FromSeconds(newInvisibilityTime),
                    MessageUpdateFields.Content | MessageUpdateFields.Visibility).Wait();
            }
        }

        public void GetApproxMsgCount()
        {
            // Fetch the queue attributes.
            cloudQueue.FetchAttributesAsync().Wait();

            // Retrieve the cached approximate message count.
            int? cachedMessageCount = cloudQueue.ApproximateMessageCount;

            //if the count is not null, and is > 0...
            if (cachedMessageCount.HasValue && cachedMessageCount.Value > 0)
            {
                // Display number of messages.
                Console.WriteLine("Number of messages in queue: " + cachedMessageCount);
            }
            else
            {
                Console.WriteLine("No messages in queue.");
            }
        }

        public void ReadMultipleMessages(int readCount, bool deleteMessages)
        {

            //read them from the queue
            IEnumerable<CloudQueueMessage> listOfMessages =
                cloudQueue.GetMessagesAsync(readCount).Result;

            //display count
            int messageCount = 0;
            foreach (CloudQueueMessage cloudQueueMessage in listOfMessages)
            {
                messageCount++;
                Console.WriteLine("message = [{0}], time = {1}",
                    cloudQueueMessage.AsString, DateTime.UtcNow);
                if (deleteMessages)
                {
                    cloudQueue.DeleteMessageAsync(cloudQueueMessage).Wait();
                }
            }

            Console.WriteLine("{0}Tried to read {1} messages. {2} messages read.", Environment.NewLine,
              readCount, messageCount);
        }

        public void ClearQueue()
        {
            //delete all messages from the queue at once
            cloudQueue.ClearAsync().Wait();
            GetApproxMsgCount();
            //delete the queue itself
            cloudQueue.DeleteAsync().Wait();
        }
    }
}
