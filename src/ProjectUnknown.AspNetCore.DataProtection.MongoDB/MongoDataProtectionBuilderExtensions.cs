using System;
using Microsoft.AspNetCore.DataProtection;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ProjectUnknown.AspNetCore.DataProtection.MongoDB
{
    public static class MongoDataProtectionBuilderExtensions
    {
        private const string DataProtectionKeysName = "DataProtection-Keys";

        /// <summary>
        /// Configures the data protection system to persist keys to specified collection in MongoDB database.
        /// </summary>
        /// <param name="builder">The builder instance to modify.</param>
        /// <param name="collectionFactory">The factory used to create <see cref="IMongoCollection{BsonDocument}"/> instances.</param>
        /// <returns>A reference to the <see cref="IDataProtectionBuilder"/> after this operation has completed.</returns>
        public static IDataProtectionBuilder PersistKeysToMongo(this IDataProtectionBuilder builder, Func<IMongoCollection<BsonDocument>> collectionFactory)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (collectionFactory == null)
            {
                throw new ArgumentNullException(nameof(collectionFactory));
            }

            return PersistKeysToMongoInternal(builder, collectionFactory);
        }

        /// <summary>
        /// Configures the data protection system to persist keys to specified collection in MongoDB database.
        /// </summary>
        /// <param name="builder">The builder instance to modify.</param>
        /// <param name="client">The <see cref="IMongoClient"/> for database access.</param>
        /// <param name="database">The name of the database.</param>
        /// <returns>A reference to the <see cref="IDataProtectionBuilder"/> after this operation has completed.</returns>
        public static IDataProtectionBuilder PersistKeysToMongo(this IDataProtectionBuilder builder, IMongoClient client, string database)
        {
            return PersistKeysToMongo(builder, client, database, DataProtectionKeysName);
        }

        /// <summary>
        /// Configures the data protection system to persist keys to specified collection in MongoDB database.
        /// </summary>
        /// <param name="builder">The builder instance to modify.</param>
        /// <param name="client">The <see cref="IMongoClient"/> for database access.</param>
        /// <param name="database">The name of the database.</param>
        /// <param name="collection">The name of the collection.</param>
        /// <returns>A reference to the <see cref="IDataProtectionBuilder"/> after this operation has completed.</returns>
        public static IDataProtectionBuilder PersistKeysToMongo(this IDataProtectionBuilder builder, IMongoClient client, string database, string collection)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (string.IsNullOrEmpty(database))
            {
                throw new ArgumentNullException(nameof(database));
            }

            if (string.IsNullOrEmpty(collection))
            {
                throw new ArgumentNullException(nameof(collection));
            }

            return PersistKeysToMongoInternal(builder, () => client.GetDatabase(database).GetCollection<BsonDocument>(collection));
        }

        /// <summary>
        /// Configures the data protection system to persist keys to specified collection in MongoDB database.
        /// </summary>
        /// <param name="builder">The builder instance to modify.</param>
        /// <param name="connectionString">Connection string to MongoDB server.</param>
        /// <param name="database">The name of the database.</param>
        /// <returns>A reference to the <see cref="IDataProtectionBuilder"/> after this operation has completed.</returns>
        public static IDataProtectionBuilder PersistKeysToMongo(this IDataProtectionBuilder builder, string connectionString, string database)
        {
            return PersistKeysToMongo(builder, connectionString, database, DataProtectionKeysName);
        }

        /// <summary>
        /// Configures the data protection system to persist keys to specified collection in MongoDB database.
        /// </summary>
        /// <param name="builder">The builder instance to modify.</param>
        /// <param name="connectionString">Connection string to MongoDB server.</param>
        /// <param name="database">The name of the database.</param>
        /// <param name="collection">The name of the collection.</param>
        /// <returns>A reference to the <see cref="IDataProtectionBuilder"/> after this operation has completed.</returns>
        public static IDataProtectionBuilder PersistKeysToMongo(this IDataProtectionBuilder builder, string connectionString, string database, string collection)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            if (string.IsNullOrEmpty(database))
            {
                throw new ArgumentNullException(nameof(database));
            }

            if (string.IsNullOrEmpty(collection))
            {
                throw new ArgumentNullException(nameof(collection));
            }

            var client = new MongoClient(connectionString);

            return PersistKeysToMongoInternal(builder, () => client.GetDatabase(database).GetCollection<BsonDocument>(collection));
        }

        private static IDataProtectionBuilder PersistKeysToMongoInternal(IDataProtectionBuilder builder, Func<IMongoCollection<BsonDocument>> collectionFactory)
        {
            builder.AddKeyManagementOptions(options =>
            {
                options.XmlRepository = new MongoXmlRepository(collectionFactory);
            });

            return builder;
        }
    }
}
