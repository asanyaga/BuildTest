using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.Core.Utility
{
    


public static class LinqExtensions
    {
        public static string ToCsv<T>(this IEnumerable<T> items)
            where T : class
        {
            var csvBuilder = new StringBuilder();
            var properties = typeof (T).GetProperties();
            foreach (T item in items)
            {
                string line = String.Join(",", properties.Select(p => p.GetValue(item, null).ToCsvValue()).ToArray());
                csvBuilder.AppendLine(line);
            }
            return csvBuilder.ToString();
        }
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> collection, int batchSize)
        {
            List<T> nextbatch = new List<T>(batchSize);
            foreach (T item in collection)
            {
                nextbatch.Add(item);
                if (nextbatch.Count == batchSize)
                {
                    yield return nextbatch;
                    nextbatch = new List<T>(batchSize);
                }
            }
            if (nextbatch.Count > 0)
                yield return nextbatch;
        }

        public static void AddToList<T, TU>(this IDictionary<T, List<TU>> dict, T key, TU elementToList)
        {

            List<TU> list;

            bool exists = dict.TryGetValue(key, out list);

            if (exists)
            {
                dict[key].Add(elementToList);
            }
            else
            {
                dict[key] = new List<TU> { elementToList };
            }

        }
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> list, int parts)
        {
            int i = 0;
            var splits = from item in list
                         group item by i++ % parts into part
                         select part.AsEnumerable();
            return splits;
        }
        public static IList<T> ToList<T>(this DataTable table) where T : new()
        {
            IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();
            IList<T> result = new List<T>();

            foreach (var row in table.Rows)
            {
                var item = CreateItemFromRow<T>((DataRow)row, properties);
                result.Add(item);
            }

            return result;
        }

        private static T CreateItemFromRow<T>(DataRow row, IList<PropertyInfo> properties) where T : new()
        {
            T item = new T();
            foreach (var property in properties)
            {
                property.SetValue(item, row[property.Name], null);
            }
            return item;
        }

        

        private static string ToCsvValue<T>(this T item)
        {
            if (item == null) return "\"\"";

            if (item is string)
            {
                return String.Format("\"{0}\"", item.ToString().Replace("\"", "\\\""));
            }
            double dummy;
            if (Double.TryParse(item.ToString(), out dummy))
            {
                return String.Format("{0}", item);
            }
            return String.Format("\"{0}\"", item);
        }
        public static void WriteCSV<T>( this IEnumerable<T> items, string path)
        {
            Type itemType = typeof(T);
            var props = itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => !p.IsMarkedWith<IgnoreInCsvAttribute>())
                                .OrderBy(p => p.Name);

            using (var writer = new StreamWriter(path))
            {
                writer.WriteLine(string.Join(", ", props.Select(p => p.Name)));

                foreach (var item in items)
                {
                   T item1 = item;
                    writer.WriteLine(string.Join(", ", props.Select(p => Map(p, item1))));
                }
            }
        }

        private static object Map<T>(PropertyInfo propertyInfo, T item)
        {
            string datavalue = "";
            if (propertyInfo.PropertyType == typeof (string))
            {
                object value = propertyInfo.GetValue(item, null);
                if (value == null) return "''";
                return "'" + value + "'";
            }
            if (propertyInfo.PropertyType == typeof (Guid))
            {
                object value = propertyInfo.GetValue(item, null);
                if (value == null || ((Guid) value) == Guid.Empty) return "NULL";
                return "'" + value + "'";
            }
            if (propertyInfo.PropertyType == typeof(Guid?))
            {
                object value = propertyInfo.GetValue(item, null);
                if (value == null || ((Guid?)value) == Guid.Empty) return "NULL";
                return "'" + value + "'";
            }
            if (propertyInfo.PropertyType == typeof (decimal))
            {
                object value = propertyInfo.GetValue(item, null);
                if (value == null) return "NULL";
                return value;
            }
            if (propertyInfo.PropertyType == typeof(int))
            {
                object value = propertyInfo.GetValue(item, null);
                if (value == null) return "NULL";
                return value;
            }
            if (propertyInfo.PropertyType == typeof(int?))
            {
                object value = propertyInfo.GetValue(item, null);
                if (value == null) return "NULL";
                return value;
            }
            if (propertyInfo.PropertyType == typeof(DateTime))
            {
                object value = propertyInfo.GetValue(item, null);
                if (value == null) return "NULL";
                return "'"+((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss")+"'";
            }
            if (propertyInfo.PropertyType == typeof(DateTime?))
            {
                object value = propertyInfo.GetValue(item, null);
                if (value == null) return "NULL";
                return "'" + ((DateTime?)value).Value.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            }
            if (propertyInfo.PropertyType == typeof(bool))
            {
                object value = propertyInfo.GetValue(item, null);
                if (value == null) return "0";
                else if ((bool) value) return "1";
                return "0";
            }
            else
            {
                propertyInfo.GetValue(item, null);
            }
            return datavalue;
        }
    }

}
