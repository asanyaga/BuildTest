using System;
using System.IO;
using Distributr.Mobile.Core.Data;

namespace Distributr.Mobile.Data
{
    public class AndroidFileSystem : IFileSystem
    {
        public string GetDatabasePath()
        {
            return Path.Combine(GetRootStorageFolder(), "distributr.db");
        }

        public string GetRootStorageFolder()
        {
            return  Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        }
    }
}