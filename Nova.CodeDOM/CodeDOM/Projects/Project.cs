﻿// The Nova Project by Ken Beckett.
// Copyright (C) 2007-2012 Inevitable Software, all rights reserved.
// Released under the Common Development and Distribution License, CDDL-1.0: http://opensource.org/licenses/cddl1.php

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

using Nova.Parsing;
using Nova.Rendering;
using Nova.Utilities;

namespace Nova.CodeDOM
{
    /// <summary>
    /// Represents a collection of <see cref="CodeUnit"/> objects (files) that
    /// are compiled together into some type of target (such as a .DLL or .EXE file).
    /// </summary>
    public class Project : CodeObject, INamedCodeObject, IFile, IComparable<Project>
    {
        #region /* STATIC FIELDS */

        /// <summary>
        /// Set to load internal types in addition to public types when loading types from referenced assemblies and
        /// projects.  This option will slow things down a bit and use up more memory.
        /// </summary>
        public static bool LoadInternalTypes;

        #endregion

        #region /* CONSTANTS */

        public const string XamlFileExtension               = ".xaml";
        public const string GeneratedFileExtension          = ".g";
        public const string DesignerGeneratedExtension      = ".Designer";
        public const string WorkflowCodeBesideFileExtension = ".xoml";

        public const string CSharpProjectFileExtension            = ".csproj";
        public const string CSharpFileExtension                   = ".cs";
        public const string XamlCSharpCodeBehindExtension         = XamlFileExtension + CSharpFileExtension;
        public const string XamlCSharpGeneratedExtension          = GeneratedFileExtension + CSharpFileExtension;
        public const string DesignerCSharpGeneratedExtension      = DesignerGeneratedExtension + CSharpFileExtension;
        public const string WorkflowCSharpCodeBesideFileExtension = WorkflowCodeBesideFileExtension + CSharpFileExtension;

        public const string VBProjectFileExtension = ".vbproj";
        public const string VBFileExtension        = ".vb";

        public const string MsCorLib         = "mscorlib";
        public const string SystemCore       = "System.Core";
        public const string DefaultFramework = DotNetFramework;

        public const string DotNetFramework          = ".NETFramework";
        public const string SilverlightFramework     = "Silverlight";
        public const string PortableLibraryFramework = ".NETPortable";

        public const string PropertiesFolder = "Properties";
        public const string ReferencesFolder = "References";
        public const string ConfigurationDebug = "Debug";
        public const string ConfigurationRelease = "Release";
        public const string PlatformAnyCPU = "AnyCPU";
        public const string PlatformX86 = "x86";
        public const string PlatformX64 = "x64";

        public const int DefaultFileAlignment = 512;
        public const int DefaultBaseAddress = 0x400000;

        public static readonly Guid FolderType = new Guid("{2150E333-8FDC-42A3-9474-1A3956D46DE8}");
        public static readonly Guid CSProjectType = new Guid("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}");
        public static readonly Guid VBProjectType = new Guid("{F184B08F-C81C-45F6-A57F-5ABD9991F28F}");
        public static readonly Guid WebSiteProjectType = new Guid("{E24C65DC-7377-472B-9ABA-BC803B73C61A}");
        public static readonly Guid WebApplicationProjectType = new Guid("{349C5851-65DF-11DA-9384-00065B846F21}");
        public static readonly Guid CPPProjectType = new Guid("{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}");
        public static readonly Guid CSWorkflowProjectType = new Guid("{14822709-B5A1-4724-98CA-57A101D1B079}");
        public static readonly Guid VBWorkflowProjectType = new Guid("{D59BE175-2ED0-4C54-BE3D-CDAA9F3214C8}");
        public static readonly Guid CSSharePointProjectType = new Guid("{593B0543-81F6-4436-BA1E-4747859CAAE2}");
        public static readonly Guid VBSharePointProjectType = new Guid("{EC05E597-79D4-47f3-ADA0-324C4F7C7484}");
        public static readonly Guid WCFProjectType = new Guid("{3D9AD99F-2412-4246-B90B-4EAA41C64699}");
        public static readonly Guid WPFProjectType = new Guid("{60DC8134-EBA5-43B8-BCC9-BB4BC16C2548}");
        public static readonly Guid SilverlightProjectType = new Guid("{A1591282-1198-4647-A2B1-27E5FF5F6F3B}");
        public static readonly Guid TestProjectType = new Guid("{3AC096D0-A1C2-E12C-1390-A8335801FDAB}");
        public static readonly Guid VisualDBToolsProjectType = new Guid("{C252FEB5-A946-4202-B1D4-9916A0590387}");
        public static readonly Guid SetupProjectType = new Guid("{54435603-DBB4-11D2-8724-00A0C9A8B90C}");
        public static readonly Guid MvcProjectType = new Guid("{F85E285D-A4E0-4152-9332-AB1D724D3325}");
        public static readonly Guid ASPMVCProjectType = new Guid("{603C0E0B-DB56-11DC-BE95-000D561079B0}");
        public static readonly Guid VstoProjectType = new Guid("{BAA0C2D2-18E2-41B9-852F-F413020CAA33}");

        #endregion

        #region /* FIELDS */

        protected string _name;
        protected bool _isNew;  // True if newly created and not saved yet
        protected string _fileName;
        protected Guid _typeGuid;

        protected string _toolsVersion;
        protected string _defaultTargets;
        protected string _configurationName;
        protected string _platform;
        protected string _outputPath;  // Normally at the configuration level, but can also be specified at the project level
        protected string _namespace;
        protected string _productVersion;
        protected string _schemaVersion;
        protected Guid _projectGuid;
        protected List<Guid> _projectTypeGuids;
        protected OutputTypes _outputType;
        protected string _startupObject;
        protected bool? _noStandardLibraries;
        protected string _appDesignerFolder;
        protected string _rootNamespace;
        protected string _assemblyName;
        protected string _deploymentDirectory;
        protected string _startArguments;
        protected string _targetFrameworkIdentifier;
        protected string _targetFrameworkVersion;
        protected string _targetFrameworkProfile;
        protected int _fileAlignment;
        protected int? _warningLevel;  // Why does this show up sometimes at the main level?
        protected bool? _signAssembly;
        protected string _assemblyOriginatorKeyFile;
        protected string _referencePath;
        protected string _sccProjectName;
        protected string _sccLocalPath;
        protected string _sccAuxPath;
        protected string _sccProvider;
        protected string _fileUpgradeFlags;
        protected string _oldToolsVersion;
        protected string _upgradeBackupLocation;
        protected string _projectType;

        protected string _silverlightVersion;
        protected bool? _silverlightApplication;
        protected string _supportedCultures;
        protected bool? _xapOutputs;
        protected bool? _generateSilverlightManifest;
        protected string _xapFilename;
        protected string _silverlightManifestTemplate;
        protected string _silverlightAppEntry;
        protected string _testPageFileName;
        protected bool? _createTestPage;
        protected bool? _validateXaml;
        protected bool? _enableOutOfBrowser;
        protected string _outOfBrowserSettingsFile;
        protected bool? _usePlatformExtensions;
        protected bool? _throwErrorsInValidation;
        protected string _linkedServerProject;
        protected bool? _mvcBuildViews;
        protected bool? _useIISExpress;
        protected string _silverlightApplicationList;
        protected bool? _nonShipping;

        /// <summary>
        /// The ReferencePath from any "*.csproj.user" file.
        /// </summary>
        protected string _userReferencePath;

        /// <summary>
        /// The project configurations.
        /// </summary>
        protected ChildList<Configuration> _configurations;

        /// <summary>
        /// The currently active project configuration.
        /// </summary>
        protected Configuration _currentConfiguration;

        /// <summary>
        /// True if the project type is not currently supported.
        /// </summary>
        protected bool _notSupported;

        /// <summary>
        /// All code units in this project.
        /// </summary>
        protected ChildList<CodeUnit> _codeUnits;

        /// <summary>
        /// References to other projects, assemblies, or COM objects.
        /// </summary>
        protected ChildList<Reference> _references;

        /// <summary>
        /// File items (source files, content files, resource files, etc).
        /// </summary>
        protected ChildList<FileItem> _fileItems;

        /// <summary>
        /// Unhandled (unparsed or unrecognized) XML data in the project file.
        /// </summary>
        protected List<UnhandledData> _unhandledData = new List<UnhandledData>();

        /// <summary>
        /// All project-global attributes (those with a target of <b>assembly</b> or <b>module</b>) defined in any <see cref="CodeUnit"/> (usually AssemblyInfo.cs).
        /// </summary>
        protected List<Attribute> _globalAttributes = new List<Attribute>();

        /// <summary>
        /// The project's global namespace.
        /// </summary>
        protected RootNamespace _globalNamespace;

        /// <summary>
        /// A cached dictionary of projects which the current one depends on (used for performance).
        /// </summary>
        protected HashSet<Project> _dependsOn;

        #endregion

        #region /* CONSTRUCTORS */

        /// <summary>
        /// Create a new <see cref="Project"/> with the specified file name and parent <see cref="Solution"/>.
        /// </summary>
        /// <remarks>
        /// To load an existing project, use <see cref="Load(string, LoadOptions, Action{LoadStatus, CodeObject})"/>.
        /// </remarks>
        public Project(string fileName, Solution solution)
        {
            _parent = solution;
            _name = Path.GetFileNameWithoutExtension(fileName);
            _isNew = true;
            _fileName = fileName;
            if (!Path.HasExtension(fileName))
                _fileName += CSharpProjectFileExtension;
            FileEncoding = Encoding.UTF8;  // Default to UTF8 encoding with a BOM
            FileHasUTF8BOM = true;

            _toolsVersion = "4.0";  // Visual Studio 2010
            _defaultTargets = "Build";
            _namespace = "http://schemas.microsoft.com/developer/msbuild/2003";
            _productVersion = "9.0.21022";  // Visual Studio 2010
            _schemaVersion = "2.0";
            _projectGuid = Guid.NewGuid();
            _outputType = OutputTypes.Library;  // Default to library
            _appDesignerFolder = PropertiesFolder;
            _rootNamespace = _name;
            _assemblyName = _rootNamespace;
            _targetFrameworkVersion = "4.0";  // Default to .NETFramework 4.0
            _fileAlignment = DefaultFileAlignment;

            Initialize();
            AddDefaultConfigurations();
            _configurationName = ((solution != null && solution.ActiveConfiguration != null) ? solution.ActiveConfiguration : ConfigurationDebug);
            _platform = PlatformAnyCPU;
            _currentConfiguration = FindConfiguration(_configurationName, _platform);
        }

        #endregion

        #region /* STATIC CONSTRUCTOR */

        static Project()
        {
            // Force a reference to CodeObject to trigger the loading of any config file if it hasn't been done yet
            ForceReference();
        }

        #endregion

        #region /* PROPERTIES */

        /// <summary>
        /// The name of the <see cref="Project"/> (does not include the file path or extension).
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// True if the <see cref="Project"/> is newly created and hasn't been saved yet.
        /// </summary>
        public bool IsNew
        {
            get { return _isNew; }
        }

        /// <summary>
        /// The associated file name of the <see cref="Project"/>.
        /// </summary>
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        /// <summary>
        /// The GUID representing the type of the <see cref="Project"/>.
        /// </summary>
        public Guid TypeGuid
        {
            get { return _typeGuid; }
        }

        /// <summary>
        /// True if the project is a 'website' project - meaning a web-based project that has no project file
        /// and appears in the solution tree as an 'http:\' URL instead of just a project name.
        /// </summary>
        public bool IsWebSiteProject
        {
            get { return (_typeGuid == WebSiteProjectType); }
        }

        /// <summary>
        /// True if the associated file exists.
        /// </summary>
        public bool FileExists
        {
            get { return File.Exists(_fileName); }
        }

        /// <summary>
        /// The encoding of the file (normally UTF8).
        /// </summary>
        public Encoding FileEncoding { get; set; }

        /// <summary>
        /// True if the file has a UTF8 byte-order-mark.
        /// </summary>
        public bool FileHasUTF8BOM { get; set; }

        /// <summary>
        /// Always <c>false</c> for a <see cref="Project"/>.  Can't be set.
        /// </summary>
        public bool FileUsingTabs
        {
            get { return false; }
            set { throw new Exception("Project files never use tabs!"); }
        }

        public string ToolsVersion
        {
            get { return _toolsVersion; }
            set { _toolsVersion = value; }
        }

        public string DefaultTargets
        {
            get { return _defaultTargets; }
            set { _defaultTargets = value; }
        }

        public string XmlNamespace
        {
            get { return _namespace; }
            set { _namespace = value; }
        }

        public string ProductVersion
        {
            get { return _productVersion; }
            set { _productVersion = value; }
        }

        public string SchemaVersion
        {
            get { return _schemaVersion; }
            set { _schemaVersion = value; }
        }

        public Guid ProjectGuid
        {
            get { return _projectGuid; }
        }

        public List<Guid> ProjectTypeGuids
        {
            get { return _projectTypeGuids; }
            set { _projectTypeGuids = value; }
        }

        public OutputTypes OutputType
        {
            get { return _outputType; }
            set { _outputType = value; }
        }

        public string StartupObject
        {
            get { return _startupObject; }
            set { _startupObject = value; }
        }

        public bool? NoStandardLibraries
        {
            get { return _noStandardLibraries; }
            set { _noStandardLibraries = value; }
        }

        public string AppDesignerFolder
        {
            get { return _appDesignerFolder; }
            set { _appDesignerFolder = value; }
        }

        public string RootNamespace
        {
            get { return _rootNamespace; }
            set { _rootNamespace = value; }
        }

        public string AssemblyName
        {
            get { return _assemblyName; }
            set { _assemblyName = value; }
        }

        public string DeploymentDirectory
        {
            get { return _deploymentDirectory; }
            set { _deploymentDirectory = value; }
        }

        public string StartArguments
        {
            get { return _startArguments; }
            set { _startArguments = value; }
        }

        public string TargetFrameworkIdentifier
        {
            get
            {
                if (_targetFrameworkIdentifier != null)
                    return _targetFrameworkIdentifier;
                if (_silverlightVersion != null)
                    return SilverlightFramework;
                if (_targetFrameworkProfile != null && _targetFrameworkProfile.StartsWith("Profile"))  // Profile1..4
                    return PortableLibraryFramework;
                return DefaultFramework;
            }
            set { _targetFrameworkIdentifier = value; }
        }

        public string TargetFrameworkVersion
        {
            get
            {
                if (_targetFrameworkVersion != null)
                    return _targetFrameworkVersion;
                // Default the target framework version to the ToolsVersion
                if (_toolsVersion != null)
                    return _toolsVersion;
                // Worst case, default to 2.0 (VS 2005 default)
                return "2.0";
            }
            set { _targetFrameworkVersion = value; }
        }

        public string TargetFrameworkProfile
        {
            get { return _targetFrameworkProfile; }
            set { _targetFrameworkProfile = value; }
        }

        public int FileAlignment
        {
            get { return _fileAlignment; }
            set { _fileAlignment = value; }
        }

        public int? WarningLevel
        {
            get { return _warningLevel; }
            set { _warningLevel = value; }
        }

        public bool? SignAssembly
        {
            get { return _signAssembly; }
            set { _signAssembly = value; }
        }

        public string ReferencePath
        {
            get { return _referencePath; }
            set { _referencePath = value; }
        }

        public string SccProjectName
        {
            get { return _sccProjectName; }
            set { _sccProjectName = value; }
        }

