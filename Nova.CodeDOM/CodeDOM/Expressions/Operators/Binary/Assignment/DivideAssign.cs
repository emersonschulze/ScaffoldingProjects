﻿// The Nova Project by Ken Beckett.
// Copyright (C) 2007-2012 Inevitable Software, all rights reserved.
// Released under the Common Development and Distribution License, CDDL-1.0: http://opensource.org/licenses/cddl1.php

using Nova.Parsing;

namespace Nova.CodeDOM
{
    /// <summary>
    /// Divides one <see cref="Expression"/> by another, and assigns the result to the left <see cref="Expression"/>.
    /// The left <see cref="Expression"/> must be an assignable object ("lvalue").
    /// </summary>
    public class DivideAssign : Assignment
    {
        #region /* CONSTRUCTORS */

        /// <summary>
        /// Create a <see cref="DivideAssign"/> operator.
        /// </summary>
        public DivideAssign(Expression left, Expression right)
            : base(left, right)
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

        #region /* METHODS */

        /// <summary>
        /// The internal name of the <see cref="BinaryOperator"/>.
        /// </summary>
        public override string GetInternalName()
        {
            return Divide.InternalName;
        }

        #endregion

        #region /* PARSING */

        /// <summary>
        /// The token used to parse the code object.
        /// </summary>
        public new const string ParseToken = "/=";

        internal static new void AddParsePoints()
        {
            Parser.AddOperatorParsePoint(ParseToken, Precedence, LeftAssociative, false, Parse);
        }

        /// <summary>
        /// Parse a <see cref="DivideAssign"/> operator.
        /// </summary>
        public static new DivideAssign Parse(Parser parser, CodeObject parent, ParseFlags flags)
        {
            return new DivideAssign(parser, parent);
        }

        protected DivideAssign(Parser parser, CodeObject parent)
            : base(parser, parent)
        { }

        #endregion
    }
}
