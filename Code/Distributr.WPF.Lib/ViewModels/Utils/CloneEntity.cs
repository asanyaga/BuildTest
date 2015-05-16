using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master;

namespace Distributr.WPF.Lib.ViewModels.Utils
{
    public static class CloneEntity
    {
        /// <summary>
        /// Does note perform deep cloning
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="existing"></param>
        /// <returns></returns>
        public static T Clone<T>(this MasterEntity existing) where T : class
        {
            Type t = typeof (T);
            var fields = t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var copy = Activator.CreateInstance(t, existing.Id, existing._DateCreated, existing._DateLastUpdated, existing._Status);
            for (int i = 0; i < fields.Length; i++)
                fields[i].SetValue(copy, fields[i].GetValue(existing));

            return copy as T;
        }

        public static T DeepClone<T>(this T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }
    }
}