        public string SccLocalPath
        {
            get { return _sccLocalPath; }
            set { _sccLocalPath = value; }
        }

        public string SccAuxPath
        {
            get { return _sccAuxPath; }
            set { _sccAuxPath = value; }
        }

        public string SccProvider
        {
            get { return _sccProvider; }
            set { _sccProvider = value; }
        }

        public string ProjectType
        {
            get { return _projectType; }
            set { _projectType = value; }
        }

        public string SilverlightVersion
        {
            get { return _silverlightVersion; }
            set { _silverlightVersion = value; }
        }

        public bool? SilverlightApplication
        {
            get { return _silverlightApplication; }
            set { _silverlightApplication = value; }
        }

        public string SupportedCultures
        {
            get { return _supportedCultures; }
            set { _supportedCultures = value; }
        }

        public bool? XapOutputs
        {
            get { return _xapOutputs; }
            set { _xapOutputs = value; }
        }

        public bool? GenerateSilverlightManifest
        {
            get { return _generateSilverlightManifest; }
            set { _generateSilverlightManifest = value; }
        }

        public string XapFilename
        {
            get { return _xapFilename; }
            set { _xapFilename = value; }
        }

        public string SilverlightManifestTemplate
        {
            get { return _silverlightManifestTemplate; }
            set { _silverlightManifestTemplate = value; }
        }

        public string SilverlightAppEntry
        {
            get { return _silverlightAppEntry; }
            set { _silverlightAppEntry = value; }
        }

        public string TestPageFileName
        {
            get { return _testPageFileName; }
            set { _testPageFileName = value; }
        }

        public bool? CreateTestPage
        {
            get { return _createTestPage; }
            set { _createTestPage = value; }
        }

        public bool? ValidateXaml
        {
            get { return _validateXaml; }
            set { _validateXaml = value; }
        }

        public bool? EnableOutOfBrowser
        {
            get { return _enableOutOfBrowser; }
            set { _enableOutOfBrowser = value; }
        }

        public string OutOfBrowserSettingsFile
        {
            get { return _outOfBrowserSettingsFile; }
            set { _outOfBrowserSettingsFile = value; }
        }

        public bool? UsePlatformExtensions
        {
            get { return _usePlatformExtensions; }
            set { _usePlatformExtensions = value; }
        }

        public bool? ThrowErrorsInValidation
        {
            get { return _throwErrorsInValidation; }
            set { _throwErrorsInValidation = value; }
        }

        public string LinkedServerProject
        {
            get { return _linkedServerProject; }
            set { _linkedServerProject = value; }
        }

        public bool? MvcBuildViews
        {
            get { return _mvcBuildViews; }
            set { _mvcBuildViews = value; }
        }

        public bool? UseIISExpress
        {
            get { return _useIISExpress; }
            set { _useIISExpress = value; }
        }

        public string SilverlightApplicationList
        {
            get { return _silverlightApplicationList; }
            set { _silverlightApplicationList = value; }
        }

        public bool? NonShippping
        {
            get { return _nonShipping; }
            set { _nonShipping = value; }
        }

        /// <summary>
        /// The ReferencePath from any "*.csproj.user" file.
        /// </summary>
        public string UserReferencePath
        {
            get { return _userReferencePath; }
        }

        /// <summary>
        /// All of the <see cref="Configuration"/>s for the <see cref="Project"/>.
        /// </summary>
        public ChildList<Configuration> Configurations
        {
            get { return _configurations; }
        }

        /// <summary>
        /// The currently active <see cref="Configuration"/>.
        /// </summary>
        public Configuration CurrentConfiguration
        {
            get { return _currentConfiguration; }
            set { _currentConfiguration = value; }
        }

        /// <summary>
        /// The name of the current <see cref="Configuration"/>.
        /// </summary>
        public string ConfigurationName
        {
            get { return (_currentConfiguration != null ? _currentConfiguration.Name : _configurationName); }
        }

        /// <summary>
        /// The platform of the current <see cref="Configuration"/>.
        /// </summary>
        public string ConfigurationPlatform
        {
            get { return (_currentConfiguration != null ? _currentConfiguration.Platform : _platform); }
        }

        /// <summary>
        /// The output path of the <see cref="Project"/>.
        /// </summary>
        public string OutputPath
        {
            get { return (_currentConfiguration != null ? _currentConfiguration.OutputPath ?? _outputPath : _outputPath); }
        }

        /// <summary>
        /// True if the project type is not currently supported.
        /// </summary>
        public bool NotSupported
        {
            get { return _notSupported; }
        }

        /// <summary>
        /// The parent <see cref="Solution"/>.
        /// </summary>
        public Solution Solution
        {
            get { return _parent as Solution; }
            set { _parent = value; }
        }

        /// <summary>
        /// All of <see cref="CodeUnit"/>s in the <see cref="Project"/>.
        /// </summary>
        public ChildList<CodeUnit> CodeUnits
        {
            get { return _codeUnits; }
        }

        /// <summary>
        /// All of <see cref="FileItem"/>s in the <see cref="Project"/>.
        /// </summary>
        public ChildList<FileItem> FileItems
        {
            get { return _fileItems; }
        }

        /// <summary>
        /// All of the references for the <see cref="Project"/>.
        /// </summary>
        public ChildList<Reference> References
        {
            get { return _references; }
        }

        /// <summary>
        /// The descriptive category of the code object.
        /// </summary>
        public string Category
        {
            get { return "project"; }
        }

        /// <summary>
        /// The global namespace for the project.
        /// </summary>
        public RootNamespace GlobalNamespace
        {
            get { return _globalNamespace; }
        }

        /// <summary>
        /// All project (assembly) level attributes defined in any code unit (such as AssemblyInfo.cs).
        /// </summary>
        public List<Attribute> GlobalAttributes
        {
            get { return _globalAttributes; }
        }

        #endregion

        #region /* METHODS */

        protected void Initialize()
        {
            _globalNamespace = new RootNamespace(ExternAlias.GlobalName, this);  // Setup the 'global' namespace
            _configurations = new ChildList<Configuration>(this);
            _references = new ChildList<Reference>(this);
            _codeUnits = new ChildList<CodeUnit>(this);
            _fileItems = new ChildList<FileItem>(this);
        }

        /// <summary>
        /// Set the current configuration using the specified index.
        /// </summary>
        public void SetCurrentConfiguration(int index)
        {
            _currentConfiguration = _configurations[0];
        }

        /// <summary>
        /// Find a <see cref="CodeUnit"/> by name.
        /// </summary>
        public CodeUnit FindCodeUnit(string name)
        {
            return Enumerable.FirstOrDefault(_codeUnits, delegate(CodeUnit codeUnit) { return StringUtil.NNEqualsIgnoreCase(codeUnit.Name, name); });
        }

        /// <summary>
        /// Find any configuration with the specified configuration name and platform name.
        /// </summary>
        /// <returns>The <see cref="Configuration"/> object if found, otherwise null.</returns>
        public Configuration FindConfiguration(string configurationName, string platformName)
        {
            foreach (Configuration configuration in _configurations)
            {
                if (configuration.Name == configurationName && (configuration.Platform == platformName
                    || configuration.Platform == null || (configuration.Platform == PlatformAnyCPU && platformName == null)))
                    return configuration;
            }
            return null;
        }

        /// <summary>
        /// Find the <see cref="RootNamespace"/> for the <see cref="Reference"/> with the specified alias name.
        /// </summary>
        public RootNamespace FindReferenceAliasNamespace(string referenceAliasName)
        {
            foreach (Reference reference in References)
            {
                if (reference.Alias == referenceAliasName)
                    return reference.AliasNamespace;
            }
            return null;
        }

        /// <summary>
        /// Determine the <see cref="RootNamespace"/> from any '::' prefix on the specified name, defaulting to the global namespace.
        /// </summary>
        protected RootNamespace GetRootNamespace(ref string name)
        {
            RootNamespace rootNamespace = null;
            int index = name.IndexOf(Lookup.ParseToken);
            if (index > 0)
            {
                string rootNamespaceName = name.Substring(0, index);
                name = name.Substring(index + Lookup.ParseToken.Length);
                rootNamespace = FindReferenceAliasNamespace(rootNamespaceName);
            }
            return (rootNamespace ?? _globalNamespace);
        }

        /// <summary>
        /// Get any Solution folders for this Project.
        /// </summary>
        public string GetSolutionFolders()
        {
            return (Solution != null ? Solution.GetSolutionFolders(this) : null);
        }

        /// <summary>
        /// Find the <see cref="Namespace"/> with the fully-specified name.
        /// </summary>
        public Namespace FindNamespace(string namespaceFullName)
        {
            RootNamespace rootNamespace = GetRootNamespace(ref namespaceFullName);
            return rootNamespace.FindNamespace(namespaceFullName);
        }

        /// <summary>
        /// Find a namespace or type with the fully-specified name.
        /// </summary>
        /// <returns>A <see cref="Namespace"/>, <see cref="TypeDecl"/>, or <see cref="Type"/> object.</returns>
        public object Find(string fullName)
        {
            RootNamespace rootNamespace = GetRootNamespace(ref fullName);
            return rootNamespace.Find(fullName);
        }

        /// <summary>
        /// Find a namespace or type in the global namespace with the fully specified name, returning a <see cref="SymbolicRef"/> to it.
        /// </summary>
        /// <param name="name">The namespace or type name (may include namespace and/or parent type prefixes).</param>
        /// <param name="isFirstOnLine">True if the returned <see cref="SymbolicRef"/> should be formatted as first-on-line.</param>
        /// <returns>A <see cref="NamespaceRef"/>, <see cref="TypeRef"/>, or an <see cref="UnresolvedRef"/> if no match was found.</returns>
        public SymbolicRef FindRef(string name, bool isFirstOnLine)
        {
            RootNamespace rootNamespace = GetRootNamespace(ref name);
            object obj = rootNamespace.Find(name);
            if (obj is Namespace)
                return new NamespaceRef((Namespace)obj, isFirstOnLine);
            if (obj is TypeDecl)
                return new TypeRef((TypeDecl)obj, isFirstOnLine);
            if (obj is Type)
                return new TypeRef((Type)obj, isFirstOnLine);
            return new UnresolvedRef(name, isFirstOnLine);
        }

        /// <summary>
        /// Find a namespace or type in the global namespace with the fully specified name, returning a <see cref="SymbolicRef"/> to it.
        /// </summary>
        /// <param name="name">The namespace or type name (may include namespace and/or parent type prefixes).</param>
        /// <returns>A <see cref="NamespaceRef"/>, <see cref="TypeRef"/>, or an <see cref="UnresolvedRef"/> if no match was found.</returns>
        public SymbolicRef FindRef(string name)
        {
            return FindRef(name, false);
        }

        /// <summary>
        /// Get an enumerator for all <see cref="TypeDecl"/>s declared in the <see cref="Project"/>
        /// (does not include TypeDecls imported from other Projects).
        /// </summary>
        public IEnumerable<TypeDecl> GetAllDeclaredTypeDecls(bool includeNestedTypes)
        {
            return Enumerable.SelectMany<CodeUnit, TypeDecl>(_codeUnits, delegate(CodeUnit codeUnit) { return codeUnit.GetTypeDecls(true, includeNestedTypes); });
        }

        /// <summary>
        /// Get an enumerator for all <see cref="TypeDecl"/>s declared in the <see cref="Project"/>
        /// (does not include TypeDecls imported from other Projects).
        /// </summary>
        public IEnumerable<TypeDecl> GetAllDeclaredTypeDecls()
        {
            return Enumerable.SelectMany<CodeUnit, TypeDecl>(_codeUnits, delegate(CodeUnit codeUnit) { return codeUnit.GetTypeDecls(true, false); });
        }

        /// <summary>
        /// Parse the specified name into a <see cref="NamespaceRef"/> or <see cref="TypeRef"/>, or a <see cref="Dot"/> or <see cref="Lookup"/> expression that evaluates to one.
        /// </summary>
        public Expression ParseName(string fullName)
        {
            RootNamespace rootNamespace = GetRootNamespace(ref fullName);
            return rootNamespace.ParseName(fullName);
        }

        /// <summary>
        /// Add a reference object.
        /// </summary>
        protected void AddReference(Reference reference, Action<LoadStatus, CodeObject> statusCallback)
        {
            bool noStdLib = (_currentConfiguration != null && _currentConfiguration.NoStdLib);

            // All projects reference 'mscorlib' implicitly, so add it now if we don't have any references yet.
            // We must do this here and not in the constructor of Project, because the type of the 'mscorlib'
            // library is determined by the targeted framework version, which must be parsed or set first.
            if (References.Count == 0 && !noStdLib)
                AddImplicitMscorlibReference();

            // Ignore any manually added 'mscorlib' reference
            if (!StringUtil.NNEqualsIgnoreCase(reference.Name, MsCorLib) || noStdLib)
            {
                References.Add(reference);
                if (statusCallback != null)
                    statusCallback(LoadStatus.ObjectCreated, reference);
            }
        }

        /// <summary>
        /// Add a reference object.
        /// </summary>
        protected void AddReference(Reference reference)
        {
            AddReference(reference, null);
        }

        /// <summary>
        /// Add an assembly reference by name.
        /// </summary>
        /// <param name="name">The short name or display name of the assembly.</param>
        /// <param name="alias">The alias for the referenced assembly, if any.</param>
        /// <param name="requiredTargetFrameworkVersion">The required target framework version for the assembly (only used for framework assemblies).</param>
        /// <param name="isHidden">True if the assembly reference should be hidden in the UI.</param>
        /// <param name="hintPath">The full path, including the file name, of where the assembly is expected to be (not required if the assembly is in the GAC).</param>
        /// <param name="specificVersion">True if the specific version specified in the display name should be used.</param>
        /// <summary>
        /// The optional parameters basically mirror settings used in the '.csproj' file.  The name is normally the file name of the
        /// assembly without the extension (short name), or can be a "display name" which includes a version number and other optional
        /// values.  If the assembly is not in the project's output directory or the GAC, it will require a hint-path, which can be
        /// either relative or absolute, and should include the file name and extension to be compatible with the standard '.csproj'
        /// files.  You can also use a path on the name itself, but standard '.csproj' files use the hint-path instead.
        /// </summary>
        public void AddAssemblyReference(string name, string alias, string requiredTargetFrameworkVersion, bool isHidden, string hintPath, bool specificVersion)
        {
            AddReference(new AssemblyReference(name, alias, requiredTargetFrameworkVersion, isHidden, hintPath, specificVersion));
        }

        /// <summary>
        /// Add an assembly reference by name.
        /// </summary>
        /// <param name="name">The short name or display name of the assembly.</param>
        /// <param name="alias">The alias for the referenced assembly, if any.</param>
        /// <param name="requiredTargetFrameworkVersion">The required target framework version for the assembly (only used for framework assemblies).</param>
        /// <param name="isHidden">True if the assembly reference should be hidden in the UI.</param>
        /// <param name="hintPath">The full path, including the file name, of where the assembly is expected to be (not required if the assembly is in the GAC).</param>
        /// <summary>
        /// The optional parameters basically mirror settings used in the '.csproj' file.  The name is normally the file name of the
        /// assembly without the extension (short name), or can be a "display name" which includes a version number and other optional
        /// values.  If the assembly is not in the project's output directory or the GAC, it will require a hint-path, which can be
        /// either relative or absolute, and should include the file name and extension to be compatible with the standard '.csproj'
        /// files.  You can also use a path on the name itself, but standard '.csproj' files use the hint-path instead.
        /// </summary>
        public void AddAssemblyReference(string name, string alias, string requiredTargetFrameworkVersion, bool isHidden, string hintPath)
        {
            AddReference(new AssemblyReference(name, alias, requiredTargetFrameworkVersion, isHidden, hintPath));
        }

