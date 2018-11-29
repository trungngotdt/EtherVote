using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using CommonServiceLocator;
namespace CommonLibrary.HelperMongo
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
                    Server = new MongoServerAddress(url, port),//"127.0.0.1", 27017),
                    Credentials = new MongoCredential[] { MongoCredential.CreateCredential("data1", name, pass) }//"user1", "pass1") }
                };
                Client = new MongoClient(setting);
                return Client;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IGetMongoCollection GetMongoCollection()
        {
            try
            {
                return ServiceLocator.Current.GetInstance<IGetMongoCollection>();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
            /*
            var data = Database.GetCollection<User>("user").Find(new BsonDocument()).ToList();
            getMongoCollection.Init(Database, "user", typeof(User));
            var arr = getMongoCollection.GetData(Builders<User>.Filter.Empty);
            return null;*/
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
