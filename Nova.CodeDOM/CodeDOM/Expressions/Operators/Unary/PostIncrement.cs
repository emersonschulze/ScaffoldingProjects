﻿// The Nova Project by Ken Beckett.
// Copyright (C) 2007-2012 Inevitable Software, all rights reserved.
// Released under the Common Development and Distribution License, CDDL-1.0: http://opensource.org/licenses/cddl1.php

using Nova.Parsing;

namespace Nova.CodeDOM
{
    /// <summary>
    /// Increments an <see cref="Expression"/> *after* it is evaluated.
    /// Use <see cref="Increment"/> instead when possible, because it's more efficient.
    /// </summary>
    public class PostIncrement : PostUnaryOperator
    {
        #region /* CONSTRUCTORS */

        /// <summary>
        /// Create a <see cref="PostIncrement"/> operator.
        /// </summary>
        public PostIncrement(Expression expression)
            : base(expression)
        { }

        #endregion

        #region /* PROPERTIES */

        /// <summary>
        /// The symbol associated with the operator.
        /// </summary>
        public override string Symbol
        {
            get { return ParseToken; }
        }

        #endregion

        #region /* PARSING */

        /// <summary>
        /// The token used to parse the code object.
        /// </summary>
        public const string ParseToken = Increment.ParseToken;

        /// <summary>
        /// The precedence of the operator.
        /// </summary>
        public const int Precedence = 100;

        /// <summary>
        /// True if the operator is left-associative, or false if it's right-associative.
        /// </summary>
        public const bool LeftAssociative = true;

        internal static new void AddParsePoints()
        {
            // Use a parse-priority of 100 (Increment uses 0)
            Parser.AddOperatorParsePoint(ParseToken, 100, Precedence, LeftAssociative, false, Parse);
        }

        /// <summary>
        /// Parse a <see cref="PostIncrement"/> operator.
        /// </summary>
        public static PostIncrement Parse(Parser parser, CodeObject parent, ParseFlags flags)
        {
            return new PostIncrement(parser, parent);
        }

        protected PostIncrement(Parser parser, CodeObject parent)
            : base(parser, parent)
        { }

        /// <summary>
        /// Get the precedence of the operator.
        /// </summary>
        public override int GetPrecedence()
        {
            return Precedence;
        }

        #endregion
    }
}
