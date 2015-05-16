using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SQLite.Net;
using SQLite.Net.Attributes;
using SQLite.Net.Interop;
using SQLiteNetExtensions.Extensions;

namespace Distributr.Mobile.Data
{
    public class Database : SQLiteConnection
    {
        public Database(ISQLitePlatform platform, string path)
            : base(platform, path)
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
            var name = type.Name;
            foreach (var tableAttribute in type.GetCustomAttributes().OfType<TableAttribute>())
            {
                name = tableAttribute.Name;
            }
            
            Execute("delete from " + name);
        }

        public List<T> GetAll<T>() where T : new()
        {
            return this.GetAllWithChildren<T>();
        }
    }
}