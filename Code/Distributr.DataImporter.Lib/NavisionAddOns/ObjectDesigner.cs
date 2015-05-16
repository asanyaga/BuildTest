using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;


namespace  IMPORTFOB.NAV
{
    using System.Runtime.InteropServices.ComTypes;
    using System.Security;
    public enum NavObjectType
    {
        TableData=6,
        Codeunit = 5,
        Dataport = 4,
        Form = 2,
        Report = 3,
        Table = 1
    }

    public class ObjectDesigner
    {
        private IObjectDesigner _objectDesigner;
        private const string DefaultMonikerName = "!C/SIDE";
        private const string ObjectDesignerGuid = "50000004-0000-1000-0001-0000836BD2D2";

        public ObjectDesigner()
        {
            Guid guid = new Guid("50000004-0000-1000-0001-0000836BD2D2");

            Hashtable runningObjects = GetActiveObjectList(DefaultMonikerName);

            foreach (DictionaryEntry de in runningObjects)
            {
                string progId = de.Key.ToString();
                if (progId.IndexOf("{") != -1)
                {
                    // Convert a class id into a friendly prog Id
                    progId = ConvertClassIdToProgId(de.Key.ToString());
                }
                object getObj = GetActiveObject(progId);
                if (getObj != null)
                {

                    this._objectDesigner = getObj as IObjectDesigner;
                    if (this._objectDesigner == null)
                    {
                        throw new Exception("Could not connect to Dynamics NAV");
                    }
                }
            }
        }

        public void CompileObject(NavObjectType navObjectType, int objectId)
        {
            int result = this._objectDesigner.CompileObject((int) navObjectType,
                                                            objectId);
            this.ProcessResult(result);
        }

        public void CompileObjects(string filter)
        {
            int result = this._objectDesigner.CompileObjects(filter);
            this.ProcessResult(result);
        }

        [DllImport("ole32.dll")]
        public static extern void CreateBindCtx(int reserved, out IBindCtx ppbc);

        [DllImport("OLE32.DLL")]
        public static extern int CreateStreamOnHGlobal(int hGlobalMemHandle,
                                                       bool fDeleteOnRelease, out IStream pOutStm);

        public string GetCompanyName()
        {
            string companyName;
            int num = this._objectDesigner.GetCompanyName(out companyName);
            return companyName;
        }

        public string GetDatabaseName()
        {
            string databaseName;
            int num = this._objectDesigner.GetDatabaseName(out databaseName);
            return databaseName;
        }

        public static Hashtable GetActiveObjectList(string filter)
        {
            Hashtable result = new Hashtable();

            IntPtr numFetched = IntPtr.Zero;
            IRunningObjectTable runningObjectTable;
            IEnumMoniker monikerEnumerator;
            IMoniker[] monikers = new IMoniker[1];

            GetRunningObjectTable(0, out runningObjectTable);
            runningObjectTable.EnumRunning(out monikerEnumerator);
            monikerEnumerator.Reset();
            Console.WriteLine("Select client:");
            int clientNo = 0;
            while (monikerEnumerator.Next(1, monikers, numFetched) == 0)
            {
                IBindCtx ctx;
                CreateBindCtx(0, out ctx);

                string runningObjectName;
                monikers[0].GetDisplayName(ctx, null, out runningObjectName);
                System.Guid classId;
                monikers[0].GetClassID(out classId);
                object runningObjectVal;
                runningObjectTable.GetObject(monikers[0], out runningObjectVal);

                if (runningObjectName.IndexOf(filter) != -1 &&
                    runningObjectName.IndexOf("company") != -1)
                {
                    clientNo += 1;
                    result[runningObjectName] = runningObjectVal;
                }
            }

            return result;
        }

        [DllImport("oleaut32.dll", CharSet = CharSet.Unicode)]
        internal static extern int GetErrorInfo(int dwReserved,
                                                [MarshalAs(UnmanagedType.Interface)] out IErrorInfo ppIErrorInfo);

