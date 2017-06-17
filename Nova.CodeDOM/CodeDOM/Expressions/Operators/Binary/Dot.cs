﻿// The Nova Project by Ken Beckett.
// Copyright (C) 2007-2012 Inevitable Software, all rights reserved.
// Released under the Common Development and Distribution License, CDDL-1.0: http://opensource.org/licenses/cddl1.php

using Nova.Parsing;
using Nova.Rendering;

namespace Nova.CodeDOM
{
    /// <summary>
    /// Performs a member lookup (either directly on a namespace or type, or indirectly on the evaluated type
    /// of the expression on the left side).
    /// </summary>
    /// <remarks>
    /// If the left side is a <see cref="NamespaceRef"/> or <see cref="TypeRef"/>, then it's a direct lookup,
    /// otherwise it's an indirect lookup on the evaluated type of the left side.  In either case, the right
    /// side must be a <see cref="SymbolicRef"/> representing the member that is being looked-up.
    /// </remarks>
    public class Dot : BinaryOperator
    {
        #region /* CONSTRUCTORS */

        /// <summary>
        /// Create a <see cref="Dot"/> operator.
        /// </summary>
        public Dot(Expression left, SymbolicRef right)
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

        /// <summary>
        /// True if the expression is const.
        /// </summary>
        public override bool IsConst
        {
            get { return (_right != null && _right.IsConst); }
        }

        #endregion

        #region /* STATIC METHODS */

        /// <summary>
        /// Create a Dot expression chain from 2 or more expressions, cloning any SymbolicRefs for convenience.
        /// </summary>
        public static Dot Create(Expression left, params SymbolicRef[] symbolicRefs)
        {
            Dot dot = new Dot(left is SymbolicRef ? (SymbolicRef)left.Clone() : left, (SymbolicRef)symbolicRefs[0].Clone());
            for (int i = 1; i < symbolicRefs.Length; ++i)
                dot = new Dot(dot, (SymbolicRef)symbolicRefs[i].Clone());
            return dot;
        }

        #endregion

        #region /* METHODS */

        /// <summary>
        /// Get the expression on the right of the right-most Lookup or Dot operator (bypass any '::' and '.' prefixes).
        /// </summary>
        public override Expression SkipPrefixes()
        {
            return Right.SkipPrefixes();
        }

        /// <summary>
        /// Get the expression on the left of the left-most dot operator.
        /// </summary>
        public override Expression FirstPrefix()
        {
            return Left.FirstPrefix();
        }

        #endregion

        #region /* PARSING */

        /// <summary>
        /// The token used to parse the code object.
        /// </summary>
        public const string ParseToken = ".";

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
            Parser.AddOperatorParsePoint(ParseToken, Precedence, LeftAssociative, false, Parse);
        }

        /// <summary>
        /// Parse a <see cref="Dot"/> operator.
        /// </summary>
        public static Dot Parse(Parser parser, CodeObject parent, ParseFlags flags)
        {
            return new Dot(parser, parent);
        }

        protected Dot(Parser parser, CodeObject parent)
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

        #region /* FORMATTING */

        /// <summary>
        /// True if the expression should have parens by default.
        /// </summary>
        public override bool HasParensDefault
        {
            get { return false; }
        }

        #endregion

        #region /* RENDERING */

        public override void AsTextExpression(CodeWriter writer, RenderFlags flags)
        {
            // If we're rendering a Description, turn off some flags for the children of the Dot
            RenderFlags passFlags = (flags & (RenderFlags.PassMask & ~(RenderFlags.Description | RenderFlags.ShowParentTypes)));
            if (_left != null)
                _left.AsText(writer, passFlags | RenderFlags.IsPrefix | RenderFlags.NoSpaceSuffix);
            UpdateLineCol(writer, flags);
            AsTextDot(writer);
            if (_right != null)
                _right.AsText(writer, passFlags | RenderFlags.HasDotPrefix | (flags & RenderFlags.Attribute));  // Special case - allow the Attribute flag to pass
        }

        public static void AsTextDot(CodeWriter writer)
        {
            writer.Write(ParseToken);
        }

        #endregion
    }
}
