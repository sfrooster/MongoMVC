using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

namespace MongoMVC.Models
{

    public class ObjectIdBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var result = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            return new ObjectId(result.AttemptedValue);
        }
    }

    public class BaseMongoEntity
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [Display(Name = "Id", Order=0)]
        [Editable(false)]
        public string IdAsString
        {
            get
            {
                return Id.ToString();
            }
        }
    }

    public class MongoEntity<T> where T : BaseMongoEntity //,new() 
    {
        private MongoClient _client;
        private MongoClient client
        {
            get
            {
                if (_client == null) _client = new MongoClient(ConfigurationManager.ConnectionStrings["MongoConnection"].ConnectionString);
                return _client;
            }
        }

        private MongoServer _server;
        private MongoServer server
        {
            get
            {
                if (_server == null) _server = client.GetServer();
                return _server;
            }
        }

        private MongoDatabase _database;
        private MongoDatabase database
        {
            get
            {
                if (_database == null) _database = server.GetDatabase("mvc");
                return _database;
            }
        }

        protected WriteConcern wc =  WriteConcern.Acknowledged;
        
        private MongoInsertOptions iopts {
            get
            {
                return new MongoInsertOptions { WriteConcern = wc };
            }
        }

        private MongoUpdateOptions uopts
        {
            get
            {
                return new MongoUpdateOptions { WriteConcern = wc };
            }
        }

        public MongoEntity()
        {
            Collection = database.GetCollection<T>(typeof(T).Name);
        }

        public MongoCollection<T> Collection { get; private set; }


        public ObjectId? Insert(T entity)
        {
            ObjectId? newId = null;

            try
            {
                var result = Collection.Insert<T>(entity, iopts);
                if (result.Ok)
                {
                    newId = entity.Id;
                }
                else
                {
                    //?!?!?
                }
            }
            catch {
                //?!?!?
            }

            return newId;
        }

        public WriteConcernResult Update(ObjectId id, UpdateBuilder<T> updates)
        {
            WriteConcernResult result = null;

            try
            {
                var query = Query<T>.EQ(e => e.Id, id);
                result = Collection.Update(query, updates, uopts);
                if (!result.Ok)
                {
                    //?!?!?
                }
            }
            catch {
                //?!?!?
            }

            return result;
        }

        public WriteConcernResult Delete(ObjectId id)
        {
            WriteConcernResult result = null;

            try
            {
                var query = Query<T>.EQ(e => e.Id, id);
                result = Collection.Remove(query, wc);
                if (!result.Ok)
                {
                    //?!?!?
                }
            }
            catch {
                //?!?!?
            }

            return result;
        }
    }
}