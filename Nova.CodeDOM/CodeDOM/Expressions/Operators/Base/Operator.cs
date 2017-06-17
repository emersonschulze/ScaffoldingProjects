﻿// The Nova Project by Ken Beckett.
// Copyright (C) 2007-2012 Inevitable Software, all rights reserved.
// Released under the Common Development and Distribution License, CDDL-1.0: http://opensource.org/licenses/cddl1.php

using Nova.Parsing;
using Nova.Rendering;

namespace Nova.CodeDOM
{
    /// <summary>
    /// The common base class of all operations (binary, unary, or other).
    /// </summary>
    public abstract class Operator : Expression
    {
        #region /* CONSTANTS */

        /// <summary>
        /// Name prefix used for overloadable operator names.
        /// </summary>
        public const string NamePrefix = "op_";

        #endregion

        #region /* CONSTRUCTORS */

        protected Operator()
        { }

        #endregion

        #region /* PROPERTIES */

        /// <summary>
        /// The symbol associated with the operator.
        /// </summary>
        public virtual string Symbol
        {
            get { return null; }
        }

        #endregion

        #region /* PARSING */

        protected Operator(Parser parser, CodeObject parent)
            : base(parser, parent)
        { }

        /// <summary>
        /// Get the precedence of the operator.
        /// </summary>
        public abstract int GetPrecedence();

        #endregion

        #region /* RENDERING */

        /// <summary>
        /// Convert operator to text.
        /// </summary>
        protected virtual void AsTextOperator(CodeWriter writer, RenderFlags flags)
        {
            writer.Write(Symbol);
        }

        #endregion
    }

    // OPERATOR PRECEDENCE AND ASSOCIATIVITY:
    // ======================================
    //
    // Operator precedence and associativity are determined by the individual operator
    // classes, when they register with the Parser.  The table below is provided as a
    // quick-reference.  A lower Precedence value means higher precedence.  When a series
    // of the same operator occur, such as "a + b + c", the associativity determines how
    // they are grouped: Left means "(a + b) + c" while right means "a + (b + c)".
    // Precedence values are spaced out to allow for the addition of new operators.
    //
    // Category           Operators              Precedence  Associativity  Overloadable
    // -----------------  ----------             ----------  -------------  ------------
    // Primary            x.y f(x) a[x] x++ x--     100          Left       x++ x--
    //                    new  typeof  sizeof                               (a[x] via indexers)
    //                    checked  unchecked
    //                    default ::
    // Unary              +  -  !  ~  ++x  --x      200          Left       +  -  !  ~  ++x  --x
    //                    (T)x
    // Multiplicative     *  /  %                   300          Left       *  /  %
    // Additive           +  -                      310          Left       +  -
    // Shift              <<  >>                    320          Left       <<  >>
    // Relational/Types   <  >  <=  >=  is  as      330          Left       <  >  <=  >=
    // Equality           ==  !=                    340          Left       ==  !=
    // Bitwise AND        &                         350          Left       &
    // Bitwise XOR        ^                         360          Left       ^
    // Bitwise OR         |                         365          Left       |
    // AND                &&                        370          Left
    // OR                 ||                        385          Left
    // IfNullThen         ??                        390          Right
    // Conditional        ? :                       400          Right
    // Assignment         =  *=  /=  %=  +=  -=     500          Right
    //                    <<=  >>=  &=  ^=  |=
    // Ref/Out            ref  out                  920          n/a
}