        public static object GetObjectFromRot(string monikerName, Guid guid)
        {
            IRunningObjectTable prot;
            IEnumMoniker ppenumMoniker;
            IntPtr pceltFetched = IntPtr.Zero;
            IMoniker[] rgelt = new IMoniker[1];
            object ppvResult = null;
            GetRunningObjectTable(0, out prot);
            prot.EnumRunning(out ppenumMoniker);
            ppenumMoniker.Reset();
            while (ppenumMoniker.Next(1, rgelt, pceltFetched) == 0)
            {
                IBindCtx ppbc;
                string ppszDisplayName;
                CreateBindCtx(0, out ppbc);
                rgelt[0].GetDisplayName(ppbc, null, out ppszDisplayName);
                if (!(!ppszDisplayName.StartsWith(monikerName) ||
                      ppszDisplayName.Equals(monikerName)))
                {
                    rgelt[0].BindToObject(ppbc, null, ref guid, out ppvResult);
                    return ppvResult;
                }
            }
            return ppvResult;
        }

        public static object GetActiveObject(string progId)
        {
            // Convert the prog id into a class id
            string classId = ConvertProgIdToClassId(progId);

            IRunningObjectTable prot = null;
            IEnumMoniker pMonkEnum = null;
            try
            {
                IntPtr Fetched = IntPtr.Zero;
                // Open the running objects table.
                GetRunningObjectTable(0, out prot);
                prot.EnumRunning(out pMonkEnum);
                pMonkEnum.Reset();
                IMoniker[] pmon = new IMoniker[1];

                // Iterate through the results
                while (pMonkEnum.Next(1, pmon, Fetched) == 0)
                {
                    IBindCtx pCtx;
                    CreateBindCtx(0, out pCtx);
                    string displayName;
                    pmon[0].GetDisplayName(pCtx, null, out displayName);
                    Marshal.ReleaseComObject(pCtx);
                    if (displayName.IndexOf(classId) != -1)
                    {
                        // Return the matching object
                        object objReturnObject;
                        prot.GetObject(pmon[0], out objReturnObject);
                        return objReturnObject;
                    }
                }
                return null;
            }
            finally
            {
                // Free resources
                if (prot != null)
                    Marshal.ReleaseComObject(prot);
                if (pMonkEnum != null)
                    Marshal.ReleaseComObject(pMonkEnum);
            }
        }

        public static Hashtable GetRunningObjectTable()
        {
            IRunningObjectTable prot;
            IEnumMoniker ppenumMoniker;
            IntPtr pceltFetched = IntPtr.Zero;
            Hashtable hashtable = new Hashtable();
            IMoniker[] rgelt = new IMoniker[1];
            GetRunningObjectTable(0, out prot);
            prot.EnumRunning(out ppenumMoniker);
            ppenumMoniker.Reset();
            while (ppenumMoniker.Next(1, rgelt, pceltFetched) == 0)
            {
                IBindCtx ppbc;
                string ppszDisplayName;
                object ppunkObject;
                CreateBindCtx(0, out ppbc);
                rgelt[0].GetDisplayName(ppbc, null, out ppszDisplayName);
                prot.GetObject(rgelt[0], out ppunkObject);
                hashtable[ppszDisplayName] = ppunkObject;
            }
            return hashtable;
        }

        [DllImport("ole32.dll", PreserveSig = false)]
        private static extern void CLSIDFromProgIDEx(
            [MarshalAs(UnmanagedType.LPWStr)] string progId, out Guid clsid);

        [DllImport("ole32.dll", PreserveSig = false)]
        private static extern void CLSIDFromProgID(
            [MarshalAs(UnmanagedType.LPWStr)] string progId, out Guid clsid);

