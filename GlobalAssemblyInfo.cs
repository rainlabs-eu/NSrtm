using System;
using System.Reflection;
using System.Runtime.InteropServices;

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

[assembly: AssemblyCompany("Rainlabs")]
[assembly: AssemblyCopyright("Copyright © 2015 Rainlabs S.C.")]

[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]

[assembly: AssemblyVersion("0.2.1.*")]
[assembly: AssemblyInformationalVersion("0.2.1-beta2")]
