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

using MongoDB.Bson;
using MongoDB.Driver;

namespace CommonLibrary.HelperMongo
{
    public class GetMongoCollection:IGetMongoCollection
    {
        private object mongoCollection;
        private Type GetType;

        public void Init(IMongoDatabase database,string nameOfCollection,Type type, MongoCollectionSettings settings=null)
        {
            GetType = type;
            mongoCollection = GetMethodGeneRef(typeof(IMongoDatabase), type, "GetCollection", database, new object[] { nameOfCollection, settings });
        }

        public Array GetData(object filter)
        {
            var method = GetMethodGeneRef(typeof(IMongoCollectionExtensions), GetType, "Find", mongoCollection,new object[] { mongoCollection, filter, null });
            var data = GetMethodGeneRef(typeof(IAsyncCursorSourceExtensions), GetType, "ToList", method, new object[] {method, default(CancellationToken) });
            var castToArray = GetMethodGeneRef(data.GetType(), GetType, "ToArray", data);
            return (castToArray as Array);
        }

       

        public object FindOneAndUpdateAsync(object filterPrevious,object filterUpdated)
        {
            var method = GetMethodGeneRef(typeof(IMongoCollectionExtensions),GetType, "FindOneAndUpdate", mongoCollection, 
                new object[] { mongoCollection, filterPrevious,filterUpdated,null, null });
            return method;
        }

        private object GetMethodGeneRef(Type typeOfMethod,Type typeMakeGeMethod,string nameOfMethod,object obj,object[] para=null)
        {
            var method = typeOfMethod.GetMethods().Where(x => x.Name.Equals(nameOfMethod)).ElementAt(0);
            method =method.IsGenericMethod? method.MakeGenericMethod(typeMakeGeMethod):method;
            var methodInvoke = method.Invoke(obj, para);
            return methodInvoke;
        }

        private object GetMethod(Type typeOfMethod, string nameOfMethod, object obj, object[] para = null)
        {
            var method = typeOfMethod.GetMethods().Where(x => x.Name.Equals(nameOfMethod)).ElementAt(0);
            var methodInvoke = method.Invoke(obj, para);
            return methodInvoke;
        }
    }
}
