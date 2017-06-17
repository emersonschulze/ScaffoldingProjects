// The Nova Project by Ken Beckett.
// Copyright (C) 2007-2012 Inevitable Software, all rights reserved.
// Released under the Common Development and Distribution License, CDDL-1.0: http://opensource.org/licenses/cddl1.php

using Microsoft.Win32;

namespace Nova.Utilities
{
    /// <summary>
    /// Utility methods for accessing the Windows registry.
    /// </summary>
    public static class RegistryUtil
    {
        /// <summary>
        /// Retrieve a string value from the Local Machine registry.
        /// </summary>
        public static string GetLocalMachineString(string keyPath, string keyName)
        {
            string val = null;
            RegistryKey key = null;
            try
            {
                key = Registry.LocalMachine.OpenSubKey(keyPath);
                if (key != null)
                {
                    val = (string)key.GetValue(keyName);
                    key.Close();
                }
            }
            finally
            {
                if (key != null)
                    key.Close();
            }
            return val;
        }

        /// <summary>
        /// Retrieve a string value from the Classes registry.
        /// </summary>
        public static string GetClassesString(string keyPath, string keyName)
        {
            string val = null;
            RegistryKey key = null;
            try
            {
                key = Registry.ClassesRoot.OpenSubKey(keyPath);
                if (key != null)
                {
                    val = (string)key.GetValue(keyName);
                    key.Close();
                }
            }
            finally
            {
                if (key != null)
                    key.Close();
            }
            return val;
        }
    }
}
