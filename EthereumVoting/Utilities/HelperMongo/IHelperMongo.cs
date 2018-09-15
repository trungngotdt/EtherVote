
using MongoDB.Driver;
using MongoDB.Bson;

namespace EthereumVoting.Utilities.HelperMongo
{
    public interface IHelperMongo
    {
        IMongoClient GetClient(string url,int port, string name, string pass);

        IMongoDatabase GetDatabase(string nameOfDatabase,MongoDatabaseSettings settings=null);

        IMongoCollection<T> GetCollection<T>(string nameOfCollection); 
    }
}
