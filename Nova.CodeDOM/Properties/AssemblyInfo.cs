using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Nova.CodeDOM")]
[assembly: AssemblyDescription("Nova CodeDOM loads C# solutions/projects/files and parses them into code objects which can then be searched, analyzed, modified, and saved back to the input files.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Inevitable Software")]
[assembly: AssemblyProduct("Nova.CodeDOM")]
[assembly: AssemblyCopyright("Copyright © 2007-2012 Inevitable Software, all rights reserved.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("b5657426-d41e-478b-905b-a69f5093599b")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
[assembly: AssemblyVersion("2.4.*")]

// Use Level1 security rules to avoid problems, such as .NET 4.0 bug that throws exceptions
// if .config files are read from a network drive.
[assembly: SecurityRules(SecurityRuleSet.Level1)]

