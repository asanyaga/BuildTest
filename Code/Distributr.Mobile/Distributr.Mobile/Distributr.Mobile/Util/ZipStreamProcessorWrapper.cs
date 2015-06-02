using System;
using System.Collections.Generic;
using System.IO;
using Distributr.Mobile.Core.Util;
using Mobile.Common.Core.Util;

namespace Distributr.Mobile.Util
{
    public class ZipStreamProcessorWrapper : IZipStreamProcessor
    {
        private readonly ZipStreamProcessor zipStreamProcessor = new ZipStreamProcessor();

        public IEnumerable<Tuple<string, string>> ProcessZipStream(Stream zipStream)
        {
            return zipStreamProcessor.ProcessZipStream(zipStream);
        }
    }
}