        public static string ConvertProgIdToClassId(string progID)
        {
            Guid testGuid;
            try
            {
                CLSIDFromProgIDEx(progID, out testGuid);
            }
            catch
            {
                try
                {
                    CLSIDFromProgID(progID, out testGuid);
                }
                catch
                {
                    return progID;
                }
            }
            return testGuid.ToString().ToUpper();
        }

        [DllImport("ole32.dll")]
        private static extern int ProgIDFromCLSID([In()] ref Guid clsid,
                                                  [MarshalAs(UnmanagedType.LPWStr)] out string lplpszProgID);

        public static string ConvertClassIdToProgId(string classID)
        {
            Guid testGuid = new Guid(classID.Replace("!", ""));
            string progId = null;
            try
            {
                ProgIDFromCLSID(ref testGuid, out progId);
            }
            catch (Exception)
            {
                return null;
            }
            return progId;
        }

        [DllImport("ole32.dll")]
        public static extern void GetRunningObjectTable(int reserved,
                                                        out IRunningObjectTable prot);

        public string GetServerName()
        {
            string serverName;
            int num = this._objectDesigner.GetServerName(out serverName);
            if (serverName != null)
                return serverName;
            else
                return string.Empty;
        }

        private void ProcessResult(int result)
        {
            if (result != 0)
            {
                IErrorInfo ppIErrorInfo = null;
                GetErrorInfo(0, out ppIErrorInfo);
                string pBstrDescription = string.Empty;
                if (ppIErrorInfo != null)
                {
                    ppIErrorInfo.GetDescription(out pBstrDescription);
                }
                string message = string.Format(CultureInfo.CurrentCulture,
                                               "Method returned an error. HRESULT = 0x{0:X8}",
                                               new object[] {result});
                if (pBstrDescription != string.Empty)
                {
                    message = message + " : " + pBstrDescription;
                }
                throw new Exception(message);
            }
        }

        public MemoryStream ReadObjectToStream(NavObjectType navObjectType,
                                               int objectId)
        {
            IStream pOutStm = null;
            CreateStreamOnHGlobal(0, true, out pOutStm);
            int result = this._objectDesigner.ReadObject((int) navObjectType,
                                                         objectId, pOutStm);
            this.ProcessResult(result);
            return this.ToMemoryStream(pOutStm);
        }

        private unsafe IStream ToIStream(Stream stream)
        {
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            uint num = 0;
            IntPtr pcbWritten = new IntPtr((void*) &num);
            IStream pOutStm = null;
            CreateStreamOnHGlobal(0, true, out pOutStm);
            pOutStm.Write(buffer, buffer.Length, pcbWritten);
            pOutStm.Seek((long) 0, 0, IntPtr.Zero);
            return pOutStm;
        }

        private unsafe MemoryStream ToMemoryStream(IStream comStream)
        {
            MemoryStream stream = new MemoryStream();
            byte[] pv = new byte[100];
            uint num = 0;
            IntPtr pcbRead = new IntPtr((void*) &num);
            comStream.Seek((long) 0, 0, IntPtr.Zero);
            do
            {
                num = 0;
                comStream.Read(pv, pv.Length, pcbRead);
                stream.Write(pv, 0, (int) num);
            } while (num > 0);
            return stream;
        }

        public void WriteObjectFromStream(Stream stream)
        {
            IStream source = this.ToIStream(stream);
            int result = this._objectDesigner.WriteObjects(source);
            this.ProcessResult(result);
        }

        [ComImport, SuppressUnmanagedCodeSecurity, Guid("1CF2B120-547D-101B-8E65-08002B2BD119"),
         InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IErrorInfo
        {
            [PreserveSig]
            int GetGUID();

            [PreserveSig]
            int GetSource([MarshalAs(UnmanagedType.BStr)] out string pBstrSource);

            [PreserveSig]
            int GetDescription(
                [MarshalAs(UnmanagedType.BStr)] out string pBstrDescription);
        }
    }


   
}
