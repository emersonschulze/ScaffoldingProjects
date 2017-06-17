﻿// The Nova Project by Ken Beckett.
// Copyright (C) 2007-2012 Inevitable Software, all rights reserved.
// Released under the Common Development and Distribution License, CDDL-1.0: http://opensource.org/licenses/cddl1.php

using Nova.Parsing;

namespace Nova.CodeDOM
{
    /// <summary>
    /// Embeds a reference to a method or indexer parameter in a documentation comment.
    /// </summary>
    public class DocParamRef : DocNameBase
    {
        #region /* CONSTRUCTORS */

        /// <summary>
        /// Create a <see cref="DocParamRef"/>.
        /// </summary>
        public DocParamRef(ParameterRef parameterRef)
            : base(parameterRef, (string)null)
        { }

        /// <summary>
        /// Create a <see cref="DocParamRef"/>.
        /// </summary>
        public DocParamRef(ParameterDecl parameterDecl)
            : base(parameterDecl.CreateRef(), (string)null)
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
        public new const string ParseToken = "paramref";

        internal static void AddParsePoints()
        {
            Parser.AddDocCommentParseTag(ParseToken, Parse);
        }

        /// <summary>
        /// Parse a <see cref="DocParamRef"/>.
        /// </summary>
        public static new DocParamRef Parse(Parser parser, CodeObject parent, ParseFlags flags)
        {
            return new DocParamRef(parser, parent);
        }

        /// <summary>
        /// Parse a <see cref="DocParamRef"/>.
        /// </summary>
        public DocParamRef(Parser parser, CodeObject parent)
            : base(parser, parent)
        { }

        #endregion

        #region /* FORMATTING */

        /// <summary>
        /// True if the code object defaults to starting on a new line.
        /// </summary>
        public override bool IsFirstOnLineDefault
        {
            get { return false; }
        }

        #endregion
    }
}