        /// <summary>
        /// Add an assembly reference by name.
        /// </summary>
        /// <param name="name">The short name or display name of the assembly.</param>
        /// <param name="alias">The alias for the referenced assembly, if any.</param>
        /// <param name="requiredTargetFrameworkVersion">The required target framework version for the assembly (only used for framework assemblies).</param>
        /// <param name="isHidden">True if the assembly reference should be hidden in the UI.</param>
        /// <summary>
        /// The optional parameters basically mirror settings used in the '.csproj' file.  The name is normally the file name of the
        /// assembly without the extension (short name), or can be a "display name" which includes a version number and other optional
        /// values.  If the assembly is not in the project's output directory or the GAC, it will require a hint-path, which can be
        /// either relative or absolute, and should include the file name and extension to be compatible with the standard '.csproj'
        /// files.  You can also use a path on the name itself, but standard '.csproj' files use the hint-path instead.
        /// </summary>
        public void AddAssemblyReference(string name, string alias, string requiredTargetFrameworkVersion, bool isHidden)
        {
            AddReference(new AssemblyReference(name, alias, requiredTargetFrameworkVersion, isHidden));
        }

        /// <summary>
        /// Add an assembly reference by name.
        /// </summary>
        /// <param name="name">The short name or display name of the assembly.</param>
        /// <param name="alias">The alias for the referenced assembly, if any.</param>
        /// <param name="requiredTargetFrameworkVersion">The required target framework version for the assembly (only used for framework assemblies).</param>
        /// <summary>
        /// The optional parameters basically mirror settings used in the '.csproj' file.  The name is normally the file name of the
        /// assembly without the extension (short name), or can be a "display name" which includes a version number and other optional
        /// values.  If the assembly is not in the project's output directory or the GAC, it will require a hint-path, which can be
        /// either relative or absolute, and should include the file name and extension to be compatible with the standard '.csproj'
        /// files.  You can also use a path on the name itself, but standard '.csproj' files use the hint-path instead.
        /// </summary>
        public void AddAssemblyReference(string name, string alias, string requiredTargetFrameworkVersion)
        {
            AddReference(new AssemblyReference(name, alias, requiredTargetFrameworkVersion));
        }

        /// <summary>
        /// Add an assembly reference by name.
        /// </summary>
        /// <param name="name">The short name or display name of the assembly.</param>
        /// <param name="hintPath">The full path, including the file name, of where the assembly is expected to be (not required if the assembly is in the GAC).</param>
        public void AddAssemblyReference(string name, string hintPath)
        {
            AddReference(new AssemblyReference(name, hintPath));
        }

        /// <summary>
        /// Add an assembly reference by name.
        /// </summary>
        /// <param name="name">The short name or display name of the assembly.</param>
        public void AddAssemblyReference(string name)
        {
            AddReference(new AssemblyReference(name));
        }

        /// <summary>
        /// Add the implicit reference to 'mscorlib'.
        /// </summary>
        protected void AddImplicitMscorlibReference()
        {
            References.Add(new AssemblyReference(MsCorLib) { IsHidden = true });
        }

        /// <summary>
        /// Add an implicit reference to System.Core if required.
        /// </summary>
        protected void AddImplicitSystemCoreReferenceIfNecessary()
        {
            // Visual Studio 2010 adds an implicit reference to System.Core when targeting framework 3.5 or
            // higher if it doesn't already exist (it also has a bug where it auto-adds the reference to new
            // projects, and lets you remove it, but trying to add it back produces an error).
            // Apparently, the ProductVersion isn't updated for converted projects (only the SLN is updated), so
            // go by the FormatVersion of the solution file instead (11.00 = VS2010 = v10, 10.00 = VS2008 = v9,
            // 9.00 = VS2005 = v8, 8.00 = VS2003 = v7.1, ? = VS2002 = v7).
            bool isVS2010orLater = (_parent == null || GACUtil.CompareVersions(Solution.FormatVersion, "11.00") >= 0);
            if (isVS2010orLater && _targetFrameworkVersion != null)
            {
                if (!Enumerable.Any(References, delegate(Reference referenceBase) { return referenceBase is AssemblyReference && StringUtil.NNEqualsIgnoreCase(referenceBase.ShortName, SystemCore); })
                    && GACUtil.CompareVersions(_targetFrameworkVersion, "3.5") >= 0)
                    AddAssemblyReference(SystemCore, null, null, true);
            }
        }

        /// <summary>
        /// Add an existing file to the project.
        /// </summary>
        /// <returns>The new CodeUnit object.</returns>
        public CodeUnit AddFile(string fileName, bool isGenerated, Action<LoadStatus, CodeObject> statusCallback)
        {
            bool noStdLib = (_currentConfiguration != null && _currentConfiguration.NoStdLib);

            // All projects reference 'mscorlib' implicitly, so add it now if we don't have any references yet.
            // We must do this here and not in the constructor of Project, because the type of the 'mscorlib'
            // library is determined by the targeted framework version, which must be parsed or set first.
            if (References.Count == 0 && !noStdLib)
                AddImplicitMscorlibReference();

            CodeUnit codeUnit = new CodeUnit(fileName, this) { IsGenerated = isGenerated };
            AddCodeUnit(codeUnit);
            if (statusCallback != null)
                statusCallback(LoadStatus.ObjectCreated, codeUnit);

            return codeUnit;
        }

        /// <summary>
        /// Add an existing file to the project.
        /// </summary>
        /// <returns>The new CodeUnit object.</returns>
        public CodeUnit AddFile(string fileName, bool isGenerated)
        {
            return AddFile(fileName, isGenerated, null);
        }

        /// <summary>
        /// Add an existing file to the project.
        /// </summary>
        /// <returns>The new CodeUnit object.</returns>
        public CodeUnit AddFile(string fileName)
        {
            return AddFile(fileName, false, null);
        }

        /// <summary>
        /// Add a <see cref="CodeUnit"/> to the <see cref="Project"/>, keeping the <see cref="CodeUnits"/> collection sorted alphabetically.
        /// </summary>
        public void AddCodeUnit(CodeUnit codeUnit)
        {
            // Insert the CodeUnit at the proper index according to its path and filename.
            // Duplicate paths shouldn't exist, but names might for CodeUnits that aren't mapped to files, so just insert
            // any duplicate matches following the existing one.
            int index = _codeUnits.BinarySearch(codeUnit);
            _codeUnits.Insert((index < 0 ? ~index : index + 1), codeUnit);
        }

        /// <summary>
        /// Create a new <see cref="CodeUnit"/>, and add it to the <see cref="Project"/> along with a corresponding <see cref="FileItem"/>.
        /// </summary>
        /// <returns>The new <see cref="CodeUnit"/> object.</returns>
        public CodeUnit CreateCodeUnit(string fileName)
        {
            CodeUnit codeUnit = new CodeUnit(fileName, this);
            AddCodeUnit(codeUnit);
            _fileItems.Add(new FileItem(BuildActions.Compile, codeUnit.FileName));
            return codeUnit;
        }

        /// <summary>
        /// Create a new <see cref="CodeUnit"/> for a code fragment, and add it to the <see cref="Project"/>.
        /// </summary>
        /// <param name="codeFragment">The code fragment.</param>
        /// <param name="fileName">The file name.</param>
        /// <returns>The new CodeUnit object.</returns>
        public CodeUnit CreateCodeUnit(string codeFragment, string fileName)
        {
            CodeUnit codeUnit = new CodeUnit(fileName, codeFragment, this);
            AddCodeUnit(codeUnit);
            return codeUnit;
        }

        /// <summary>
        /// Remove the specified <see cref="CodeUnit"/> from the <see cref="Project"/>, also removing the corresponding <see cref="FileItem"/>.
        /// </summary>
        /// <param name="codeUnit">The <see cref="CodeUnit"/> to be removed.</param>
        public void RemoveCodeUnit(CodeUnit codeUnit)
        {
            _codeUnits.Remove(codeUnit);
            _fileItems.RemoveAll(delegate(FileItem x) { return x.FileName == codeUnit.FileName; });
        }

        /// <summary>
        /// Rename the file name of the specified <see cref="CodeUnit"/> in the <see cref="Project"/>, also renaming the corresponding <see cref="FileItem"/>.
        /// </summary>
        /// <param name="codeUnit">The <see cref="CodeUnit"/> to be renamed.</param>
        /// <param name="newfileName">The new file name.</param>
        public void RenameCodeUnit(CodeUnit codeUnit, string newfileName)
        {
            foreach (FileItem fileItem in _fileItems)
            {
                if (fileItem.FileName == codeUnit.FileName)
                    fileItem.FileName = newfileName;
            }
            codeUnit.Name = Path.GetFileName(newfileName);
            codeUnit.FileName = newfileName;
        }

        /// <summary>
        /// Compare one <see cref="Project"/> to another.
        /// </summary>
        public int CompareTo(Project project)
        {
            // Sort by name only (not path)
            return _name.CompareTo(project.Name);
        }

        private void AddDependencies(Project project)
        {
            foreach (Reference reference in project.References)
            {
                if (reference is ProjectReference)
                {
                    Project referencedProject = ((ProjectReference)reference).ReferencedProject;
                    if (referencedProject != null)
                    {
                        _dependsOn.Add(referencedProject);
                        AddDependencies(referencedProject);
                    }
                }
            }
        }

        /// <summary>
        /// Determine if this project depends on the specified project.
        /// </summary>
        public bool IsDependentOn(Project project)
        {
            if (_dependsOn == null)
            {
                // Create a cached dictionary of dependencies if it doesn't exist yet
                _dependsOn = new HashSet<Project>();
                AddDependencies(this);
            }
            return _dependsOn.Contains(project);
        }

        /// <summary>
        /// Determine if a compiler directive symbol is defined for the current configuration.
        /// </summary>
        public bool IsCompilerDirectiveSymbolDefined(string name)
        {
            return (_currentConfiguration != null && _currentConfiguration.IsConstantDefined(name));
        }

        /// <summary>
        /// Define a compiler directive symbol for the current configuration.
        /// </summary>
        public void DefineCompilerDirectiveSymbol(string name)
        {
            if (_currentConfiguration != null)
                _currentConfiguration.DefineConstant(name);
        }

        /// <summary>
        /// Un-define a compiler direcive symbol for the current configuration.
        /// </summary>
        public void UndefineCompilerDirectiveSymbol(string name)
        {
            if (_currentConfiguration != null)
                _currentConfiguration.UndefineConstant(name);
        }

        /// <summary>
        /// Get the full output path of the <see cref="Project"/>.
        /// </summary>
        public string GetFullOutputPath()
        {
            string outputPath = OutputPath;
            if (outputPath != null)
            {
                if (!Path.IsPathRooted(outputPath))
                    outputPath = FileUtil.CombineAndNormalizePath(GetDirectory(), outputPath);
            }
            return outputPath;
        }

        /// <summary>
        /// Add a listed annotation to the <see cref="Project"/>.
        /// </summary>
        public void AnnotationAdded(Annotation annotation, CodeUnit codeUnit, bool sendStatus)
        {
            // Update the list of assembly-level attributes
            if (annotation is Attribute)
            {
                lock (this)
                    _globalAttributes.Add((Attribute)annotation);
            }
            // Pass the annotation to the solution
            else if (_parent != null)
                Solution.AnnotationAdded(annotation, this, codeUnit, sendStatus);
        }

        /// <summary>
        /// Remove a listed annotation from the <see cref="Project"/>.
        /// </summary>
        public void AnnotationRemoved(Annotation annotation)
        {
            // Update the list of assembly-level attributes
            if (annotation is Attribute)
            {
                lock (this)
                    _globalAttributes.Remove((Attribute)annotation);
            }
            // Pass the annotation to the solution
            else if (_parent != null)
                Solution.AnnotationRemoved(annotation);
        }

        protected override void NotifyListedAnnotationAdded(Annotation annotation)
        {
            if (_parent != null)
                Solution.AnnotationAdded(annotation, this, null, true);
        }

        protected override void NotifyListedAnnotationRemoved(Annotation annotation)
        {
            if (_parent != null)
                Solution.AnnotationRemoved(annotation);
        }

        /// <summary>
        /// Log the specified text message with the specified severity level.
        /// </summary>
        public void LogMessage(string message, MessageSeverity severity, string toolTip)
        {
            string prefix = (severity == MessageSeverity.Error ? "ERROR: " : (severity == MessageSeverity.Warning ? "Warning: " : ""));
            Log.WriteLine(prefix + "Project '" + _name + "': " + message, toolTip != null ? toolTip.TrimEnd() : null);
        }

        /// <summary>
        /// Log the specified text message with the specified severity level.
        /// </summary>
        public void LogMessage(string message, MessageSeverity severity)
        {
            LogMessage(message, severity, null);
        }

        /// <summary>
        /// Log the specified exception and message.
        /// </summary>
        public string LogException(Exception ex, string message)
        {
            return Log.Exception(ex, message + " project '" + _name + "'");
        }

        /// <summary>
        /// Log the specified text message and also attach it as an annotation.
        /// </summary>
        public void LogAndAttachMessage(string message, MessageSeverity severity, MessageSource source, string toolTip)
        {
            LogMessage(message, severity, toolTip);
            AttachMessage(message, severity, source);
        }

        /// <summary>
        /// Log the specified text message and also attach it as an annotation.
        /// </summary>
        public void LogAndAttachMessage(string message, MessageSeverity severity, MessageSource source)
        {
            LogMessage(message, severity, null);
            AttachMessage(message, severity, source);
        }

        /// <summary>
        /// Log the specified exception and message and also attach it as an annotation.
        /// </summary>
        public void LogAndAttachException(Exception ex, string message, MessageSource source)
        {
            message = LogException(ex, message);
            AttachMessage(message, MessageSeverity.Error, source);
        }

        /// <summary>
        /// Add the <see cref="CodeObject"/> to the specified dictionary.
        /// </summary>
        public virtual void AddToDictionary(NamedCodeObjectDictionary dictionary)
        {
            dictionary.Add(_name, this);
        }

        /// <summary>
        /// Remove the <see cref="CodeObject"/> from the specified dictionary.
        /// </summary>
        public virtual void RemoveFromDictionary(NamedCodeObjectDictionary dictionary)
        {
            dictionary.Remove(_name, this);
        }

