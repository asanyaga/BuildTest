namespace IMPORTFOB.NAV
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices.ComTypes;

    public class StreamSupport
    {
        public static unsafe IStream ToIStream(Stream stream,
                             ref IStream comStream)
        {
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            uint num = 0;
            IntPtr pcbWritten = new IntPtr((void*)&num);
            comStream.Write(buffer, buffer.Length, pcbWritten);
            return comStream;
        }

        public static unsafe MemoryStream ToMemoryStream(IStream comStream)
        {
            MemoryStream stream = new MemoryStream();
            byte[] pv = new byte[100];
            uint num = 0;

            IntPtr pcbRead = new IntPtr((void*)&num);

            do
            {
                num = 0;
                comStream.Read(pv, pv.Length, pcbRead);
                stream.Write(pv, 0, (int)num);
            }
            while (num > 0);
            return stream;
        }
    }
}
