﻿// The Nova Project by Ken Beckett.
// Copyright (C) 2007-2012 Inevitable Software, all rights reserved.
// Released under the Common Development and Distribution License, CDDL-1.0: http://opensource.org/licenses/cddl1.php

using Nova.Parsing;

namespace Nova.CodeDOM
{
    /// <summary>
    /// Documents the value of a property.
    /// </summary>
    public class DocValue : DocComment
    {
        #region /* CONSTRUCTORS */

        /// <summary>
        /// Create a <see cref="DocValue"/>.
        /// </summary>
        public DocValue(string text)
            : base(text)
        { }

        /// <summary>
        /// Create a <see cref="DocValue"/>.
        /// </summary>
        public DocValue(params DocComment[] docComments)
            : base(docComments)
        { }

        #endregion

        #region /* PROPERTIES */

        /// <summary>
        /// The XML tag name for the documentation comment.
        /// </summary>
        public override string TagName
        {
            get { return ParseToken; }
        }

        #endregion

        #region /* PARSING */

        /// <summary>
        /// The token used to parse the code object.
        /// </summary>
        public new const string ParseToken = ValueParameterDecl.FixedName;

        internal static void AddParsePoints()
        {
            Parser.AddDocCommentParseTag(ParseToken, Parse);
        }

        /// <summary>
        /// Parse a <see cref="DocValue"/>.
        /// </summary>
        public static new DocValue Parse(Parser parser, CodeObject parent, ParseFlags flags)
        {
            return new DocValue(parser, parent);
        }

        /// <summary>
        /// Parse a <see cref="DocValue"/>.
        /// </summary>
        public DocValue(Parser parser, CodeObject parent)
        {
            ParseTag(parser, parent);  // Ignore any attributes
        }

        #endregion
    }
}
