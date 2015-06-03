using System;
using System.Collections.Generic;
using System.IO;

namespace Distributr.Mobile.Core.Util
{
    public interface IZipStreamProcessor
    {
        // Implementers of this method should us "yeild return" so that files can
        // be processed in chunks and do not exhaust memory
        // item1 = fileName, item2 = fileContents 
        IEnumerable<Tuple<String, String>> ProcessZipStream(Stream zipStream);
    }
}