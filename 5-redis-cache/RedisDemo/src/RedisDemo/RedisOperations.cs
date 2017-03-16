using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RedisDemo
{
    public class RedisOperations
    {
        static string CONNECTION_STRING = "";


        public void BasicCacheOps()
        {
            Console.WriteLine("Connecting to Redis Cache");
            ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(CONNECTION_STRING);
            IDatabase cache = connection.GetDatabase();

            Console.WriteLine("Adding strings and ints to the Cache");
            // Perform cache operations using the cache object...
            // Simple put of integral data types into the cache
            cache.StringSet("key1", "value");
            cache.StringSet("key2", 25);

            // Simple get of data types from the cache
            string key1 = cache.StringGet("key1");
            int key2 = (int)cache.StringGet("key2");
            Console.WriteLine("key 1:" + key1);
            Console.WriteLine("key 2:" + key2);

            Console.WriteLine("Incrementing key 2");
            var result = cache.StringIncrement("key2"); //after operation Our int number is now 102
            Console.WriteLine("incremented result : "+ result);
            Console.WriteLine("Incrementing key 2 by 100");
            result = cache.StringIncrement("key2", 100); //after operation Our int number is now 102
            Console.WriteLine("incremented result : " + result);

            Console.WriteLine("Setting string value with expiration set to 90 min");
            cache.StringSet("key1", "value1", TimeSpan.FromMinutes(90));

            Console.WriteLine("Adding complex objects to the cache");
            // Store to cache
            cache.StringSet("e25", JsonConvert.SerializeObject(new Employee(25, "Clayton Gragg")));
            // Retrieve from cache
            Employee e25 = JsonConvert.DeserializeObject<Employee>(cache.StringGet("e25"));
            Console.WriteLine(cache.StringGet("e25"));
            Console.WriteLine("Listing keys");

            var endpoints = connection.GetEndPoints(true);
            foreach (var endpoint in endpoints)
            {
                var server = connection.GetServer(endpoint);
                foreach (var key in server.Keys())
                {
                    Console.WriteLine("Key: " + key.ToString() +", value: " + cache.StringGet(key.ToString()));
                }
            }

            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
        }
    }
}
