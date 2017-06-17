﻿// The Nova Project by Ken Beckett.
// Copyright (C) 2007-2012 Inevitable Software, all rights reserved.
// Released under the Common Development and Distribution License, CDDL-1.0: http://opensource.org/licenses/cddl1.php

using System.Collections.Generic;

using Nova.Parsing;
using Nova.Rendering;

namespace Nova.CodeDOM
{
    /// <summary>
    /// Allows for documentation comments to be included from a separate file.
    /// </summary>
    public class DocInclude : DocComment
    {
        #region /* FIELDS */

        protected string _file;
        protected string _path;

        #endregion

        #region /* CONSTRUCTORS */

        /// <summary>
        /// Create a <see cref="DocInclude"/>.
        /// </summary>
        protected DocInclude(string file, string path)
        {
            _file = file;
            _path = path;
        }

        #endregion

        #region /* PROPERTIES */

        public string File
        {
            get { return _file; }
            set { _file = value; }
        }

        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        #endregion

        #region /* PARSING */

        /// <summary>
        /// The token used to parse the code object.
        /// </summary>
        public new const string ParseToken = "include";

        internal static void AddParsePoints()
        {
            Parser.AddDocCommentParseTag(ParseToken, Parse);
        }

        /// <summary>
        /// Parse a <see cref="DocInclude"/>.
        /// </summary>
        public static new DocInclude Parse(Parser parser, CodeObject parent, ParseFlags flags)
        {
            return new DocInclude(parser, parent);
        }

        /// <summary>
        /// The file attribute name.
        /// </summary>
        public const string FileAttributeName = "file";

        /// <summary>
        /// The path attribute name.
        /// </summary>
        public const string PathAttributeName = "path";

        protected DocInclude(Parser parser, CodeObject parent)
        {
            Dictionary<string, object> attributes = ParseTag(parser, parent);
            if (attributes != null)
            {
                // Extract the 'file' and 'path' attributes, ignoring any unexpected attributes
                foreach (KeyValuePair<string, object> attribute in attributes)
                {
                    if (attribute.Key == FileAttributeName)
                        _file = attribute.Value.ToString();
                    else if (attribute.Key == PathAttributeName)
                        _path = attribute.Value.ToString();
                }
            }
        }

        #endregion

        #region /* RENDERING */

        protected override void AsTextStart(CodeWriter writer, RenderFlags flags)
        {
            // Note that the attributes for this tag use single quote delimiters instead of double
            writer.Write("<" + TagName + " " + FileAttributeName + "='" + _file + "' "
                + PathAttributeName + "='" + _path + "'" + (_content == null && !MissingEndTag ? " />" : ">"));
        }

        protected override void AsTextEnd(CodeWriter writer, RenderFlags flags)
        {
            if (_content != null)
                base.AsTextEnd(writer, flags);
        }

        #endregion
    }
}
