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

#region Imports

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

#endregion Imports

namespace DigitalRuby.IPBan
{
    /// <summary>
    /// Info about a single Windows event viewer lookup
    /// </summary>
    public class EventViewerExpression
    {
        /// <summary>
        /// The regex, created from Regex property
        /// </summary>
        [XmlIgnore]
        public Regex RegexObject { get; private set; }

        /// <summary>
        /// Xpath to find
        /// </summary>
        public string XPath { get; set; } = string.Empty;

        private string regex = string.Empty;
        /// <summary>
        /// Regex string
        /// </summary>
        public IPBanExtensionMethods.XmlCData Regex
        {
            get { return regex; }
            set { RegexObject = IPBanConfig.ParseRegex(regex = value); }
        }
    }

    /// <summary>
    /// A single Windows event viewer group
    /// </summary>
    [XmlRoot("Group")]
    public class EventViewerExpressionGroup
    {
        /// <summary>
        /// The event viewer source
        /// </summary>
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// Keywords as a ULONG
        /// </summary>
        [XmlIgnore]
        public ulong KeywordsULONG { get; private set; }

        /// <summary>
        /// Minimum Windows major version - see https://msdn.microsoft.com/en-us/library/windows/desktop/ms724832%28v=vs.85%29.aspx?f=255&MSPPError=-2147217396
        /// </summary>
        public int MinimumWindowsMajorVersion { get; set; } = 6;

        /// <summary>
        /// Minimum Windows minor version - see https://msdn.microsoft.com/en-us/library/windows/desktop/ms724832%28v=vs.85%29.aspx?f=255&MSPPError=-2147217396
        /// </summary>
        public int MinimumWindowsMinorVersion { get; set; } = 0;

        /// <summary>
        /// Set automatically, determines whether this expression is failed login (false) or successful login (true)
        /// </summary>
        public bool NotifyOnly { get; set; }

        /// <summary>
        /// Keywords backing variable, private
        /// </summary>
        private string keywords;

        /// <summary>
        /// Keywords as HEX string
        /// </summary>
        public string Keywords
        {
            get { return keywords; }
            set
            {
                keywords = value;

                // parse, removing any 0x prefix
                if (value.StartsWith("0x"))
                {
                    value = value.Substring(2);
                }                    
                KeywordsULONG = ulong.Parse(value, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
            }
        }

        public string GetQueryString(int id = 1)
        {
            ulong keywordsDecimal = ulong.Parse(Keywords.Substring(2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
            return "<Query Id='" + id.ToString(CultureInfo.InvariantCulture) + "' Path='" + Path + "'><Select Path='" + Path + "'>*[System[(band(Keywords," + keywordsDecimal.ToString() + "))]]</Select></Query>";
        }

        public void SetExpressionsFromExpressionsText()
        {
            if (ExpressionsText == null)
            {
                return;
            }

            Expressions.Clear();
            string[] lines = IPBanConfig.CleanMultilineString(ExpressionsText).Split('\n');
            string line;
            EventViewerExpression currentExpression = null;
            for (int i = 0; i < lines.Length; i++)
            {
                line = lines[i].Trim();
                if (line.StartsWith("//"))
                {
                    if (currentExpression != null)
                    {
                        Expressions.Add(currentExpression);
                    }
                    currentExpression = new EventViewerExpression { XPath = line, Regex = string.Empty };
                }
                else if (line.Length != 0 && currentExpression != null)
                {
                    currentExpression.Regex += line + "\n";
                }
            }
            if (currentExpression != null)
            {
                Expressions.Add(currentExpression);
            }
        }

        /// <summary>
        /// Path to the event viewer entry, i.e. Application or Security
        /// </summary>
        public string Path { get; set; } = string.Empty;

        [XmlArray("Expressions")]
        [XmlArrayItem("Expression")]
        public List<EventViewerExpression> Expressions { get; set; } = new List<EventViewerExpression>();

        /// <summary>
        /// If using plain text expressions, this will be set and needs conversion. Leave null if you are using Expressions directly.
        /// The format is xpath (//*), newline, and then regex, newline, repeated.
        /// </summary>
        [XmlIgnore]
        public string ExpressionsText { get; set; }
    }

    /// <summary>
    /// Base class for event viewer expressions
    /// </summary>
    public class EventViewerExpressions
    {
        [XmlArray("Groups")]
        [XmlArrayItem("Group")]
        public List<EventViewerExpressionGroup> Groups { get; set; } = new List<EventViewerExpressionGroup>();
    }

    /// <summary>
    /// List of Windows event viewer groups for failed login attempts
    /// </summary>
    [XmlType("ExpressionsToBlock")]
    public class EventViewerExpressionsToBlock : EventViewerExpressions { }

    /// <summary>
    /// List of Windows event viewer groups for success login attempts
    /// </summary>
    [XmlType("ExpressionsToNotify")]
    public class EventViewerExpressionsToNotify : EventViewerExpressions { }
}
