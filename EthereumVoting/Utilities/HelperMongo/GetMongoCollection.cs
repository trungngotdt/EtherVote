using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
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

        public void Init(IMongoDatabase database,string nameOfCollection,Type type, MongoCollectionSettings settings=null)
        {
            GetType = type;
            mongoCollection = GetMethodRef(typeof(IMongoDatabase), type, "GetCollection", database, new object[] { nameOfCollection, settings });
        }

        public Array GetData(object filter)
        {
            var method = GetMethodRef(typeof(IMongoCollectionExtensions), GetType, "Find", mongoCollection,new object[] { mongoCollection, filter, null });
            var data = GetMethodRef(typeof(IAsyncCursorSourceExtensions), GetType, "ToList", method, new object[] {method, default(CancellationToken) });
            var castToArray = GetMethodRef(data.GetType(), GetType, "ToArray", data);
            return (castToArray as Array);
        }

        private object GetMethodRef(Type typeOfMethod,Type typeMakeGeMethod,string nameOfMethod,object obj,object[] para=null)
        {
            var method = typeOfMethod.GetMethods().Where(x => x.Name.Equals(nameOfMethod)).ElementAt(0);
            method =method.IsGenericMethod? method.MakeGenericMethod(typeMakeGeMethod):method;
            var methodInvoke = method.Invoke(obj, para);
            return methodInvoke;
        }
    }
}
