using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace TableStorage
{
    public class CustomerEntity : TableEntity
    {
        private readonly string partitionKey = "customer";

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FavoriteMovie { get; set; }
        public string FavoriteLanguage { get; set; }

        public CustomerEntity() { }

        public CustomerEntity(string firstName, string lastName,
          string favoriteMovie, string favoriteLanguage)
        {
            this.PartitionKey = partitionKey;
            this.RowKey = firstName + " " + lastName;

            FirstName = firstName;
            LastName = lastName;
            FavoriteMovie = favoriteMovie;
            FavoriteLanguage = favoriteLanguage;
        }

    }
}
