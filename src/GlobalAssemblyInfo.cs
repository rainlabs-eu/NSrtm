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

[assembly: AssemblyVersion("0.3.0.0")]
[assembly: AssemblyInformationalVersion("0.3.0-beta1-4-g56677ea-0")]
