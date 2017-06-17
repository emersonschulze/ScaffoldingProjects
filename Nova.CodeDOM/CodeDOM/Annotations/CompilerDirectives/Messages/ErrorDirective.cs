﻿// The Nova Project by Ken Beckett.
// Copyright (C) 2007-2012 Inevitable Software, all rights reserved.
// Released under the Common Development and Distribution License, CDDL-1.0: http://opensource.org/licenses/cddl1.php

using Nova.Parsing;

namespace Nova.CodeDOM
{
    /// <summary>
    /// Forces the compiler to emit an error message.
    /// </summary>
    public class ErrorDirective : MessageDirective
    {
        #region /* CONSTRUCTORS */

        /// <summary>
        /// Create an <see cref="ErrorDirective"/>.
        /// </summary>
        public ErrorDirective(string message)
            : base(message)
        { }

        #endregion

        #region /* PARSING */

        /// <summary>
        /// The token used to parse the code object.
        /// </summary>
        public new const string ParseToken = "error";

        internal static void AddParsePoints()
        {
            Parser.AddCompilerDirectiveParsePoint(ParseToken, Parse);
        }

        /// <summary>
        /// Parse an <see cref="ErrorDirective"/>.
        /// </summary>
        public static ErrorDirective Parse(Parser parser, CodeObject parent, ParseFlags flags)
        {
            return new ErrorDirective(parser, parent);
        }

        /// <summary>
        /// Parse an <see cref="ErrorDirective"/>.
        /// </summary>
        public ErrorDirective(Parser parser, CodeObject parent)
            : base(parser, parent)
        {
            ParseMessage(parser);
        }

        #endregion

        #region /* RENDERING */

        /// <summary>
        /// The keyword associated with the compiler directive (if any).
        /// </summary>
        public override string DirectiveKeyword
        {
            get { return ParseToken; }
        }

        #endregion
    }
}
