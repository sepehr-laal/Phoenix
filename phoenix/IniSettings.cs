﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace phoenix
{
    /// <summary>
    /// A class that can read and write to INI files using Kernel32 API
    /// It can auto save the read entries automatically to an INI file.
    /// </summary>
    class IniSettings
    {
        private Dictionary<string, Dictionary<string, string>>
                        m_Settings  = new Dictionary<string, Dictionary<string, string>>();
        private string  m_Path      = string.Empty;

        /// <summary>
        /// Constructor. Creates an empty file if "path" does not exist.
        /// </summary>
        /// <param name="path"></param>
        public IniSettings(string path)
        {
            m_Path = path;

            // Convert relative to absolute path. Kernel32 API
            // does not like relative paths apparently.
            if (!Path.IsPathRooted(m_Path))
                m_Path = Path.GetFullPath(m_Path);

            // Create an empty INI file if it does not exist.
            if (!File.Exists(m_Path))
                File.Create(m_Path).Dispose();

            Logger.Info(string.Format("path to settings files is: {0}", m_Path));
        }

        ~IniSettings()
        {
            // Save read values on destruction
            SaveReadEntries();
        }

        /// <summary>
        /// Writes a string entry to INI file, you'd hardly ever need this
        /// </summary>
        /// <param name="Section"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        public void Write(string Section, string Key, string Value)
        {
            if (!NativeMethods.WritePrivateProfileString(Section, Key, Value, m_Path))
            {
                Logger.Error(string.Format(
                    "Failed to write INI entry in section {0} with key {1} and value {2}",
                    Section,
                    Key,
                    Value));
            }
        }

        /// <summary>
        /// Store a value to be saved later via SaveReadEntries()
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Section"></param>
        /// <param name="Key"></param>
        /// <param name="value"></param>
        public void Store<T>(string Section, string Key, T value)
        {
            if (!m_Settings.ContainsKey(Section))
                m_Settings[Section] = new Dictionary<string, string>();

            string to_be_stored = value.ToString();

            // http://www.askingbox.com/question/windows-api-getprofilestring-and-getprivateprofilestring-clears-quotes
            if (to_be_stored.StartsWith("\"") && to_be_stored.EndsWith("\""))
                to_be_stored = string.Format("\"{0}\"", to_be_stored);

            m_Settings[Section][Key] = to_be_stored.Replace("\n", "<br>");
        }

        /// <summary>
        /// Reads an entry in INI file, returns DefaultValue if not found
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Section"></param>
        /// <param name="Key"></param>
        /// <param name="DefaultValue"></param>
        /// <returns></returns>
        public T Read<T>(string Section, string Key, T DefaultValue)
        {
            StringBuilder strb = new StringBuilder(2048);

            if (NativeMethods.GetPrivateProfileString(Section, Key, "", strb, strb.Capacity, m_Path) > 0
                && CanChangeType(strb.ToString(), typeof(T)))
            {
                try
                {
                    T temp_val = (T)Convert.ChangeType(strb.ToString(), typeof(T));
                    Store(Section, Key, temp_val);
                    return temp_val;
                }
                catch
                {
                    Logger.Warn(string.Format(
                        "Failed to change INI entry type in section {0} and key {1}. Default value: {1}",
                        Section,
                        Key,
                        DefaultValue));
                }
            }
            else
            {
                Logger.Warn(string.Format(
                    "Failed to read INI entry in section {0} and key {1} or entry is empty.",
                    Section,
                    Key));
            }

            Store(Section, Key, DefaultValue);
            return DefaultValue;
        }

        /// <summary>
        /// Save everything read through Read() API so far back to INI file
        /// </summary>
        public void SaveReadEntries()
        {
            Logger.Info("Saving INI entries back to settings file.");

            foreach (var keys in m_Settings)
            {
                var section = keys.Key;
                foreach (var entries in m_Settings[section])
                {
                    Write(section, entries.Key, entries.Value);
                }
            }
        }

        private static bool CanChangeType(object value, Type conversionType)
        {
            if (conversionType == null || value == null || (value as IConvertible) == null)
            {
                return false;
            }

            return true;
        }
    }
}
