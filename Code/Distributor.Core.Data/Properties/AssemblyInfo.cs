using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Distributr.Core.Data")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("Distributr.Core.Data")]
[assembly: AssemblyCopyright("Copyright ©  2011")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("74a4ad70-197d-4c38-9523-e351994a93cd")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.xml", Watch = true)]//Distributr.Core.Data.Tests.dll.config
//[assembly: InternalsVisibleTo("Distributr.Core.Data.Tests")]
[assembly:InternalsVisibleTo("Distributr.Reports")]
[assembly:InternalsVisibleTo("Distributr.HQ.Lib")]
[assembly:InternalsVisibleTo("Distributr.HQ.Report")]
[assembly: InternalsVisibleTo("Distributr.WPF.Lib")]
[assembly: InternalsVisibleTo("Distributr.Core.Data.Tests")]
[assembly: InternalsVisibleTo("Distributr.WPF.Lib.Data")]
[assembly: InternalsVisibleTo("Distributr.Core.Data.2015.Tests")]

