using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Distributr.Mobile.Core.Data;
using Distributr.Mobile.Core.Data.Sequences;
using SQLite.Net;
using SQLite.Net.Attributes;
using SQLite.Net.Interop;
using SQLiteNetExtensions.Extensions;

namespace Distributr.Mobile.Data
{
    //We only use one instance of this class across the whole app. It can be shared by multi-threads.
    public class Database : SQLiteConnection
    {
        //A multi-threaded, read-write connection where the database will be created, if necessary
        private const SQLiteOpenFlags OpenFlags =
            SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create;

        public Database(ISQLitePlatform platform, IFileSystem fileSystem)
            : base(platform, fileSystem.GetDatabasePath(), OpenFlags, storeDateTimeAsTicks: true)
        {
            CreateTables();
        }

        private void CreateTables()
        {
            foreach (var type in DatabaseConfig.GetPersistentTypes())
            {
                CreateTable(type);
            }
        }

        public void ClearTables()
        {
            foreach (var type in DatabaseConfig.GetTransientTypes())
            {
                DeleteAll(type);
            }
        }

        public void DeleteAll(Type type)
        {
            var name = GetTableName(type);
            Execute("delete from " + name);
        }

        public int Count(Type type)
        {
            var name = GetTableName(type);
            return ExecuteScalar<int>("select count(*) from " + name);            
        }

        public static string GetTableName(Type type)
        {
            var name = type.Name;
            foreach (var tableAttribute in type.GetCustomAttributes().OfType<TableAttribute>())
            {
                name = tableAttribute.Name;
            }
            return name;
        }

        public List<T> GetAll<T>() where T : new()
        {
            return this.GetAllWithChildren<T>(recursive:true);
        }

        public long SequenceNextValue(SequenceName sequenceName)
        {
            var sequence = Find<DatabaseSequence>(sequenceName);
            if (sequence == null)
            {
                Insert(new DatabaseSequence() { SequenceName = sequenceName, NextValue = 2 });
                return 1;
            }
            
            var value = sequence.NextValue++;
            Update(sequence);
            
            return value;
        }
    }
}