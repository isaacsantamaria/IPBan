﻿/*
MIT License

Copyright (c) 2019 Digital Ruby, LLC - https://www.digitalruby.com

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace DigitalRuby.IPBan
{
    /// <summary>
    /// Log file to parse data
    /// </summary>
    [XmlRoot("LogFile")]
    public class IPBanLogFileToParse
    {
        /// <summary>
        /// Source
        /// </summary>
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// Path and mask, one per line
        /// </summary>
        public string PathAndMask { get; set; } = string.Empty;

        /// <summary>
        /// Recursive directory search?
        /// </summary>
        public bool Recursive { get; set; }

        /// <summary>
        /// Failed login regex
        /// </summary>
        public IPBanExtensionMethods.XmlCData FailedLoginRegex { get; set; } = string.Empty;

        /// <summary>
        /// Successful login regex
        /// </summary>
        public IPBanExtensionMethods.XmlCData SuccessfulLoginRegex { get; set; } = string.Empty;

        /// <summary>
        /// Platform regex, i.e. Windows, Linux, etc.
        /// </summary>
        public IPBanExtensionMethods.XmlCData PlatformRegex { get; set; } = string.Empty;

        /// <summary>
        /// How often in milliseconds to ping the file
        /// </summary>
        public int PingInterval { get; set; } = 10000;

        /// <summary>
        /// Max file size in bytes
        /// </summary>
        public int MaxFileSize { get; set; }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            return string.Format("Path/mask: {0}, platform: {1}", PathAndMask, PlatformRegex);
        }
    }

    /// <summary>
    /// Log files to parse
    /// </summary>
    [XmlType("LogFilesToParse")]
    public class IPBanLogFilesToParse
    {
        /// <summary>
        /// Log files
        /// </summary>
        [XmlArray("LogFiles")]
        [XmlArrayItem("LogFile")]
        public IPBanLogFileToParse[] LogFiles;
    }
}
