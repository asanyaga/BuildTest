using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using Distributr.Core.Resources;
using Distributr.Core.Resources.Util;

namespace Distributr.WSAPI.Lib.Services
{
    public class MessageSourceAccessor : IMessageSourceAccessor
    {
        private static Dictionary<string, string> _resources;
        private static object locker = new object();
        private static MessageSourceAccessor instance;
      //  private static ILog _logger = LogManager.GetLogger("MessageSourceAccessor");
        private MessageSourceAccessor()
        {

        }
        public static MessageSourceAccessor GetInstance(string platformType)
        {
          //  _logger.InfoFormat("GetInstance platform type {0}", platformType);
            lock (locker)
            {
                if (instance == null)
                {
                    try
                    {
                      
                        IList<string> messageresource = new List<string>();
                        var data = LocalizationResource.Distributr;
                        using (StreamReader sr = new StreamReader(new MemoryStream(data)))
                        {
                            while (sr.Peek() >= 0)
                            {
                                messageresource.Add(sr.ReadLine());
                            }
                        }

                        _resources = new Dictionary<string, string>();
                        _resources = MessageResourceHelper.Process(messageresource);
                        instance = new MessageSourceAccessor();
                    }
                    catch (Exception ex)
                    {
                      //  _logger.Error("Resource file load error", ex);
                        throw;
                    }
                }
            }
            return instance;
        }

       

        public string GetText(string code)
        {
            string text = code;
            if (code == null)
                throw new Exception("Error VC-1001");
            if (_resources.Any(s => s.Key == code.Trim()))
            {
                text = _resources.First(s => s.Key == code.Trim()).Value;
            }
            return text;
        }

       
    }
}
