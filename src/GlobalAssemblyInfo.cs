﻿using System;
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

[assembly: AssemblyVersion("0.3.0.0")]
[assembly: AssemblyInformationalVersion("0.3.0-beta2-0")]
