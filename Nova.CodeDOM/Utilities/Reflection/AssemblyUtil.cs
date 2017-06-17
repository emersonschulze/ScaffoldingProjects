﻿// The Nova Project by Ken Beckett.
// Copyright (C) 2007-2012 Inevitable Software, all rights reserved.
// Released under the Common Development and Distribution License, CDDL-1.0: http://opensource.org/licenses/cddl1.php

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nova.Utilities
{
    /// <summary>
    /// Static helper methods for <see cref="Assembly"/>.
    /// </summary>
    public static class AssemblyUtil
    {
        #region /* CONSTANTS */

        /// <summary>
        /// The name of the version part of a display name.
        /// </summary>
        public const string VersionTag = "Version=";

        #endregion

        #region /* STATIC HELPER METHODS */

        /// <summary>
        /// Determine if the specified assembly name is a display name.
        /// </summary>
        public static bool IsDisplayName(string assemblyName)
        {
            return assemblyName.Contains(",");
        }

        /// <summary>
        /// Get a normalized version of the specified assembly display name, including the name, Version (if any),
        /// Culture (if any), PublicKeyToken (null if none), and processorArchitecture (if not the default MSIL).
        /// </summary>
        public static string GetNormalizedDisplayName(string assemblyName)
        {
            AssemblyName name = new AssemblyName(assemblyName);
            string result = name.FullName;
            // Remove any PublicKeyToken tag if the value is null (assembly references might omit it)
            const string nullKey = ", PublicKeyToken=null";
            int index = result.IndexOf(nullKey);
            return (index > 0 ? result.Remove(index, nullKey.Length) : result);
        }

        /// <summary>
        /// Determine if the specified assembly name has a version number.
        /// </summary>
        public static bool HasVersion(string assemblyName)
        {
            return assemblyName.Contains(VersionTag);
        }

        /// <summary>
        /// Get the version number from an assembly display name.
        /// </summary>
        public static string GetVersion(string assemblyName)
        {
            int versionIndex = assemblyName.IndexOf(VersionTag);
            if (versionIndex > 0)
            {
                versionIndex += VersionTag.Length;
                int commaIndex = assemblyName.IndexOf(",", versionIndex);
                if (commaIndex < versionIndex)
                    commaIndex = assemblyName.Length;
                return assemblyName.Substring(versionIndex, commaIndex - versionIndex);
            }
            return null;
        }

        #endregion
    }
}
