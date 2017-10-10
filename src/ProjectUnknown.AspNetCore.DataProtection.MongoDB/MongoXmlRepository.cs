using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.AspNetCore.DataProtection.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ProjectUnknown.AspNetCore.DataProtection.MongoDB
{
    /// <summary>
    /// An <see cref="IXmlRepository"/> which is backed by MongoDB.
    /// </summary>
    public class MongoXmlRepository : IXmlRepository
    {
        private readonly Func<IMongoCollection<BsonDocument>> collectionFactory;

        /// <summary>
        /// Creates a new instance of the <see cref="MongoXmlRepository"/>.
        /// </summary>
        /// <param name="collectionFactory">The factory used to create <see cref="IMongoCollection{BsonDocument}"/> instances.</param>
        public MongoXmlRepository(Func<IMongoCollection<BsonDocument>> collectionFactory)
        {
            this.collectionFactory = collectionFactory;
        }

        public IReadOnlyCollection<XElement> GetAllElements()
        {
            return GetAllElementsCore().ToList().AsReadOnly();
        }

        private IEnumerable<XElement> GetAllElementsCore()
        {
            var collection = collectionFactory();
            var cursor = collection.FindSync(FilterDefinition<BsonDocument>.Empty);

            while (cursor.MoveNext())
            {
                foreach (var item in cursor.Current)
                {
                    if (item.TryGetElement("xml", out var value) && value.Value.BsonType == BsonType.String)
                    {
                        yield return XElement.Parse(value.Value.AsString);
                    }
                    else
                    {
                        throw new Exception("Invalid document.");
                    }
                }
            }
        }

        public void StoreElement(XElement element, string friendlyName)
        {
            var collection = collectionFactory();
            var document = new BsonDocument
            {
                new BsonElement("xml", element.ToString(SaveOptions.DisableFormatting))
            };

            collection.InsertOne(document);
        }
    }
}
