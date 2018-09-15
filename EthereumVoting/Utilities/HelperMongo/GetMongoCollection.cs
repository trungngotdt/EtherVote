using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EthereumVoting.Model;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EthereumVoting.Utilities.HelperMongo
{
    public class GetMongoCollection:IGetMongoCollection
    {
        private object mongoCollection;
        private Type GetType;

        public void Init(IMongoDatabase database,string nameOfCollection,Type type,MongoCollectionSettings settings=null)
        {
            var method= typeof(IMongoDatabase).GetMethod("GetCollection");
            MethodInfo generic = method.MakeGenericMethod(type);
            var aa= database.GetCollection<User>(nameOfCollection, settings);
            mongoCollection = generic.Invoke(database, new object[] { nameOfCollection, settings });
            var method2 =typeof( IMongoCollectionExtensions).GetMethods().Where(x=>x.Name.Equals("Find")).ElementAt(2);
            var collection = mongoCollection.GetType().GetMethods();
            var ss = database.GetCollection<User>(nameOfCollection, settings).Find(x=>true).ToList();
            
            foreach (var item in collection)
            {
                var name = item.Name;
                Debug.WriteLine(name+" is "+item.IsGenericMethod);
            }
            var ge = method2.MakeGenericMethod(type);
            //var re = ge.Invoke(mongoCollection, new object[] { mongoCollection,XamlGeneratedNamespace=,null });


            //GetType = type;
            //var aa = typeof(ttype);
            //database.GetCollection<type>("");
            //database.GetCollection<User>(nameOfCollection, settings);
        }
/*
        public List<Type> GetAllData()
        {
            (mongoCollection as IMongoCollection<GetType>).Find<GetType>();
        }*/
    }
}
