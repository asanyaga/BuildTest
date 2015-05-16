namespace Distributr.WPF.Lib.Services.Util
{
    public static class Logging
    {
        public static void Log(string message, LOGLEVEL logLevel)
        {
            //try
            //{
            //    using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
            //    {
            //        string directory = "log";
            //        if(store.GetDirectoryNames(directory).Length==0)
            //        {
            //            store.CreateDirectory(directory);
            //        }

            //        string filename = string.Format(@"{0}\{1}.log",directory, DateTime.Now.ToString("yyyyMMdd"));
            //        using (Stream stream = new IsolatedStorageFileStream(filename, FileMode.Append, FileAccess.Write, store))
            //        {
            //            StreamWriter writer = new StreamWriter(stream);
            //            switch (logLevel)
            //            {
            //                case LOGLEVEL.INFO:
            //                    writer.Write(String.Format("{0:u} [INFO] {1}{2}", DateTime.Now, message, Environment.NewLine));
            //                    break;
            //                case LOGLEVEL.WARNING:
            //                    writer.Write(String.Format("{0:u} [WARNING] {1}{2}", DateTime.Now, message, Environment.NewLine));
            //                    break;
            //                case LOGLEVEL.ERROR:
            //                    writer.Write(String.Format("{0:u} [ERROR] {1}{2}", DateTime.Now, message, Environment.NewLine));
            //                    break;
            //                case LOGLEVEL.FATAL:
            //                    writer.Write(String.Format("{0:u} [FATAL] {1}{2}", DateTime.Now, message, Environment.NewLine));
            //                    break;
            //                default:
            //                    break;
            //            }
            //            writer.Close();
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //}
        }

    }


    public enum LOGLEVEL
    {
        INFO,
        WARNING,
        ERROR,
        FATAL
    }

}