        /// <summary>
        /// Load the specified <see cref="Project"/> file and all child code files directly (without a <see cref="Solution"/>).
        /// </summary>
        /// <param name="fileName">The project (".csproj") file.</param>
        /// <param name="configuration">The project configuration to use (usually 'Debug' or 'Release').</param>
        /// <param name="platform">The project platform to use (such as 'AnyCPU', 'x86', etc.</param>
        /// <param name="loadOptions">Determines various optional processing.</param>
        /// <param name="statusCallback">Status callback for monitoring progress.</param>
        /// <returns>The resulting <see cref="Project"/> object.</returns>
        /// <remarks>
        /// Loading a Project directly goes through the following steps:
        ///   - Parse the Project file, creating a Project object and loading the lists of References and code files (CodeUnits).
        ///   - Parse all code files.
        /// The 'LoadOnly' option stops this process after parsing the Project file, which is useful
        /// if only the project file itself is being viewed, analyzed, or edited.
        /// </remarks>
        public static Project Load(string fileName, string configuration, string platform, LoadOptions loadOptions, Action<LoadStatus, CodeObject> statusCallback)
        {
            Project project = null;
            try
            {
                if (statusCallback != null)
                    statusCallback(LoadStatus.Loading, null);
                Stopwatch overallStopWatch = new Stopwatch();
                overallStopWatch.Start();
                GC.Collect();
                long startBytes = GC.GetTotalMemory(true);

                // Handle a relative path to the file
                if (!Path.IsPathRooted(fileName))
                    fileName = FileUtil.CombineAndNormalizePath(Environment.CurrentDirectory, fileName);

                // Abort if the file doesn't exist - otherwise, the Parse() method will end up returning a valid but empty object
                // with an error message attached (which is done so errors can appear inside a loaded Solution tree).
                if (!File.Exists(fileName))
                {
                    Log.WriteLine("ERROR: Project file '" + fileName + "' does not exist.");
                    return null;
                }

                Log.WriteLine("Loading project '" + Path.GetFileNameWithoutExtension(fileName) + "' ...");

                // Create a dummy solution for the project file, since it is being loaded directly
                Solution solution = new Solution(Path.ChangeExtension(fileName, Solution.SolutionFileExtension), statusCallback);
                if (statusCallback != null)
                    statusCallback(LoadStatus.ObjectCreated, solution);

                // Parse the project and code units
                Unrecognized.Count = 0;
                project = Parse(fileName, solution, statusCallback);
                solution.AddProject(project);

                // Change the default configuration and platform if a configuration was specified, and change the solution
                // configuration to match.
                if (configuration != null)
                    project._currentConfiguration = project.FindConfiguration(configuration, platform);
                solution.ActiveConfiguration = project.ConfigurationName;
                solution.ActivePlatform = (project.ConfigurationPlatform == PlatformAnyCPU ? Solution.PlatformAnyCPU : project.ConfigurationPlatform);
                Log.WriteLine("Active Configuration and Platform: " + project.ConfigurationName + " - " + project.ConfigurationPlatform);

                if (statusCallback != null)
                    statusCallback(LoadStatus.ProjectParsed, project);

                Log.WriteLine("Loaded project '" + project.Name + "', total elapsed time: " + overallStopWatch.Elapsed.TotalSeconds.ToString("N3"));
                if (statusCallback != null)
                    statusCallback(LoadStatus.ProjectsLoaded, null);

                // Parse all code units in the project
                if (loadOptions.HasFlag(LoadOptions.ParseSources))
                    project.ParseCodeUnits(loadOptions, statusCallback);

                long memoryUsage = GC.GetTotalMemory(true) - startBytes;
                Log.WriteLine(string.Format("Total elapsed time: {0:N3}, memory usage: {1} MBs", overallStopWatch.Elapsed.TotalSeconds, memoryUsage / (1024 * 1024)));

                if (statusCallback != null)
                    statusCallback(LoadStatus.LoggingResults, null);
                solution.LogMessageCounts(loadOptions.HasFlag(LoadOptions.LogMessages));
            }
            catch (Exception ex)
            {
                Log.Exception(ex, "loading project");
            }
            return project;
        }

        /// <summary>
        /// Load the specified <see cref="Project"/> file and all child code files directly (without a <see cref="Solution"/>).
        /// </summary>
        /// <param name="fileName">The project (".csproj") file.</param>
        /// <param name="configuration">The project configuration to use (usually 'Debug' or 'Release').</param>
        /// <param name="platform">The project platform to use (such as 'AnyCPU', 'x86', etc.</param>
        /// <param name="loadOptions">Determines various optional processing.</param>
        /// <returns>The resulting <see cref="Project"/> object.</returns>
        /// <remarks>
        /// Loading a Project directly goes through the following steps:
        ///   - Parse the Project file, creating a Project object and loading the lists of References and code files (CodeUnits).
        ///   - Parse all code files.
        /// The 'LoadOnly' option stops this process after parsing the Project file, which is useful
        /// if only the project file itself is being viewed, analyzed, or edited.
        /// </remarks>
        public static Project Load(string fileName, string configuration, string platform, LoadOptions loadOptions)
        {
            return Load(fileName, configuration, platform, loadOptions, null);
        }

        /// <summary>
        /// Load the specified <see cref="Project"/> file and all child code files directly (without a <see cref="Solution"/>).
        /// </summary>
        /// <param name="fileName">The project (".csproj") file.</param>
        /// <param name="configuration">The project configuration to use (usually 'Debug' or 'Release').</param>
        /// <param name="platform">The project platform to use (such as 'AnyCPU', 'x86', etc.</param>
        /// <returns>The resulting <see cref="Project"/> object.</returns>
        /// <remarks>
        /// Loading a Project directly goes through the following steps:
        ///   - Parse the Project file, creating a Project object and loading the lists of References and code files (CodeUnits).
        ///   - Parse all code files.
        /// The 'LoadOnly' option stops this process after parsing the Project file, which is useful
        /// if only the project file itself is being viewed, analyzed, or edited.
        /// </remarks>
        public static Project Load(string fileName, string configuration, string platform)
        {
            return Load(fileName, configuration, platform, LoadOptions.Complete, null);
        }

        /// <summary>
        /// Load the specified <see cref="Project"/> file and all child code files directly (without a <see cref="Solution"/>).
        /// </summary>
        /// <param name="fileName">The project (".csproj") file.</param>
        /// <param name="configuration">The project configuration to use (usually 'Debug' or 'Release').</param>
        /// <returns>The resulting <see cref="Project"/> object.</returns>
        /// <remarks>
        /// Loading a Project directly goes through the following steps:
        ///   - Parse the Project file, creating a Project object and loading the lists of References and code files (CodeUnits).
        ///   - Parse all code files.
        /// The 'LoadOnly' option stops this process after parsing the Project file and resolving References, which is useful
        /// if only the project file itself is being viewed, analyzed, or edited.
        /// </remarks>
        public static Project Load(string fileName, string configuration)
        {
            return Load(fileName, configuration, null, LoadOptions.Complete, null);
        }

        /// <summary>
        /// Load the specified <see cref="Project"/> file and all child code files directly (without a <see cref="Solution"/>).
        /// </summary>
        /// <param name="fileName">The project ('.csproj') file.</param>
        /// <param name="loadOptions">Determines various optional processing.</param>
        /// <param name="statusCallback">Status callback for monitoring progress.</param>
        /// <returns>The resulting <see cref="Project"/> object.</returns>
        /// <remarks>
        /// Loading a Project directly goes through the following steps:
        ///   - Parse the Project file, creating a Project object and loading the lists of References and code files (CodeUnits).
        ///   - Parse all code files.
        /// The 'LoadOnly' option stops this process after parsing the Project file, which is useful
        /// if only the project file itself is being viewed, analyzed, or edited.
        /// </remarks>
        public static Project Load(string fileName, LoadOptions loadOptions, Action<LoadStatus, CodeObject> statusCallback)
        {
            return Load(fileName, null, null, loadOptions, statusCallback);
        }

        /// <summary>
        /// Load the specified <see cref="Project"/> file and all child code files directly (without a <see cref="Solution"/>).
        /// </summary>
        /// <param name="fileName">The project ('.csproj') file.</param>
        /// <param name="loadOptions">Determines various optional processing.</param>
        /// <returns>The resulting <see cref="Project"/> object.</returns>
        /// <remarks>
        /// Loading a Project directly goes through the following steps:
        ///   - Parse the Project file, creating a Project object and loading the lists of References and code files (CodeUnits).
        ///   - Parse all code files.
        /// The 'LoadOnly' option stops this process after parsing the Project file and resolving References, which is useful
        /// if only the project file itself is being viewed, analyzed, or edited.
        /// </remarks>
        public static Project Load(string fileName, LoadOptions loadOptions)
        {
            return Load(fileName, null, null, loadOptions, null);
        }

        /// <summary>
        /// Load the specified <see cref="Project"/> file and all child code files directly (without a <see cref="Solution"/>).
        /// </summary>
        /// <param name="fileName">The project ('.csproj') file.</param>
        /// <returns>The resulting <see cref="Project"/> object.</returns>
        /// <remarks>
        /// Loading a Project directly goes through the following steps:
        ///   - Parse the Project file, creating a Project object and loading the lists of References and code files (CodeUnits).
        ///   - Parse all code files.
        /// The 'LoadOnly' option stops this process after parsing the Project file, which is useful
        /// if only the project file itself is being viewed, analyzed, or edited.
        /// </remarks>
        public static Project Load(string fileName)
        {
            return Load(fileName, null, null, LoadOptions.Complete, null);
        }

        /// <summary>
        /// Parse all <see cref="CodeUnit"/>s in the <see cref="Project"/>.
        /// </summary>
        public void ParseCodeUnits(LoadOptions loadOptions, Action<LoadStatus, CodeObject> statusCallback)
        {
            if (statusCallback != null)
                statusCallback(LoadStatus.Parsing, null);
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            ParseCodeUnits(loadOptions.HasFlag(LoadOptions.DoNotParseBodies) ? ParseFlags.SkipMethodBodies : ParseFlags.None);
            if (Unrecognized.Count > 0)
                Log.WriteLine("UNRECOGNIZED OBJECT COUNT: " + Unrecognized.Count);
            Log.WriteLine("Parsed project '" + Name + "', elapsed time: " + stopWatch.Elapsed.TotalSeconds.ToString("N3"));
        }

        /// <summary>
        /// Parse all <see cref="CodeUnit"/>s in the <see cref="Project"/>.
        /// </summary>
        public void ParseCodeUnits(LoadOptions loadOptions)
        {
            ParseCodeUnits(loadOptions, null);
        }

        /// <summary>
        /// Parse all <see cref="CodeUnit"/>s in the <see cref="Project"/>.
        /// </summary>
        public void ParseCodeUnits()
        {
            ParseCodeUnits(LoadOptions.Complete, null);
        }

        /// <summary>
        /// Unload the project - clear its global namespace.
        /// </summary>
        public void Unload()
        {
            _globalNamespace.RemoveAll();
        }

        /// <summary>
        /// Get the directory of the <see cref="Project"/> (handles website directories).
        /// </summary>
        public string GetDirectory()
        {
            return (IsWebSiteProject ? (_parent != null ? Solution.GetWebSiteDirectory(_name) : null) : Path.GetDirectoryName(_fileName));
        }

        /// <summary>
        /// Save the <see cref="Project"/> to the specified file name.
        /// </summary>
        public void SaveAs(string fileName)
        {
            // Don't try to save web projects
            if (!IsWebSiteProject)
            {
                try
                {
                    Log.DetailWriteLine("Saving project to '" + fileName + "' ...");

                    // VS project files are XML, are normally UTF8, don't use tabs, and use 2-space indents
                    // It's "preferred" to use XmlWriter.Create(), BUT adds a UTF-8 BOM to the file, which Visual Studio apparently
                    // does not normally use, despite the 'utf-8' encoding in the XML header.  Using 'new XmlTextWriter' for now
                    // because it's simpler and avoids the BOM problem.
                    //StreamWriter textWriter = new StreamWriter(fileName, false, FileEncoding);
                    //XmlWriterSettings xmlWriterSettings = new XmlWriterSettings { Indent = true };
                    //using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, xmlWriterSettings))
                    using (XmlTextWriter xmlWriter = new XmlTextWriter(fileName, FileEncoding))
                    {
                        xmlWriter.Formatting = Formatting.Indented;
                        xmlWriter.WriteStartDocument();
                        AsText(xmlWriter);
                        xmlWriter.WriteEndDocument();
                    }
                }
                catch (Exception ex)
                {
                    Log.Exception(ex, "writing");
                }
            }
            _isNew = false;
        }

        /// <summary>
        /// Save the <see cref="Project"/>.
        /// </summary>
        public void Save()
        {
            SaveAs(CodeUnit.GetSaveFileName(_fileName));
        }

        /// <summary>
        /// Save the <see cref="Project"/> plus all <see cref="CodeUnit"/>s.
        /// </summary>
        public void SaveAll()
        {
            Save();
            foreach (CodeUnit codeUnit in CodeUnits)
                codeUnit.Save();
        }

        /// <summary>
        /// Add a new <see cref="Configuration"/> to the <see cref="Project"/>.
        /// </summary>
        public void AddConfiguration(Configuration configuration)
        {
            _configurations.Add(configuration);
            if (Solution != null)
                Solution.AddProjectConfiguration(configuration);
        }

        /// <summary>
        /// Add default configurations to a newly created project (Debug and Release for AnyCPU).
        /// </summary>
        public void AddDefaultConfigurations()
        {
            AddConfiguration(new Configuration(ConfigurationDebug, PlatformAnyCPU, true, DebugTypes.full, false, "DEBUG;TRACE"));
            AddConfiguration(new Configuration(ConfigurationRelease, PlatformAnyCPU, false, DebugTypes.pdbonly, true, "TRACE"));
        }

        /// <summary>
        /// Add default assembly references to a newly created project (System, System.Core, System.Data,
        /// System.Data.DataSetExtensions, System.Xml, System.Xml.Linq, Microsoft.CSharp).
        /// </summary>
        public void AddDefaultAssemblyReferences()
        {
            AddAssemblyReference("System");
            AddAssemblyReference("System.Core");
            AddAssemblyReference("System.Data");
            AddAssemblyReference("System.Data.DataSetExtensions");
            AddAssemblyReference("System.Xml");
            AddAssemblyReference("System.Xml.Linq");
            AddAssemblyReference("Microsoft.CSharp");
        }

        /// <summary>
        /// Get the description of the target framework.
        /// </summary>
        public string GetTargetFrameworkDescription()
        {
            string targetFramework = null;
            if (IsWebSiteProject)
            {
                if (_parent != null)
                {
                    Solution.ProjectEntry projectEntry = Solution.FindProjectEntry(_name);
                    if (projectEntry != null)
                    {
                        Solution.ProjectSection projectSection = projectEntry.FindProjectSection(Solution.WebsitePropertiesProjectSection);
                        if (projectSection != null)
                        {
                            string targetFrameworkMoniker = projectSection.FindValue("TargetFrameworkMoniker");
                            if (targetFrameworkMoniker != null)
                                targetFramework = Uri.UnescapeDataString(targetFrameworkMoniker.Trim('"')).Replace(",Version=", " ");
                        }
                    }
                }
            }
            else
            {
                if (TargetFrameworkIdentifier != null && TargetFrameworkVersion != null)
                {
                    targetFramework = TargetFrameworkIdentifier + " v" + TargetFrameworkVersion
                        + (_targetFrameworkProfile != null ? " " + _targetFrameworkProfile + " Profile" : "");
                }
            }
            return targetFramework;
        }

        /// <summary>
        /// Get the full name of the <see cref="INamedCodeObject"/>, including any namespace name.
        /// </summary>
        public string GetFullName(bool descriptive)
        {
            return _name;
        }

        /// <summary>
        /// Get the full name of the <see cref="INamedCodeObject"/>, including any namespace name.
        /// </summary>
        public string GetFullName()
        {
            return _name;
        }

        #endregion

        #region /* PARSING */

