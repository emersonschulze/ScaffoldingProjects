﻿// The Nova Project by Ken Beckett.
// Copyright (C) 2007-2012 Inevitable Software, all rights reserved.
// Released under the Common Development and Distribution License, CDDL-1.0: http://opensource.org/licenses/cddl1.php

using Nova.Parsing;

namespace Nova.CodeDOM
{
    /// <summary>
    /// Provides a summary comment for a code object.
    /// </summary>
    public class DocSummary : DocComment
    {
        #region /* CONSTRUCTORS */

        /// <summary>
        /// Create a <see cref="DocSummary"/>.
        /// </summary>
        public DocSummary(string text)
            : base(text)
        { }

        /// <summary>
        /// Create a <see cref="DocSummary"/>.
        /// </summary>
        public DocSummary(params DocComment[] docComments)
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

        #region /* METHODS */

        /// <summary>
        /// Returns the <see cref="DocSummary"/> itself.
        /// </summary>
        public override DocSummary GetDocSummary()
        {
            return this;
        }

        #endregion

        #region /* PARSING */

        /// <summary>
        /// The token used to parse the code object.
        /// </summary>
        public new const string ParseToken = "summary";

        internal static void AddParsePoints()
        {
            Parser.AddDocCommentParseTag(ParseToken, Parse);
        }

        /// <summary>
        /// Parse a <see cref="DocSummary"/>.
        /// </summary>
        public static new DocSummary Parse(Parser parser, CodeObject parent, ParseFlags flags)
        {
            return new DocSummary(parser, parent);
        }

        /// <summary>
        /// Parse a <see cref="DocSummary"/>.
        /// </summary>
        public DocSummary(Parser parser, CodeObject parent)
        {
            ParseTag(parser, parent);  // Ignore any attributes
        }

        #endregion
    }
}
