﻿// The Nova Project by Ken Beckett.
// Copyright (C) 2007-2012 Inevitable Software, all rights reserved.
// Released under the Common Development and Distribution License, CDDL-1.0: http://opensource.org/licenses/cddl1.php

using Nova.Parsing;

namespace Nova.CodeDOM
{
    /// <summary>
    /// The common base class of all unary operators (<see cref="PreUnaryOperator"/>, <see cref="PostUnaryOperator"/>).
    /// </summary>
    public abstract class UnaryOperator : Operator
    {
        #region /* FIELDS */

        protected Expression _expression;

        // If the operator is overloaded, a hidden reference (OperatorRef) to the overloaded
        // operator declaration is stored here.
        protected SymbolicRef _operatorRef;

        #endregion

        #region /* CONSTRUCTORS */

        protected UnaryOperator(Expression expression)
        {
            Expression = expression;
        }

        #endregion

        #region /* PROPERTIES */

        /// <summary>
        /// The associated <see cref="Expression"/>.
        /// </summary>
        public Expression Expression
        {
            get { return _expression; }
            set { SetField(ref _expression, value, true); }
        }

        /// <summary>
        /// A hidden OperatorRef to an overloaded operator declaration (if any).
        /// </summary>
        public override SymbolicRef HiddenRef
        {
            get { return _operatorRef; }
        }

        /// <summary>
        /// True if the expression is const.
        /// </summary>
        public override bool IsConst
        {
            // If the expression is const, then the result will be const
            get { return (_expression != null && _expression.IsConst); }
        }

        #endregion

        #region /* METHODS */

        /// <summary>
        /// The internal name of the <see cref="UnaryOperator"/>.
        /// </summary>
        public virtual string GetInternalName()
        {
            return null;
        }

        /// <summary>
        /// Deep-clone the code object.
        /// </summary>
        public override CodeObject Clone()
        {
            UnaryOperator clone = (UnaryOperator)base.Clone();
            clone.CloneField(ref clone._expression, _expression);
            clone.CloneField(ref clone._operatorRef, _operatorRef);
            return clone;
        }

        #endregion

        #region /* PARSING */

        protected UnaryOperator(Parser parser, CodeObject parent)
            : base(parser, parent)
        {
            // Get rid of parens around the expression if they're not necessary
            if (AutomaticFormattingCleanup && !parser.IsGenerated && _expression != null
                && !(_expression is Operator && ((Operator)_expression).GetPrecedence() > GetPrecedence()))
                _expression.HasParens = false;
        }

        #endregion

        #region /* FORMATTING */

        /// <summary>
        /// Determines if the code object only requires a single line for display.
        /// </summary>
        public override bool IsSingleLine
        {
            get { return (base.IsSingleLine && (_expression == null || (!_expression.IsFirstOnLine && _expression.IsSingleLine))); }
            set
            {
                base.IsSingleLine = value;
                if (value && _expression != null)
                {
                    _expression.IsFirstOnLine = false;
                    _expression.IsSingleLine = true;
                }
            }
        }

        protected override void DefaultFormatField(CodeObject field)
        {
            base.DefaultFormatField(field);

            // Force parens around the expression if it's an operator with lower precedence than the current one,
            // otherwise parens aren't necessary so force them off.
            Expression expression = (Expression)field;
            expression.HasParens = (expression is Operator && ((Operator)expression).GetPrecedence() > GetPrecedence());
        }

        #endregion
    }
}
