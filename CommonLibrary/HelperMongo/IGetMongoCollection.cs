﻿using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.HelperMongo
{
    public interface IGetMongoCollection
    {
        void Init(IMongoDatabase database, string nameOfCollection, Type type, MongoCollectionSettings settings = null);

        Array GetData(object filter);

        void InserOne(object obj);

        object FindOneAndUpdateAsync(object filterPrevious, object filterUpdated);
    }
}
