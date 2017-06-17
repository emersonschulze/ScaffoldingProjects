﻿// The Nova Project by Ken Beckett.
// Copyright (C) 2007-2012 Inevitable Software, all rights reserved.
// Released under the Common Development and Distribution License, CDDL-1.0: http://opensource.org/licenses/cddl1.php

using Nova.Parsing;

namespace Nova.CodeDOM
{
    /// <summary>
    /// The common base class of all binary bitwise operators (<see cref="BitwiseAnd"/>, <see cref="BitwiseOr"/>, <see cref="BitwiseXor"/>).
    /// </summary>
    public abstract class BinaryBitwiseOperator : BinaryOperator
    {
        #region /* CONSTRUCTORS */

        protected BinaryBitwiseOperator(Expression left, Expression right)
            : base(left, right)
        { }

        #endregion

        #region /* PARSING */

        protected BinaryBitwiseOperator(Parser parser, CodeObject parent)
            : base(parser, parent)
        { }

        #endregion

        #region /* FORMATTING */

        /// <summary>
        /// True if the expression should have parens by default.
        /// </summary>
        public override bool HasParensDefault
        {
            get { return false; }  // Default to NO parens for binary bitwise operators
        }

        #endregion
    }
}