        /// <summary>
        /// Parse a project from a file.
        /// </summary>
        public static Project Parse(string fileName, Solution solution, Action<LoadStatus, CodeObject> statusCallback)
        {
            // Determine the project type GUID
            Guid typeGuid;
            if (fileName.EndsWith(CSharpProjectFileExtension))
                typeGuid = CSProjectType;
            else if (fileName.EndsWith(VBProjectFileExtension))
                typeGuid = VBProjectType;
            else
                typeGuid = new Guid();

            return Parse(Path.GetFileNameWithoutExtension(fileName), fileName, typeGuid, Guid.Empty, solution, statusCallback);
        }

        /// <summary>
        /// Parse a project from a file.
        /// </summary>
        public static Project Parse(string fileName, Solution solution)
        {
            return Parse(fileName, solution, null);
        }

        /// <summary>
        /// Parse a project from a file.
        /// </summary>
        public static Project Parse(string name, string fileName, Guid typeGuid, Guid projectGuid, Solution solution, Action<LoadStatus, CodeObject> statusCallback)
        {
            // Create the project object
            Project project;
            if (typeGuid == CSProjectType)
                project = new Project(name, fileName, typeGuid, projectGuid, solution, true, statusCallback);
            else if (typeGuid == VBProjectType)
            {
                // VB projects aren't fully supported, but we parse the project file in order to get the OutputPath
                // and AssemblyName, which are needed to find and load the output assembly.
                project = new Project(name, fileName, typeGuid, projectGuid, solution, false, statusCallback);
            }
            else
            {
                // We also parse other project file types in order to get the OutputPath and AssemblyName if possible,
                // so we can find and load the output assembly.
                project = new Project(name, fileName, typeGuid, projectGuid, solution, false, statusCallback);
            }

            if (project.NotSupported)
                project.AttachMessage("Project type isn't fully supported (source files won't be parsed)", MessageSeverity.Warning, MessageSource.Parse);
            else
            {
                // Load user settings from any ".csproj.user" file
                string userSettingsFile = Path.ChangeExtension(fileName, ".csproj.user");
                project.ParseUserSettings(userSettingsFile);
            }

            return project;
        }

        /// <summary>
        /// Parse a project from a file.
        /// </summary>
        public static Project Parse(string name, string fileName, Guid typeGuid, Guid projectGuid, Solution solution)
        {
            return Parse(name, fileName, typeGuid, projectGuid, solution, null);
        }

