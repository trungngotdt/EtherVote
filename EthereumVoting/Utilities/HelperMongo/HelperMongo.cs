using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using CommonServiceLocator;
using EthereumVoting.Model;

namespace EthereumVoting.Utilities.HelperMongo
{
    public class HelperMongo : IHelperMongo
    {
        private IMongoClient Client;
        private IMongoDatabase Database;

        public IMongoClient GetClient(string url,int port, string name, string pass)
        {
            try
            {
                var setting = new MongoClientSettings()
                {
                    Server = new MongoServerAddress("127.0.0.1", 27017),
                    Credentials = new MongoCredential[] { MongoCredential.CreateCredential("data1", "user1", "pass1") }
                };
                Client = new MongoClient(setting);
                return Client;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IMongoCollection<T> GetCollection<T>(string nameOfCollection)
        {
            var getMongoCollection = ServiceLocator.Current.GetInstance<IGetMongoCollection>();
            var data = Database.GetCollection<User>("user").Find(new BsonDocument()).ToList();
            getMongoCollection.Init(Database, "user", typeof(User));
            return null;
        }

        public IMongoDatabase GetDatabase(string nameOfDatabase,MongoDatabaseSettings settings=null)
        {
            try
            {
                return Database = Client.GetDatabase(nameOfDatabase, settings);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
    }
}
