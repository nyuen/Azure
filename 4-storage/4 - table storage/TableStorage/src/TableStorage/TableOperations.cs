using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace TableStorage
{
    public class TableOperations
    {
        string storageAccountName = "azureenablement";
        string storageAccountKey = "ACCESSKEY";
        string tableName = "tabledemo";

        public string ConnectionString { get; set; }
        public CloudStorageAccount cloudStorageAccount { get; set; }
        public CloudTableClient cloudTableClient { get; set; }
        public CloudTable cloudTable { get; set; }

        public void SetUpObjects()
        {
            //set up table name
            tableName = tableName + System.Guid.NewGuid().ToString().Substring(0, 12).Replace("-", "");
            Console.WriteLine("TableName = {0}", tableName);

            ConnectionString =
                string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}",
                storageAccountName, storageAccountKey);

            //heirarchy of objects -- get reference to storage account
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(ConnectionString);

            //heirarchy of objects -- get reference to table client
            cloudTableClient = cloudStorageAccount.CreateCloudTableClient();

            //heirarchy of objects -- get reference to the table and create it
            cloudTable = cloudTableClient.GetTableReference(tableName);
            cloudTable.CreateIfNotExistsAsync().Wait();
        }
        


        public void TableOps()
        {
            SetUpObjects();

            Console.WriteLine("{0}Adding records.", Environment.NewLine);
            AddCustomer("Robin", "Shahan", "Red", "C#");
            AddCustomer("Michael", "Washam", "Mission Impossible", "PowerShell");
            AddCustomer("Luke", "Skywalker", "StarWars:Return of the Jedi", "Ewok");
            AddCustomer("Han", "Solo", "StarWars:The Empire Strikes Back", "Wookie");
            AddCustomer("Darth", "Vader", "Star Wars", "English");

            Console.WriteLine("{0}", Environment.NewLine);
            GetCustomerFavorites("Luke", "Skywalker");

            Console.WriteLine("{0}", Environment.NewLine);
            SetCustomerFavorites("Darth", "Vader", "Revenge of the Sith", "Evil");
            GetCustomerFavorites("Darth", "Vader");

            Console.WriteLine("{0}", Environment.NewLine);
            GetListOfCustomers();

            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
        }

        public void AddCustomer(string firstName, string lastName,
            string favoriteMovie, string favoriteLanguage)
        {
            Console.WriteLine("[AddCustomer] called. {0} {1}, {2}, {3}",
                firstName, lastName, favoriteMovie, favoriteLanguage);

            //create a new entity
            CustomerEntity cust =
                new CustomerEntity(firstName, lastName, favoriteMovie, favoriteLanguage);

            //instantiate an insert option and pass it the entity
            TableOperation insertOperation = TableOperation.Insert(cust);

            //execute the operation
            cloudTable.ExecuteAsync(insertOperation).Wait();
        }

        public void GetCustomerFavorites(string firstName, string lastName)
        {
            string favoriteMovie = string.Empty;
            string favoriteLanguage = string.Empty;

            //instantiate a new entity
            CustomerEntity cust = new CustomerEntity(firstName, lastName, string.Empty, string.Empty);

            //instantiate a retrieve operatino, passing the partition key and row key
            TableOperation retrieveOperation =
              TableOperation.Retrieve<CustomerEntity>(cust.PartitionKey, cust.RowKey);

            //execute the operation
            TableResult retrievedResult = cloudTable.ExecuteAsync(retrieveOperation).Result;

            //if records were found
            if (retrievedResult != null)
            {
                //cast the result as a CustomerEntity and access and print the properties
                CustomerEntity oneCust = (CustomerEntity)retrievedResult.Result;
                favoriteMovie = oneCust.FavoriteMovie;
                favoriteLanguage = oneCust.FavoriteLanguage;
            }
            Console.WriteLine("[GetCustomerFavoties] called. {0} {1}, {2}, {3}",
                firstName, lastName, favoriteMovie, favoriteLanguage);
        }

        public void SetCustomerFavorites(string firstName, string lastName,
            string favoriteMovie, string favoriteLanguage)
        {
            Console.WriteLine("[SetCustomerFavorites] for {0} {1}, {2}, {3}",
                firstName, lastName, favoriteMovie, favoriteLanguage);

            //create a customer entity with the data set
            CustomerEntity cust =
              new CustomerEntity(firstName, lastName, favoriteMovie, favoriteLanguage);

            //set up the operation to retrieve the entity matching the primary key
            TableOperation retrieveOperation =
              TableOperation.Retrieve<CustomerEntity>(cust.PartitionKey, cust.RowKey);

            //execute the operation
            TableResult retrievedResult = cloudTable.ExecuteAsync(retrieveOperation).Result;

            //if a matching entity was found...
            if (retrievedResult != null)
            {
                //cast the result to a customer entity, update the properties
                CustomerEntity updateEntity = (CustomerEntity)retrievedResult.Result;
                updateEntity.FavoriteLanguage = favoriteLanguage;
                updateEntity.FavoriteMovie = favoriteMovie;

                //create an update operation and pass it the new entity
                TableOperation updateOperation = TableOperation.Replace(updateEntity);

                //execute the update operation
                cloudTable.ExecuteAsync(updateOperation).Wait();

                Console.WriteLine("{0}", Environment.NewLine);
                DeleteCustomer("Michael", "Washam");
                DeleteCustomer("Robin", "Shahan");
                GetListOfCustomers();

                //cleanup
                cloudTable.DeleteAsync().Wait();
            }
        }

        public void GetListOfCustomers()
        {
            //get list of all records in a specific partition

            Console.WriteLine("[GetListOfCustomers] called.");

            //used for numbering the rows
            int index = 0;
            var items = new List<CustomerEntity>();
            //set up a query where the partition key is "customer", which is what our partition key is
            TableQuery<CustomerEntity> query = new TableQuery<CustomerEntity>().Where
                (TableQuery.GenerateFilterCondition("PartitionKey",
                 QueryComparisons.Equal, "customer"));

            //execute the query
            TableContinuationToken token = new TableContinuationToken();
            do
            {

                TableQuerySegment<CustomerEntity> seg =  cloudTable.ExecuteQuerySegmentedAsync<CustomerEntity>(query, token).Result;
                token = seg.ContinuationToken;
                items.AddRange(seg);
             } while (token != null);

            //print any entities returned
            if (items != null)
            {
                foreach (CustomerEntity cust in items)
                {
                    index++;
                    Console.WriteLine("{0}, {1} {2},"
                        + " {3}, {4}",
                        index, cust.FirstName, cust.LastName,
                        cust.FavoriteMovie, cust.FavoriteLanguage);
                }
            }
            else
            {
                Console.WriteLine("[GetListOfCustomers] No rows found in table.");
            }
        }

        public void DeleteCustomer(string firstName, string lastName)
        {
            Console.WriteLine("[DeleteCustomer] called for {0} {1}", firstName, lastName);

            //instantiate an entity with the primary key
            CustomerEntity cust = new CustomerEntity(firstName, lastName, string.Empty, string.Empty);

            //set up the operation to look for the record
            TableOperation retrieveOperation =
              TableOperation.Retrieve<CustomerEntity>(cust.PartitionKey, cust.RowKey);

            //execute the operation retrieving the entity
            TableResult retrievedResult = cloudTable.ExecuteAsync(retrieveOperation).Result;

            //if it found the record
            if (retrievedResult != null)
            {
                //cast the result to the customer entity
                CustomerEntity oneCust = (CustomerEntity)retrievedResult.Result;

                //create a delete operation and pass it the entity, then execute the operation
                TableOperation deleteOperation = TableOperation.Delete(oneCust);
                cloudTable.ExecuteAsync(deleteOperation).Wait();
            }
        }
    }
}