        /// <summary>
        /// Parse a project from a standard VS project file.
        /// </summary>
        protected Project(string name, string fileName, Guid typeGuid, Guid projectGuid, Solution solution, bool isSupported, Action<LoadStatus, CodeObject> statusCallback)
        {
            Log.DetailWriteLine("Loading project '" + name + "' ...");

            // Initialize the project object
            _parent = solution;
            _name = name;
            _fileName = fileName;
            _typeGuid = typeGuid;
            _projectGuid = projectGuid;
            _notSupported = !(isSupported || IsWebSiteProject);
            _configurations = new ChildList<Configuration>(this);
            Initialize();
            if (statusCallback != null)
                statusCallback(LoadStatus.ObjectCreated, this);

            // Special handling for web site projects
            if (IsWebSiteProject)
            {
                ParseWebSiteProject(name, solution, statusCallback);
                return;
            }

            // Check that the file exists (to avoid an exception)
            if (!File.Exists(_fileName))
            {
                LogAndAttachMessage("Project file '" + _fileName + "' doesn't exist!", MessageSeverity.Error, MessageSource.Parse);
                return;
            }

            try
            {
                // Parse the project file
                bool firstElement = true;

                // Open the file and store the encoding and BOM status for use when saving
                FileStream fileStream = new FileStream(_fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                byte[] bom = new byte[3];
                fileStream.Read(bom, 0, 3);
                if (bom[0] == 0xEF && bom[1] == 0xBB && bom[2] == 0xBF)
                    FileHasUTF8BOM = true;
                fileStream.Position = 0;
                StreamReader streamReader = new StreamReader(fileStream);
                streamReader.Peek();  // Peek at the first char so that the encoding is determined
                FileEncoding = streamReader.CurrentEncoding;

                // Parse the file using an XmlReader
                using (XmlReader xmlReader = XmlReader.Create(streamReader))
                {
                    string projectPath = Path.GetDirectoryName(_fileName);
                    string xmlns = null;
                    Locations location = Locations.BeforeProperties;
                    bool lastElementWasItemGroupHeader = false;
                    string unhandledData = null;

                    // Read the next node
                    while (xmlReader.Read())
                    {
                        if (xmlReader.NodeType == XmlNodeType.XmlDeclaration)
                        {
                            // Ignore the declaration node for now
                        }
                        else if (xmlReader.NodeType == XmlNodeType.Element)
                        {
                            if (firstElement)
                            {
                                firstElement = false;
                                if (xmlReader.Name == "VisualStudioProject")
                                {
                                    LogAndAttachMessage("Project files prior to VS 2005 aren't supported - upgrade the project file with VS first.", MessageSeverity.Error, MessageSource.Parse);
                                    return;
                                }
                                if (xmlReader.Name != "Project")
                                {
                                    LogAndAttachMessage("Project file format not recognized or not supported!", MessageSeverity.Error, MessageSource.Parse);
                                    return;
                                }
                                if (xmlReader.MoveToAttribute("ToolsVersion"))
                                    _toolsVersion = xmlReader.Value;
                                if (xmlReader.MoveToAttribute("DefaultTargets"))
                                    _defaultTargets = xmlReader.Value;
                                if (xmlReader.MoveToAttribute("xmlns"))
                                {
                                    _namespace = xmlReader.Value;
                                    xmlns = " xmlns=\"" + _namespace + "\"";
                                }
                            }
                            else if (xmlReader.Name == "PropertyGroup" && !xmlReader.IsEmptyElement)
                            {
                                if (!xmlReader.HasAttributes)
                                    location = Locations.MainProperties;
                                else
                                {
                                    xmlReader.MoveToFirstAttribute();
                                    if (xmlReader.Name == "Condition" && xmlReader.Value.Contains("$(Configuration)"))
                                    {
                                        AddConfiguration(new Configuration(xmlReader, this));
                                        string configurationName;
                                        string platform;
                                        solution.GetProjectConfiguration(solution.ActiveConfiguration, solution.ActivePlatform, this, out configurationName, out platform);
                                        _currentConfiguration = FindConfiguration(configurationName ?? _configurationName, platform ?? _platform);
                                    }
                                    else if (xmlReader.Name == "Label" && xmlReader.Value == "Globals")  // Used by C++ projects (.vcxproj)
                                        location = Locations.MainProperties;
                                    else
                                    {
                                        xmlReader.MoveToElement();
                                        unhandledData = xmlReader.ReadOuterXml();
                                    }
                                }
                            }
                            else if (xmlReader.Name == "ItemGroup" && !xmlReader.IsEmptyElement)
                            {
                                location = Locations.Items;
                                lastElementWasItemGroupHeader = true;
                                continue;
                            }
                            else if (location == Locations.MainProperties)
                            {
                                if (xmlReader.Name == "Configuration" && _configurationName == null)
                                    _configurationName = xmlReader.ReadString().Trim();
                                else if (xmlReader.Name == "Platform" && _platform == null)
                                    _platform = xmlReader.ReadString().Trim();
                                else if (xmlReader.Name == "ProductVersion")
                                    _productVersion = xmlReader.ReadString().Trim();
                                else if (xmlReader.Name == "SchemaVersion")
                                    _schemaVersion = xmlReader.ReadString().Trim();
                                else if (StringUtil.NNEqualsIgnoreCase(xmlReader.Name, "ProjectGuid"))  // C++ uses 'ProjectGUID'
                                    _projectGuid = Guid.Parse(xmlReader.ReadString().Trim());
                                else if (xmlReader.Name == "ProjectTypeGuids")
                                {
                                    string[] guids = xmlReader.ReadString().Trim().Split(';');
                                    if (guids != null)
                                    {
                                        _projectTypeGuids = new List<Guid>();
                                        foreach (string guid in guids)
                                            _projectTypeGuids.Add(Guid.Parse(guid));
                                    }
                                }
                                else if (xmlReader.Name == "OutputType")
                                    _outputType = StringUtil.ParseEnum(xmlReader.ReadString(), OutputTypes.Library);
                                else if (xmlReader.Name == "OutputPath" && _outputPath == null)
                                    _outputPath = xmlReader.ReadString().Trim();
                                else if (xmlReader.Name == "StartupObject")
                                    _startupObject = xmlReader.ReadString().Trim();
                                else if (xmlReader.Name == "NoStandardLibraries")
                                    _noStandardLibraries = StringUtil.ParseBool(xmlReader.ReadString());
                                else if (xmlReader.Name == "AppDesignerFolder")
                                    _appDesignerFolder = StringUtil.EmptyAsNull(xmlReader.ReadString());
                                else if (xmlReader.Name == "RootNamespace")
                                    _rootNamespace = xmlReader.ReadString().Trim();
                                else if (xmlReader.Name == "AssemblyName")
                                    _assemblyName = xmlReader.ReadString().Trim();
                                else if (xmlReader.Name == "DeploymentDirectory")
                                    _deploymentDirectory = StringUtil.EmptyAsNull(xmlReader.ReadString());
                                else if (xmlReader.Name == "StartArguments")
                                    _startArguments = StringUtil.EmptyAsNull(xmlReader.ReadString());
                                else if (xmlReader.Name == "TargetFrameworkIdentifier")
                                    _targetFrameworkIdentifier = StringUtil.EmptyAsNull(xmlReader.ReadString());
                                else if (xmlReader.Name == "TargetFrameworkVersion")
                                    _targetFrameworkVersion = xmlReader.ReadString().Substring(1);  // Skip "v"
                                else if (xmlReader.Name == "TargetFrameworkProfile")
                                    _targetFrameworkProfile = StringUtil.EmptyAsNull(xmlReader.ReadString());
                                else if (xmlReader.Name == "FileAlignment")
                                    _fileAlignment = StringUtil.ParseInt(xmlReader.ReadString());
                                else if (xmlReader.Name == "WarningLevel")
                                    _warningLevel = StringUtil.ParseInt(xmlReader.ReadString());
                                else if (xmlReader.Name == "SignAssembly")
                                    _signAssembly = StringUtil.ParseBool(xmlReader.ReadString());
                                else if (xmlReader.Name == "AssemblyOriginatorKeyFile")
                                    _assemblyOriginatorKeyFile = StringUtil.EmptyAsNull(xmlReader.ReadString());
                                else if (xmlReader.Name == "ReferencePath")
                                    _referencePath = StringUtil.EmptyAsNull(xmlReader.ReadString());
                                else if (xmlReader.Name == "SccProjectName")
                                    _sccProjectName = StringUtil.EmptyAsNull(xmlReader.ReadString());
                                else if (xmlReader.Name == "SccLocalPath")
                                    _sccLocalPath = StringUtil.EmptyAsNull(xmlReader.ReadString());
                                else if (xmlReader.Name == "SccAuxPath")
                                    _sccAuxPath = StringUtil.EmptyAsNull(xmlReader.ReadString());
                                else if (xmlReader.Name == "SccProvider")
                                    _sccProvider = StringUtil.EmptyAsNull(xmlReader.ReadString());
                                else if (xmlReader.Name == "FileUpgradeFlags")
                                    _fileUpgradeFlags = StringUtil.EmptyAsNull(xmlReader.ReadString());
                                else if (xmlReader.Name == "OldToolsVersion")
                                    _oldToolsVersion = StringUtil.EmptyAsNull(xmlReader.ReadString());
                                else if (xmlReader.Name == "UpgradeBackupLocation")
                                    _upgradeBackupLocation = StringUtil.EmptyAsNull(xmlReader.ReadString());
                                else if (xmlReader.Name == "ProjectType")
                                    _projectType = StringUtil.EmptyAsNull(xmlReader.ReadString());
                                else if (xmlReader.Name == "SilverlightVersion")
                                    _silverlightVersion = StringUtil.EmptyAsNull(xmlReader.ReadString());
                                else if (xmlReader.Name == "SilverlightApplication")
                                    _silverlightApplication = StringUtil.ParseBool(xmlReader.ReadString());
                                else if (xmlReader.Name == "SupportedCultures")
                                    _supportedCultures = StringUtil.EmptyAsNull(xmlReader.ReadString());
                                else if (xmlReader.Name == "XapOutputs")
                                    _xapOutputs = StringUtil.ParseBool(xmlReader.ReadString());
                                else if (xmlReader.Name == "GenerateSilverlightManifest")
                                    _generateSilverlightManifest = StringUtil.ParseBool(xmlReader.ReadString());
                                else if (xmlReader.Name == "XapFilename")
                                    _xapFilename = StringUtil.EmptyAsNull(xmlReader.ReadString());
                                else if (xmlReader.Name == "SilverlightManifestTemplate")
                                    _silverlightManifestTemplate = StringUtil.EmptyAsNull(xmlReader.ReadString());
                                else if (xmlReader.Name == "SilverlightAppEntry")
                                    _silverlightAppEntry = StringUtil.EmptyAsNull(xmlReader.ReadString());
                                else if (xmlReader.Name == "TestPageFileName")
                                    _testPageFileName = StringUtil.EmptyAsNull(xmlReader.ReadString());
                                else if (xmlReader.Name == "CreateTestPage")
                                    _createTestPage = StringUtil.ParseBool(xmlReader.ReadString());
                                else if (xmlReader.Name == "ValidateXaml")
                                    _validateXaml = StringUtil.ParseBool(xmlReader.ReadString());
                                else if (xmlReader.Name == "EnableOutOfBrowser")
                                    _enableOutOfBrowser = StringUtil.ParseBool(xmlReader.ReadString());
                                else if (xmlReader.Name == "OutOfBrowserSettingsFile")
                                    _outOfBrowserSettingsFile = StringUtil.EmptyAsNull(xmlReader.ReadString());
                                else if (xmlReader.Name == "UsePlatformExtensions")
                                    _usePlatformExtensions = StringUtil.ParseBool(xmlReader.ReadString());
                                else if (xmlReader.Name == "ThrowErrorsInValidation")
                                    _throwErrorsInValidation = StringUtil.ParseBool(xmlReader.ReadString());
                                else if (xmlReader.Name == "LinkedServerProject")
                                    _linkedServerProject = StringUtil.EmptyAsNull(xmlReader.ReadString());
                                else if (xmlReader.Name == "MvcBuildViews")
                                    _mvcBuildViews = StringUtil.ParseBool(xmlReader.ReadString());
                                else if (xmlReader.Name == "UseIISExpress")
                                    _useIISExpress = StringUtil.ParseBool(xmlReader.ReadString());
                                else if (xmlReader.Name == "SilverlightApplicationList")
                                    _silverlightApplicationList = StringUtil.EmptyAsNull(xmlReader.ReadString());
                                else if (xmlReader.Name == "Nonshipping")
                                    _nonShipping = StringUtil.ParseBool(xmlReader.ReadString());
                                else
                                {
                                    if (_configurations.Count == 0)
                                        unhandledData = xmlReader.ReadOuterXml();
                                    else
                                    {
                                        unhandledData = "<PropertyGroup>" + xmlReader.ReadOuterXml();
                                        while (xmlReader.NodeType != XmlNodeType.EndElement || xmlReader.Name != "PropertyGroup")
                                            unhandledData += xmlReader.ReadOuterXml();
                                        unhandledData += "</PropertyGroup>";
                                        location = (_codeUnits.Count == 0 ? Locations.AfterProperties : Locations.AfterItems);
                                    }
                                }
                            }
                            else if (location == Locations.Items)
                            {
                                if (xmlReader.Name == "Reference")
                                    AddReference(new AssemblyReference(xmlReader, this), statusCallback);
                                else if (xmlReader.Name == "ProjectReference")
                                    AddReference(new ProjectReference(xmlReader, this), statusCallback);
                                else if (xmlReader.Name == "COMReference")
                                    AddReference(new COMReference(xmlReader, this), statusCallback);
                                else
                                {
                                    BuildActions buildAction = StringUtil.ParseEnum(xmlReader.Name, BuildActions.Unrecognized);
                                    if (buildAction != BuildActions.Unrecognized && xmlReader.HasAttributes)
                                    {
                                        xmlReader.MoveToFirstAttribute();
                                        if (xmlReader.Name == "Include")
                                            ParseFileItem(xmlReader, buildAction, lastElementWasItemGroupHeader, projectPath, statusCallback);
                                        else
                                        {
                                            xmlReader.MoveToElement();
                                            unhandledData = xmlReader.ReadOuterXml();
                                        }
                                    }
                                    else
                                        unhandledData = xmlReader.ReadOuterXml();
                                }
                            }
                            else
                                unhandledData = xmlReader.ReadOuterXml();
                            lastElementWasItemGroupHeader = false;
                        }
                        else if (xmlReader.NodeType == XmlNodeType.EndElement)
                        {
                            if (xmlReader.Name == "PropertyGroup")
                                location = (_codeUnits.Count == 0 ? Locations.AfterProperties : Locations.AfterItems);
                            else if (xmlReader.Name == "ItemGroup")
                                location = Locations.AfterItems;
                        }
                        else if (xmlReader.NodeType == XmlNodeType.Comment)
                            _unhandledData.Add(new UnhandledData("  <!--" + xmlReader.Value + "-->", location));
                        else if (xmlReader.NodeType != XmlNodeType.Whitespace && xmlReader.NodeType != XmlNodeType.SignificantWhitespace)
                            unhandledData = xmlReader.ReadOuterXml();
                        if (unhandledData != null)
                        {
                            string rawData = unhandledData.Replace(xmlns, "");
                            if (!string.IsNullOrEmpty(rawData))
                                _unhandledData.Add(new UnhandledData("  " + rawData, location));
                            unhandledData = null;
                        }
                    }
                }

                // Add an implicit reference to System.Core under the proper circumstances
                AddImplicitSystemCoreReferenceIfNecessary();
            }
            catch (Exception ex)
            {
                LogAndAttachException(ex, "parsing", MessageSource.Parse);
            }
        }

        protected void ParseWebSiteProject(string name, Solution solution, Action<LoadStatus, CodeObject> statusCallback)
        {
            try
            {
                string path = solution.GetWebSiteDirectory(name);
                if (path != null)
                {
                    // Parse the local 'Web.Config' file
                    ParseWebConfig(path, false, statusCallback);

                    // Parse the master 'Web.Config' file
                    string runtimeVersion = TargetFrameworkVersion;
                    // For 3.0 or 3.5, use the 2.0 runtime location
                    if (GACUtil.CompareVersions(runtimeVersion, "4.0") < 0)
                        runtimeVersion = "2.0";
                    string runtimePath = Environment.GetEnvironmentVariable("FrameworkDir") ?? Environment.GetEnvironmentVariable("windir") + @"\Microsoft.NET\Framework\";
                    if (runtimeVersion.StartsWith("2"))
                        runtimePath += @"v2.0.50727\";
                    else //if (runtimeVersion.StartsWith("4"))
                        runtimePath += @"v4.0.30319\";
                    runtimePath += @"Config\";
                    ParseWebConfig(runtimePath, true, statusCallback);

                    // Load any project references from the solution file entry
                    List<string> projectReferenceFileNames = new List<string>();
                    Solution.ProjectEntry projectEntry = solution.FindProjectEntry(name);
                    if (projectEntry != null)
                    {
                        Solution.ProjectSection projectSection = projectEntry.FindProjectSection(Solution.WebsitePropertiesProjectSection);
                        if (projectSection != null)
                        {
                            string projectReferencesRaw = projectSection.FindValue("ProjectReferences");
                            if (projectReferencesRaw != null)
                            {
                                string[] projectReferences = projectReferencesRaw.Trim('"').TrimEnd(';').Split(';');
                                foreach (string projectReference in projectReferences)
                                {
                                    // We can't look up project references by GUID here, because there might be forward references.
                                    // So, add the reference using the GUID as the name, and we'll resolve it later.
                                    string[] items = projectReference.Split('|');
                                    AddReference(new ProjectReference(items[0], items[1], Guid.Parse(items[0])), statusCallback);
                                    projectReferenceFileNames.Add(items[1]);
                                }
                            }
                        }
                    }

                    // Load any "BIN" references from the output path
                    string outputPath = Path.Combine(path, OutputPath);
                    if (Directory.Exists(outputPath))
                    {
                        foreach (string assemblyFile in Directory.EnumerateFiles(outputPath, "*.dll"))
                        {
                            if (!projectReferenceFileNames.Contains(Path.GetFileName(assemblyFile)))
                                AddReference(new AssemblyReference(Path.GetFileNameWithoutExtension(assemblyFile)), statusCallback);
                        }
                    }

                    // Load any source files from the physical path
                    foreach (string sourceFile in Directory.EnumerateFiles(path, "*" + CSharpFileExtension, SearchOption.AllDirectories))
                        AddFile(sourceFile, false, statusCallback);
                }
            }
            catch (Exception ex)
            {
                LogAndAttachException(ex, "parsing website", MessageSource.Parse);
            }
        }

        /// <summary>
        /// Parse any 'Web.Config' file for configuration and references.
        /// </summary>
        /// <returns>True if any references were parsed.</returns>
        protected void ParseWebConfig(string path, bool isMasterFile, Action<LoadStatus, CodeObject> statusCallback)
        {
            string webConfigFile = Path.Combine(path, "Web.Config");
            if (File.Exists(webConfigFile))
            {
                LogMessage("Parsing '" + webConfigFile + "'...", MessageSeverity.Information);
                try
                {
                    // Parse the file using an XmlReader
                    FileStream fileStream = new FileStream(webConfigFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                    StreamReader streamReader = new StreamReader(fileStream);
                    using (XmlReader xmlReader = XmlReader.Create(streamReader))
                    {
                        bool inCompilation = false;
                        bool inAssemblies = false;

                        // Read the next node
                        while (xmlReader.Read())
                        {
                            if (xmlReader.NodeType == XmlNodeType.Element)
                            {
                                if (xmlReader.Name == "compilation")
                                {
                                    inCompilation = !xmlReader.IsEmptyElement;
                                    // Don't read this config data from the master file - only a local one
                                    if (!isMasterFile && xmlReader.HasAttributes)
                                    {
                                        if (xmlReader.MoveToAttribute("debug"))
                                        {
                                            _configurationName = (StringUtil.ParseBool(xmlReader.Value) ? ConfigurationDebug : ConfigurationRelease);
                                            AddDefaultConfigurations();
                                            _configurations[0].OutputPath = "Bin";
                                            _configurations[1].OutputPath = "Bin";
                                            string configurationName = null;
                                            string platform;
                                            if (Solution != null)
                                                Solution.GetProjectConfiguration(Solution.ActiveConfiguration, Solution.ActivePlatform, this, out configurationName, out platform);
                                            _platform = PlatformAnyCPU;  // Always AnyCPU for web projects
                                            _currentConfiguration = FindConfiguration(configurationName ?? _configurationName, _platform);
                                        }
                                        if (xmlReader.MoveToAttribute("targetFramework"))
                                            _targetFrameworkVersion = xmlReader.Value;
                                    }
                                }
                                else if (inCompilation && xmlReader.Name == "assemblies" && !xmlReader.IsEmptyElement)
                                    inAssemblies = true;
                                else if (inAssemblies && xmlReader.Name == "add")
                                {
                                    if (xmlReader.HasAttributes)
                                    {
                                        if (xmlReader.MoveToAttribute("assembly"))
                                        {
                                            string assemblyName = xmlReader.Value;
                                            // Just ignore any '*', as we'll always look in the BIN directory anyway
                                            if (assemblyName != "*")
                                                AddReference(new AssemblyReference(assemblyName), statusCallback);
                                        }
                                    }
                                }
                            }
                            else if (xmlReader.NodeType == XmlNodeType.EndElement)
                            {
                                if (xmlReader.Name == "assemblies")
                                    inAssemblies = false;
                                else if (xmlReader.Name == "compilation")
                                    inCompilation = false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogAndAttachException(ex, "parsing 'Web.Config' for ", MessageSource.Parse);
                }
            }
        }

        /// <summary>
        /// Parse file items.
        /// </summary>
        protected void ParseFileItem(XmlReader xmlReader, BuildActions buildAction, bool isFirstInGroup, string projectPath,
            Action<LoadStatus, CodeObject> statusCallback)
        {
            try
            {
                // Parse the file item
                FileItem fileItem = new FileItem(xmlReader, buildAction, projectPath, isFirstInGroup, this);
                _fileItems.Add(fileItem);

                // Special handling for source files
                if (buildAction == BuildActions.Compile)
                {
                    // Add a CodeUnit for the source file
                    AddFile(fileItem.FileName, false, statusCallback);
                }
            }
            catch (Exception ex)
            {
                LogAndAttachException(ex, "parsing file item in ", MessageSource.Parse);
            }
        }

        /// <summary>
        /// Parse all <see cref="CodeUnit"/>s in the <see cref="Project"/>.
        /// </summary>
        public void ParseCodeUnits(ParseFlags flags)
        {
            foreach (CodeUnit codeUnit in _codeUnits)
                codeUnit.Parse(flags);
        }

        /// <summary>
        /// Parse user settings from any ".csproj.user" file.
        /// </summary>
        protected void ParseUserSettings(string fileName)
        {
            if (!File.Exists(fileName))
                return;
            try
            {
                // Open the file and store the encoding and BOM status for use when saving
                FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                byte[] bom = new byte[3];
                fileStream.Read(bom, 0, 3);
                //if (bom[0] == 0xEF && bom[1] == 0xBB && bom[2] == 0xBF)
                //    FileHasUTF8BOM = true;
                fileStream.Position = 0;
                StreamReader streamReader = new StreamReader(fileStream);
                streamReader.Peek();  // Peek at the first char so that the encoding is determined
                //FileEncoding = streamReader.CurrentEncoding;

                // Parse the file using an XmlReader
                using (XmlReader xmlReader = XmlReader.Create(streamReader))
                {
                    Locations location = Locations.BeforeProperties;

                    // Read the next node
                    while (xmlReader.Read())
                    {
                        if (xmlReader.NodeType == XmlNodeType.XmlDeclaration)
                        {
                            // Ignore the declaration node for now
                        }
                        else if (xmlReader.NodeType == XmlNodeType.Element)
                        {
                            if (xmlReader.Name == "PropertyGroup" && !xmlReader.IsEmptyElement)
                            {
                                if (!xmlReader.HasAttributes)
                                    location = Locations.MainProperties;
                                else
                                {
                                    xmlReader.MoveToFirstAttribute();
                                    if (xmlReader.Name == "Condition" && xmlReader.Value.Contains("$(Configuration)"))
                                    {
                                        location = Locations.ConfigurationProperties;
                                        // Setup to read configuration-specific properties here
                                    }
                                    else
                                        xmlReader.MoveToElement();
                                }
                            }
                            else if (location == Locations.MainProperties)
                            {
                                //if (xmlReader.Name == "ProjectView")
                                //    _projectView = xmlReader.ReadString().Trim();
                                if (xmlReader.Name == "ReferencePath")
                                    _userReferencePath = xmlReader.ReadString().Trim();
                            }
                        }
                        else if (xmlReader.NodeType == XmlNodeType.EndElement)
                        {
                            if (xmlReader.Name == "PropertyGroup")
                                location = Locations.AfterProperties;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogException(ex, "parsing user settings for");
            }
        }

        #endregion

        #region /* RENDERING */

        /// <summary>
        /// Always <c>false</c>.
        /// </summary>
        public override bool IsRenderable
        {
            get { return false; }
        }

        public string GetRenderName()
        {
            if (IsWebSiteProject)
            {
                string name = _fileName;
                if (name.StartsWith("http:"))
                {
                    if (!name.EndsWith("/"))
                        name += "/";
                }
                else if (Path.IsPathRooted(name))
                {
                    if (name.EndsWith("\\"))
                        name = name.Substring(0, name.Length - 1);
                    int index = name.LastIndexOf('\\');
                    if (index > 0)
                        name = Path.GetPathRoot(name) + "..." + name.Substring(index);
                    name += "\\";
                }
                return name;
            }
            return _name;
        }

        public override void AsText(CodeWriter writer, RenderFlags flags)
        {
            if (flags.HasFlag(RenderFlags.Description))
                writer.Write(GetRenderName());
            else
                base.AsText(writer, flags);
        }

        public void AsText(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement(null, "Project", _namespace);
            if (_toolsVersion != null)
                xmlWriter.WriteAttributeString("ToolsVersion", _toolsVersion);
            xmlWriter.WriteAttributeString("DefaultTargets", _defaultTargets);

            WriteUnhandledData(xmlWriter, Locations.BeforeProperties);

            xmlWriter.WriteStartElement("PropertyGroup");

            xmlWriter.WriteStartElement("Configuration");
            xmlWriter.WriteAttributeString("Condition", " '$(Configuration)' == '' ");
            xmlWriter.WriteValue(ConfigurationName);
            xmlWriter.WriteEndElement();

            string platform = ConfigurationPlatform;
            if (platform != null)
            {
                xmlWriter.WriteStartElement("Platform");
                xmlWriter.WriteAttributeString("Condition", " '$(Platform)' == '' ");
                xmlWriter.WriteValue(_platform);
                xmlWriter.WriteEndElement();
            }

            if (_productVersion != null)
                xmlWriter.WriteElementString("ProductVersion", _productVersion);
            if (_schemaVersion != null)
                xmlWriter.WriteElementString("SchemaVersion", _schemaVersion);
            xmlWriter.WriteElementString("ProjectGuid", _projectGuid.ToString("B").ToUpper());
            if (_projectTypeGuids != null)
            {
                string guids = "";
                foreach (Guid guid in _projectTypeGuids)
                    guids = StringUtil.Append(guids, ";", guid.ToString("B").ToUpper());
                xmlWriter.WriteElementString("ProjectTypeGuids", guids);
            }
            xmlWriter.WriteElementString("OutputType", _outputType.ToString());
            if (_startupObject != null)
                xmlWriter.WriteElementString("StartupObject", _startupObject);
            if (_noStandardLibraries.HasValue)
                xmlWriter.WriteElementString("NoStandardLibraries", _noStandardLibraries.ToString().ToLower());
            if (_appDesignerFolder != null)
                xmlWriter.WriteElementString("AppDesignerFolder", _appDesignerFolder);
            if (_rootNamespace != null)
                xmlWriter.WriteElementString("RootNamespace", _rootNamespace);
            xmlWriter.WriteElementString("AssemblyName", _assemblyName);
            if (_deploymentDirectory != null)
                xmlWriter.WriteElementString("DeploymentDirectory", _deploymentDirectory);
            if (_startArguments != null)
                xmlWriter.WriteElementString("StartArguments", _startArguments);
            if (_targetFrameworkIdentifier != null)
                xmlWriter.WriteElementString("TargetFrameworkIdentifier", _targetFrameworkIdentifier);
            if (_targetFrameworkVersion != null)
                xmlWriter.WriteElementString("TargetFrameworkVersion", "v" + _targetFrameworkVersion);
            if (_targetFrameworkProfile != null)
                xmlWriter.WriteElementString("TargetFrameworkProfile", _targetFrameworkProfile);
            if (_fileAlignment > 0)
                xmlWriter.WriteElementString("FileAlignment", _fileAlignment.ToString());
            if (_warningLevel.HasValue)
                xmlWriter.WriteElementString("WarningLevel", _warningLevel.GetValueOrDefault().ToString());
            if (_signAssembly.HasValue)
                xmlWriter.WriteElementString("SignAssembly", _signAssembly.ToString().ToLower());
            if (_assemblyOriginatorKeyFile != null)
                xmlWriter.WriteElementString("AssemblyOriginatorKeyFile", _assemblyOriginatorKeyFile);
            if (_referencePath != null)
                xmlWriter.WriteElementString("ReferencePath", _referencePath);
            if (_sccProjectName != null)
                xmlWriter.WriteElementString("SccProjectName", _sccProjectName);
            if (_sccLocalPath != null)
                xmlWriter.WriteElementString("SccLocalPath", _sccLocalPath);
            if (_sccAuxPath != null)
                xmlWriter.WriteElementString("SccAuxPath", _sccAuxPath);
            if (_sccProvider != null)
                xmlWriter.WriteElementString("SccProvider", _sccProvider);
            if (_fileUpgradeFlags != null)
                xmlWriter.WriteElementString("FileUpgradeFlags", _fileUpgradeFlags);
            if (_oldToolsVersion != null)
                xmlWriter.WriteElementString("OldToolsVersion", _oldToolsVersion);
            if (_upgradeBackupLocation != null)
                xmlWriter.WriteElementString("UpgradeBackupLocation", _upgradeBackupLocation);
            if (_projectType != null)
                xmlWriter.WriteElementString("ProjectType", _projectType);
            if (_silverlightVersion != null)
            {
                xmlWriter.WriteElementString("SilverlightVersion", _silverlightVersion);
                xmlWriter.WriteElementString("SilverlightApplication", _silverlightApplication.ToString().ToLower());
            }
            if (_supportedCultures != null)
                xmlWriter.WriteElementString("SupportedCultures", _supportedCultures);
            if (_xapOutputs.HasValue)
                xmlWriter.WriteElementString("XapOutputs", _xapOutputs.ToString().ToLower());
            if (_generateSilverlightManifest.HasValue)
                xmlWriter.WriteElementString("GenerateSilverlightManifest", _generateSilverlightManifest.ToString().ToLower());
            if (_xapFilename != null)
                xmlWriter.WriteElementString("XapFilename", _xapFilename);
            if (_silverlightManifestTemplate != null)
                xmlWriter.WriteElementString("SilverlightManifestTemplate", _silverlightManifestTemplate);
            if (_silverlightAppEntry != null)
                xmlWriter.WriteElementString("SilverlightAppEntry", _silverlightAppEntry);
            if (_testPageFileName != null)
                xmlWriter.WriteElementString("TestPageFileName", _testPageFileName);
            if (_createTestPage.HasValue)
                xmlWriter.WriteElementString("CreateTestPage", _createTestPage.ToString().ToLower());
            if (_validateXaml.HasValue)
                xmlWriter.WriteElementString("ValidateXaml", _validateXaml.ToString().ToLower());
            if (_enableOutOfBrowser.HasValue)
                xmlWriter.WriteElementString("EnableOutOfBrowser", _enableOutOfBrowser.ToString().ToLower());
            if (_outOfBrowserSettingsFile != null)
                xmlWriter.WriteElementString("OutOfBrowserSettingsFile", _outOfBrowserSettingsFile);
            if (_usePlatformExtensions.HasValue)
                xmlWriter.WriteElementString("UsePlatformExtensions", _usePlatformExtensions.ToString().ToLower());
            if (_throwErrorsInValidation.HasValue)
                xmlWriter.WriteElementString("ThrowErrorsInValidation", _throwErrorsInValidation.ToString().ToLower());
            if (_linkedServerProject != null)
                xmlWriter.WriteElementString("LinkedServerProject", _linkedServerProject);
            if (_mvcBuildViews.HasValue)
                xmlWriter.WriteElementString("MvcBuildViews", _mvcBuildViews.ToString().ToLower());
            if (_useIISExpress.HasValue)
                xmlWriter.WriteElementString("UseIISExpress", _useIISExpress.ToString().ToLower());
            if (_silverlightApplicationList != null)
                xmlWriter.WriteElementString("SilverlightApplicationList", _silverlightApplicationList);
            if (_nonShipping.HasValue)
                xmlWriter.WriteElementString("Nonshipping", _nonShipping.ToString().ToLower());

            WriteUnhandledData(xmlWriter, Locations.MainProperties);

            xmlWriter.WriteEndElement();

            // Write project configurations
            foreach (Configuration projectConfiguration in _configurations)
                projectConfiguration.AsText(xmlWriter);

            WriteUnhandledData(xmlWriter, Locations.AfterProperties);

            // Write all references
            if (Enumerable.Any(_references, delegate(Reference reference) { return !reference.IsHidden; }))
            {
                xmlWriter.WriteStartElement("ItemGroup");
                foreach (Reference reference in _references)
                {
                    if (!reference.IsHidden)
                        reference.AsText(xmlWriter);
                }
                xmlWriter.WriteEndElement();
            }

            // Write all file items
            if (_fileItems.Count > 0)
            {
                xmlWriter.WriteStartElement("ItemGroup");
                bool first = true;
                foreach (FileItem fileItem in _fileItems)
                {
                    if (fileItem.IsFirstInGroup && !first)
                    {
                        xmlWriter.WriteEndElement();
                        xmlWriter.WriteStartElement("ItemGroup");
                    }
                    fileItem.AsText(xmlWriter);
                    first = false;
                }
                xmlWriter.WriteEndElement();
            }

            // Write any unhandled file items
            if (Enumerable.Any(_unhandledData, delegate(UnhandledData unhandledData) { return unhandledData.Location == Locations.Items; }))
            {
                xmlWriter.WriteStartElement("ItemGroup");
                WriteUnhandledData(xmlWriter, Locations.Items);
                xmlWriter.WriteEndElement();
            }

            WriteUnhandledData(xmlWriter, Locations.AfterItems);

            xmlWriter.WriteEndElement();
        }

        protected void WriteUnhandledData(XmlWriter xmlWriter, Locations location)
        {
            foreach (UnhandledData unhandledData in _unhandledData)
            {
                if (unhandledData.Location == location)
                    WriteRaw(xmlWriter, unhandledData.Data, _namespace);
            }
        }

        protected void WriteRaw(XmlWriter xmlWriter, string rawData, string @namespace)
        {
            // This is workaround for a nasty bug in the XmlWriter where WriteRaw() turns off formatting, which MS refuses to fix.
            // Another option would be to always retain the unrecognized element (using ReadInnerXml to read the content, instead
            // of ReadOuterXml to read the element plus content), writing it with WriteStart/EndElement and using WriteRaw() only
            // to write the content (because formatting is turned off only until the parent element is ended).
            // See: https://connect.microsoft.com/VisualStudio/feedback/details/677081/xmlwriter-writeraw-permanently-disables-all-formatting
            string wrappedData = "<R xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">" + rawData.Trim('\r', '\n', ' ') + "</R>";
            XmlDocument xmlDocument = new XmlDocument { PreserveWhitespace = true };
            xmlDocument.LoadXml(wrappedData);
            XmlNode wrappedNode = xmlDocument.FirstChild.FirstChild;
            if (wrappedNode != null)
                wrappedNode.WriteTo(xmlWriter);
        }

        #endregion

        #region /* ENUMS */

        /// <summary>
        /// Enumeration of output types.
        /// </summary>
        public enum OutputTypes { WinExe, Exe, Library }

        /// <summary>
        /// Enumeration of debug types.
        /// </summary>
        public enum DebugTypes { none, full, pdbonly }

        /// <summary>
        /// Enumeration of error reporting types.
        /// </summary>
        public enum ErrorReporting { none, prompt, send, queue }

        /// <summary>
        /// Enumeration of types of serialization generation.
        /// </summary>
        public enum GenerateSerializationTypes { Auto, Off, On }

        /// <summary>
        /// Enumeration of build actions.
        /// </summary>
        public enum BuildActions
        {
            Unrecognized,
            None,
            Compile,
            Content,
            EmbeddedResource,
            Resource,
            ApplicationDefinition,
            Page,
            SplashScreen,
            DesignData,
            DesignDataWithDesignTimeCreatableTypes,
            EntityDeploy,
            // Used internally (not in VS UI)
            AppDesigner,
            Folder
        }

        /// <summary>
        /// Enumeration of copy actions.
        /// </summary>
        public enum CopyActions { None, Always, PreserveNewest }

        #endregion

        #region /* PROJECT CONFIGURATION */

        /// <summary>
        /// Configuration settings for a <see cref="Project"/>.
        /// </summary>
        public class Configuration : CodeObject
        {
            #region /* FIELDS */

            public string Name;
            public string Platform;

            public bool DebugSymbols;
            public DebugTypes DebugType;  // Values: none, full, pdbonly
            public bool Optimize;
            public string OutputPath;
            public bool? EnableUnmanagedDebugging;
            public string _defineConstants;
            public bool NoStdLib;
            public bool NoConfig;
            public ErrorReporting ErrorReport;  // Values: none, prompt, send, queue
            public int WarningLevel = 4;
            public bool TreatWarningsAsErrors;
            public bool? IncrementalBuild;
            public bool AllowUnsafeBlocks;
            public string DocumentationFile;
            public string PlatformTarget;
            public string NoWarn;
            public string WarningsAsErrors;
            public bool RunCodeAnalysis;
            public string CodeAnalysisRules;
            public string CodeAnalysisRuleSet;
            public bool RegisterForComInterop;
            public GenerateSerializationTypes GenerateSerializationAssemblies;  // Values: Auto, Off, On
            public string LangVersion;                                          // Values: default, ISO-1, ISO-2, 3
            public bool CheckForOverflowUnderflow;
            public int FileAlignment = DefaultFileAlignment;  // Values: 512, 1024, 2048, 4096, 8192
            public int BaseAddress = DefaultBaseAddress;
            public bool UseVSHostingProcess = true;
            public string ConfigurationOverrideFile;  // OLD?
            public bool RemoveIntegerChecks;          // VB only?  Remove if false.

            /// <summary>
            /// Unhandled (unparsed or unrecognized) XML data in the project configuration.
            /// </summary>
            protected List<UnhandledData> _unhandledData = new List<UnhandledData>();

            /// <summary>
            /// A set of all defined constants for quick lookups.
            /// </summary>
            private HashSet<string> _constants = new HashSet<string>();

            #endregion

            #region /* CONSTRUCTORS */

            /// <summary>
            /// Create a <see cref="Configuration"/>.
            /// </summary>
            public Configuration(string name, string platform, bool debugSymbols, DebugTypes debugType, bool optimize, string defineConstants)
            {
                Name = name;
                Platform = platform;
                DebugSymbols = debugSymbols;
                DebugType = debugType;
                Optimize = optimize;
                OutputPath = @"bin\" + name + @"\";
                DefineConstants = defineConstants;
            }

            #endregion

            #region /* PROPERTIES */

            /// <summary>
            /// The parent <see cref="Project"/>.
            /// </summary>
            public Project ParentProject
            {
                get { return _parent as Project; }
            }

            /// <summary>
            /// The defined constants for this configuration as a delimited string.
            /// </summary>
            public string DefineConstants
            {
                get { return _defineConstants; }
                set
                {
                    _defineConstants = value;
                    _constants.Clear();
                    if (!string.IsNullOrEmpty(value))
                    {
                        foreach (string constant in value.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries))
                            _constants.Add(constant);
                    }
                }
            }

            /// <summary>
            /// The defined constants for this configuration as a set of strings.
            /// </summary>
            public HashSet<string> Constants
            {
                get { return _constants; }
            }

            #endregion

            #region /* METHODS */

            /// <summary>
            /// Determine if the specified constant is defined.
            /// </summary>
            public bool IsConstantDefined(string name)
            {
                return (_constants.Contains(name) || name == "USING_NOVA" || name == "USING_NOVA_2");
            }

            /// <summary>
            /// Define a constant.
            /// </summary>
            public void DefineConstant(string name)
            {
                _constants.Add(name);
                _defineConstants = StringUtil.ToString(_constants, ";");
            }

            /// <summary>
            /// Un-define a constant.
            /// </summary>
            public void UndefineConstant(string name)
            {
                _constants.Remove(name);
                _defineConstants = StringUtil.ToString(_constants, ";");
            }

            #endregion

            #region /* PARSING */

            /// <summary>
            /// Parse from the specified <see cref="XmlReader"/>.
            /// </summary>
            public Configuration(XmlReader xmlReader, Project parent)
            {
                Parent = parent;
                string xmlns = " xmlns=\"" + parent._namespace + "\"";

                // Parse the configuration name and Platform from the Condition attribute
                string condition = xmlReader.Value;
                int start = condition.IndexOf("==");
                if (start > 0)
                {
                    start = condition.IndexOf('\'', start + 2);
                    if (start > 0)
                    {
                        ++start;
                        int end = condition.IndexOf('\'', start);
                        if (end > 0)
                        {
                            string value = condition.Substring(start, end - start);
                            string[] values = value.Split('|');
                            Name = values[0];
                            if (values.Length > 1)
                                Platform = values[1];
                        }
                    }
                }

                // Parse all child tags
                while (!(xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == "PropertyGroup") && xmlReader.Read())
                {
                    if (xmlReader.NodeType == XmlNodeType.Element)
                    {
                        if (xmlReader.Name == "DebugSymbols")
                            DebugSymbols = StringUtil.ParseBool(xmlReader.ReadString());
                        else if (xmlReader.Name == "DebugType")
                            DebugType = StringUtil.ParseEnum(xmlReader.ReadString(), DebugTypes.none);
                        else if (xmlReader.Name == "Optimize")
                            Optimize = StringUtil.ParseBool(xmlReader.ReadString());
                        else if (xmlReader.Name == "OutputPath")
                            OutputPath = xmlReader.ReadString().Trim();
                        else if (xmlReader.Name == "EnableUnmanagedDebugging")
                            EnableUnmanagedDebugging = StringUtil.ParseBool(xmlReader.ReadString());
                        else if (xmlReader.Name == "DefineConstants")
                            DefineConstants = StringUtil.EmptyAsNull(xmlReader.ReadString());
                        else if (xmlReader.Name == "NoStdLib")
                            NoStdLib = StringUtil.ParseBool(xmlReader.ReadString());
                        else if (xmlReader.Name == "NoConfig")
                            NoConfig = StringUtil.ParseBool(xmlReader.ReadString());
                        else if (xmlReader.Name == "ErrorReport")
                            ErrorReport = StringUtil.ParseEnum(xmlReader.ReadString(), ErrorReporting.prompt);
                        else if (xmlReader.Name == "WarningLevel")
                            WarningLevel = StringUtil.ParseInt(xmlReader.ReadString());
                        else if (xmlReader.Name == "TreatWarningsAsErrors")
                            TreatWarningsAsErrors = StringUtil.ParseBool(xmlReader.ReadString());
                        else if (xmlReader.Name == "IncrementalBuild")
                            IncrementalBuild = StringUtil.ParseBool(xmlReader.ReadString());
                        else if (xmlReader.Name == "AllowUnsafeBlocks")
                            AllowUnsafeBlocks = StringUtil.ParseBool(xmlReader.ReadString());
                        else if (xmlReader.Name == "AllowUnsafeBlocks")
                            AllowUnsafeBlocks = StringUtil.ParseBool(xmlReader.ReadString());
                        else if (xmlReader.Name == "DocumentationFile")
                            DocumentationFile = StringUtil.EmptyAsNull(xmlReader.ReadString());
                        else if (xmlReader.Name == "PlatformTarget")
                            PlatformTarget = StringUtil.EmptyAsNull(xmlReader.ReadString());
                        else if (xmlReader.Name == "RunCodeAnalysis")
                            RunCodeAnalysis = StringUtil.ParseBool(xmlReader.ReadString());
                        else if (xmlReader.Name == "CodeAnalysisRules")
                            CodeAnalysisRules = StringUtil.EmptyAsNull(xmlReader.ReadString());
                        else if (xmlReader.Name == "CodeAnalysisRuleSet")
                            CodeAnalysisRuleSet = StringUtil.EmptyAsNull(xmlReader.ReadString());
                        else if (xmlReader.Name == "NoWarn")
                            NoWarn = StringUtil.EmptyAsNull(xmlReader.ReadString());
                        else if (xmlReader.Name == "WarningsAsErrors")
                            WarningsAsErrors = StringUtil.EmptyAsNull(xmlReader.ReadString());
                        else if (xmlReader.Name == "RegisterForComInterop")
                            RegisterForComInterop = StringUtil.ParseBool(xmlReader.ReadString());
                        else if (xmlReader.Name == "GenerateSerializationAssemblies")
                            GenerateSerializationAssemblies = StringUtil.ParseEnum(xmlReader.ReadString(), GenerateSerializationTypes.Auto);
                        else if (xmlReader.Name == "LangVersion")
                            LangVersion = StringUtil.EmptyAsNull(xmlReader.ReadString());
                        else if (xmlReader.Name == "CheckForOverflowUnderflow")
                            CheckForOverflowUnderflow = StringUtil.ParseBool(xmlReader.ReadString());
                        else if (xmlReader.Name == "FileAlignment")
                            FileAlignment = StringUtil.ParseInt(xmlReader.ReadString());
                        else if (xmlReader.Name == "BaseAddress")
                            BaseAddress = StringUtil.ParseInt(xmlReader.ReadString());
                        else if (xmlReader.Name == "UseVSHostingProcess")
                            UseVSHostingProcess = StringUtil.ParseBool(xmlReader.ReadString());
                        else if (xmlReader.Name == "ConfigurationOverrideFile")
                            ConfigurationOverrideFile = StringUtil.EmptyAsNull(xmlReader.ReadString());
                        else if (xmlReader.Name == "RemoveIntegerChecks")
                            RemoveIntegerChecks = StringUtil.ParseBool(xmlReader.ReadString());
                        else
                        {
                            string unhandledData = xmlReader.ReadOuterXml().Replace(xmlns, "");
                            if (!string.IsNullOrEmpty(unhandledData))
                                _unhandledData.Add(new UnhandledData("    " + unhandledData, Locations.ConfigurationProperties));
                        }
                    }
                }
            }

            #endregion

            #region /* RENDERING */

            /// <summary>
            /// Save to the specified <see cref="XmlWriter"/>.
            /// </summary>
            public void AsText(XmlWriter xmlWriter)
            {
                Project parentProject = ParentProject;

                xmlWriter.WriteStartElement("PropertyGroup");
                string configuration = (Platform != null ? " '$(Configuration)|$(Platform)' == '" + Name + "|" + Platform + "' "
                    : " '$(Configuration)' == '" + Name + "' ");
                xmlWriter.WriteAttributeString("Condition", configuration);

                if (DebugSymbols)
                    xmlWriter.WriteElementString("DebugSymbols", "true");
                if (DebugType != DebugTypes.none)
                    xmlWriter.WriteElementString("DebugType", DebugType.ToString());
                xmlWriter.WriteElementString("Optimize", Optimize.ToString().ToLower());
                if (OutputPath != null)
                    xmlWriter.WriteElementString("OutputPath", OutputPath);
                if (EnableUnmanagedDebugging.HasValue)
                    xmlWriter.WriteElementString("EnableUnmanagedDebugging", EnableUnmanagedDebugging.ToString().ToLower());
                if (DefineConstants != null)
                    xmlWriter.WriteElementString("DefineConstants", DefineConstants);
                if (NoStdLib)
                    xmlWriter.WriteElementString("NoStdLib", NoStdLib.ToString().ToLower());
                if (NoConfig)
                    xmlWriter.WriteElementString("NoConfig", NoConfig.ToString().ToLower());
                if (ErrorReport != ErrorReporting.none)
                    xmlWriter.WriteElementString("ErrorReport", ErrorReport.ToString());
                xmlWriter.WriteElementString("WarningLevel", WarningLevel.ToString());
                if (TreatWarningsAsErrors)
                    xmlWriter.WriteElementString("TreatWarningsAsErrors", TreatWarningsAsErrors.ToString().ToLower());
                if (IncrementalBuild.HasValue)
                    xmlWriter.WriteElementString("IncrementalBuild", IncrementalBuild.ToString().ToLower());
                if (AllowUnsafeBlocks)
                    xmlWriter.WriteElementString("AllowUnsafeBlocks", AllowUnsafeBlocks.ToString().ToLower());
                if (DocumentationFile != null)
                    xmlWriter.WriteElementString("DocumentationFile", (parentProject != null ? FileUtil.RemoveCommonRootPath(DocumentationFile, parentProject.FileName) : DocumentationFile));
                if (PlatformTarget != null)
                    xmlWriter.WriteElementString("PlatformTarget", PlatformTarget);
                if (RunCodeAnalysis)
                    xmlWriter.WriteElementString("RunCodeAnalysis", RunCodeAnalysis.ToString().ToLower());
                if (CodeAnalysisRules != null)
                    xmlWriter.WriteElementString("CodeAnalysisRules", CodeAnalysisRules);
                if (CodeAnalysisRuleSet != null)
                    xmlWriter.WriteElementString("CodeAnalysisRuleSet", CodeAnalysisRuleSet);
                if (NoWarn != null)
                    xmlWriter.WriteElementString("NoWarn", NoWarn);
                if (WarningsAsErrors != null)
                    xmlWriter.WriteElementString("WarningsAsErrors", WarningsAsErrors);
                if (RegisterForComInterop)
                    xmlWriter.WriteElementString("RegisterForComInterop", RegisterForComInterop.ToString().ToLower());
                if (GenerateSerializationAssemblies != GenerateSerializationTypes.Auto)
                    xmlWriter.WriteElementString("GenerateSerializationAssemblies", GenerateSerializationAssemblies.ToString());
                if (LangVersion != null && LangVersion != "default")
                    xmlWriter.WriteElementString("LangVersion", LangVersion);
                if (CheckForOverflowUnderflow)
                    xmlWriter.WriteElementString("CheckForOverflowUnderflow", CheckForOverflowUnderflow.ToString().ToLower());
                if (FileAlignment != DefaultFileAlignment)
                    xmlWriter.WriteElementString("FileAlignment", FileAlignment.ToString());
                if (BaseAddress != DefaultBaseAddress)
                    xmlWriter.WriteElementString("BaseAddress", BaseAddress.ToString());
                if (!UseVSHostingProcess)
                    xmlWriter.WriteElementString("UseVSHostingProcess", UseVSHostingProcess.ToString().ToLower());
                if (ConfigurationOverrideFile != null)
                    xmlWriter.WriteElementString("ConfigurationOverrideFile", ConfigurationOverrideFile);
                if (RemoveIntegerChecks)
                    xmlWriter.WriteElementString("RemoveIntegerChecks", RemoveIntegerChecks.ToString().ToLower());

                if (parentProject != null)
                {
                    foreach (UnhandledData unhandledData in _unhandledData)
                        parentProject.WriteRaw(xmlWriter, unhandledData.Data, parentProject._namespace);
                }

                xmlWriter.WriteEndElement();
            }

            #endregion
        }

        #endregion

        #region /* FILE ITEM */

        /// <summary>
        /// Represents a file item in a <see cref="Project"/> file.
        /// </summary>
        public class FileItem : CodeObject
        {
            #region /* FIELDS */

            /// <summary>
            /// The build action for the file.
            /// Values: None, Compile, Content, EmbeddedResource, Resource, ApplicationDefinition, Page, SplashScreen, DesignData, DesignDataWithDesignTimeCreatableTypes, EntityDeploy
            /// </summary>
            public BuildActions BuildAction;

            /// <summary>
            /// The file name.
            /// </summary>
            public string FileName;

            /// <summary>
            /// The link for the file (if any).
            /// </summary>
            public string Link;

            /// <summary>
            /// The copy action for the file.  Values: None, Always, PreserveNewest
            /// </summary>
            public CopyActions CopyToOutputDirectory;

            public bool AutoGen;
            public bool DesignTime;
            public bool DesignTimeSharedInput;
            public string DependentUpon;
            public string Generator;
            public string LastGenOutput;
            public string SubType;
            public string CustomToolNamespace;

            /// <summary>
            /// True if the file appears first in the current group in the <see cref="Project"/> file.
            /// </summary>
            public bool IsFirstInGroup;

            /// <summary>
            /// Unhandled (unparsed or unrecognized) XML data in the file item.
            /// </summary>
            protected List<UnhandledData> _unhandledData = new List<UnhandledData>();

            #endregion

            #region /* CONSTRUCTORS */

            /// <summary>
            /// Create a <see cref="FileItem"/>.
            /// </summary>
            public FileItem(BuildActions buildAction, string fileName)
            {
                BuildAction = buildAction;
                FileName = fileName;
            }

            #endregion

            #region /* PROPERTIES */

            /// <summary>
            /// The parent <see cref="Project"/>.
            /// </summary>
            public Project ParentProject
            {
                get { return _parent as Project; }
            }

            #endregion

            #region /* PARSING */

            /// <summary>
            /// Parse from the specified <see cref="XmlReader"/>.
            /// </summary>
            public FileItem(XmlReader xmlReader, BuildActions buildAction, string projectPath, bool isFirstInGroup, Project parent)
            {
                Parent = parent;
                IsFirstInGroup = isFirstInGroup;
                BuildAction = buildAction;
                FileName = xmlReader.Value;
                if (!Path.IsPathRooted(FileName))
                    FileName = projectPath + @"\" + FileName;
                xmlReader.MoveToContent();

                // Parse any child elements
                if (!xmlReader.IsEmptyElement)
                {
                    string xmlns = " xmlns=\"" + parent._namespace + "\"";
                    while (!(xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == buildAction.ToString()) && xmlReader.Read())
                    {
                        if (xmlReader.NodeType == XmlNodeType.Element)
                        {
                            if (xmlReader.Name == "Link")
                                Link = StringUtil.EmptyAsNull(xmlReader.ReadString());
                            else if (xmlReader.Name == "CopyToOutputDirectory")
                                CopyToOutputDirectory = StringUtil.ParseEnum(xmlReader.ReadString(), CopyActions.None);
                            else if (xmlReader.Name == "AutoGen")
                                AutoGen = StringUtil.ParseBool(xmlReader.ReadString());
                            else if (xmlReader.Name == "DesignTime")
                                DesignTime = StringUtil.ParseBool(xmlReader.ReadString());
                            else if (xmlReader.Name == "DesignTimeSharedInput")
                                DesignTimeSharedInput = StringUtil.ParseBool(xmlReader.ReadString());
                            else if (xmlReader.Name == "DependentUpon")
                                DependentUpon = StringUtil.EmptyAsNull(xmlReader.ReadString());
                            else if (xmlReader.Name == "Generator")
                            {
                                if (Generator == null)  // Only take the first one in case of dups
                                    Generator = StringUtil.EmptyAsNull(xmlReader.ReadString());
                            }
                            else if (xmlReader.Name == "LastGenOutput")
                                LastGenOutput = StringUtil.EmptyAsNull(xmlReader.ReadString());
                            else if (xmlReader.Name == "SubType")
                            {
                                if (SubType == null)  // Only take the first one in case of dups)
                                    SubType = StringUtil.EmptyAsNull(xmlReader.ReadString());
                            }
                            else if (xmlReader.Name == "CustomToolNamespace")
                                CustomToolNamespace = StringUtil.EmptyAsNull(xmlReader.ReadString());
                            else
                            {
                                string unhandledData = xmlReader.ReadOuterXml().Replace(xmlns, "");
                                if (!string.IsNullOrEmpty(unhandledData))
                                    _unhandledData.Add(new UnhandledData("    " + unhandledData, Locations.ConfigurationProperties));
                            }
                        }
                    }
                }
            }

            #endregion

            #region /* RENDERING */

            /// <summary>
            /// Save to the specified <see cref="XmlWriter"/>.
            /// </summary>
            public void AsText(XmlWriter xmlWriter)
            {
                Project parentProject = ParentProject;

                xmlWriter.WriteStartElement(BuildAction.ToString());
                xmlWriter.WriteAttributeString("Include", (parentProject != null ? FileUtil.RemoveCommonRootPath(FileName, parentProject.FileName) : FileName));

                if (Link != null)
                    xmlWriter.WriteElementString("Link", Link);
                if (CopyToOutputDirectory != CopyActions.None)
                    xmlWriter.WriteElementString("CopyToOutputDirectory", CopyToOutputDirectory.ToString());
                if (AutoGen)
                    xmlWriter.WriteElementString("AutoGen", AutoGen.ToString());
                if (DesignTime)
                    xmlWriter.WriteElementString("DesignTime", DesignTime.ToString());
                if (DesignTimeSharedInput)
                    xmlWriter.WriteElementString("DesignTimeSharedInput", DesignTimeSharedInput.ToString());
                if (DependentUpon != null)
                    xmlWriter.WriteElementString("DependentUpon", DependentUpon);
                if (Generator != null)
                    xmlWriter.WriteElementString("Generator", Generator);
                if (LastGenOutput != null)
                    xmlWriter.WriteElementString("LastGenOutput", LastGenOutput);
                if (SubType != null)
                    xmlWriter.WriteElementString("SubType", SubType);
                if (CustomToolNamespace != null)
                    xmlWriter.WriteElementString("CustomToolNamespace", CustomToolNamespace);

                if (parentProject != null)
                {
                    foreach (UnhandledData unhandledData in _unhandledData)
                        parentProject.WriteRaw(xmlWriter, unhandledData.Data, parentProject._namespace);
                }

                xmlWriter.WriteEndElement();
            }

            #endregion
        }

        #endregion

        #region /* UNHANDLED DATA */

        /// <summary>
        /// Represents unhandled data in a <see cref="Project"/> file.
        /// </summary>
        public class UnhandledData
        {
            /// <summary>
            /// The unhandled data.
            /// </summary>
            public string Data;

            /// <summary>
            /// The location of the unhandled data.
            /// </summary>
            public Locations Location;

            /// <summary>
            /// Create an <see cref="UnhandledData"/> object.
            /// </summary>
            public UnhandledData(string data, Locations location)
            {
                Data = data;
                Location = location;
            }
        }

        public enum Locations { BeforeProperties, MainProperties, ConfigurationProperties, AfterProperties, Items, AfterItems }

        #endregion
    }
}
