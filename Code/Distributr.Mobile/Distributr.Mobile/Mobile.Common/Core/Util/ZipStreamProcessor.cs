using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Java.Util.Zip;

namespace Mobile.Common.Core.Util
{
    // This class depends on Java.Util.Zip which is not available in Shared Code Libraries such 
    // as Distributr.Mobile.Core. This means that we can't actually unit test the unzipping logic :/
    public class ZipStreamProcessor
    {
        public virtual IEnumerable<Tuple<String, String>> ProcessZipStream(Stream zipStream)
        {
            using (var zipInputStream = new ZipInputStream(zipStream))
            {
                ZipEntry zipEntry;
                while ((zipEntry = zipInputStream.NextEntry) != null)
                {
                    var fileName = Path.GetFileName(zipEntry.Name);

                    if (fileName != String.Empty)
                    {
                        using (var stream = new MemoryStream())
                        {
                            var size = 4096;
                            var data = new byte[size];
                            while (true)
                            {
                                size = zipInputStream.Read(data, 0, data.Length);
                                if (size > 0)
                                {
                                    stream.Write(data, 0, size);
                                }
                                else
                                {
                                    break;
                                }
                            }

                            var contents = Encoding.Default.GetString(stream.ToArray());

                            yield return Tuple.Create(fileName, contents);
                        }
                    }
                }
            }
        }
    }
}