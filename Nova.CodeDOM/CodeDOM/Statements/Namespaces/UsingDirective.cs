﻿// The Nova Project by Ken Beckett.
// Copyright (C) 2007-2012 Inevitable Software, all rights reserved.
// Released under the Common Development and Distribution License, CDDL-1.0: http://opensource.org/licenses/cddl1.php

using Nova.Parsing;
using Nova.Rendering;

namespace Nova.CodeDOM
{
    /// <summary>
    /// Imports the contents of a <see cref="Namespace"/> into the current scope.
    /// </summary>
    public class UsingDirective : Statement
    {
        #region /* FIELDS */

        /// <summary>
        /// The expression should be either a <see cref="NamespaceRef"/>, or a <see cref="Dot"/> operator whose right-most
        /// operand evaluates to a <see cref="NamespaceRef"/>.
        /// </summary>
        protected Expression _namespace;

        #endregion

        #region /* CONSTRUCTORS */

        /// <summary>
        /// Create a <see cref="UsingDirective"/>.
        /// </summary>
        public UsingDirective(Expression expression)
        {
            Namespace = expression;
        }

        #endregion

        #region /* PROPERTIES */

        /// <summary>
        /// The namespace <see cref="Expression"/>.
        /// </summary>
        public Expression Namespace
        {
            get { return _namespace; }
            set { SetField(ref _namespace, value, true); }
        }

        /// <summary>
        /// The keyword associated with the <see cref="Statement"/>.
        /// </summary>
        public override string Keyword
        {
            get { return ParseToken; }
        }

        #endregion

        #region /* METHODS */

        /// <summary>
        /// Evaluate the namespace expression into the targeted <see cref="NamespaceRef"/>.
        /// </summary>
        public NamespaceRef GetNamespaceRef()
        {
            Expression expression = _namespace.SkipPrefixes();
            return (expression is NamespaceRef ? (NamespaceRef)expression : (expression is AliasRef ? ((AliasRef)expression).Namespace : null));
        }

        /// <summary>
        /// Deep-clone the code object.
        /// </summary>
        public override CodeObject Clone()
        {
            UsingDirective clone = (UsingDirective)base.Clone();
            clone.CloneField(ref clone._namespace, _namespace);
            return clone;
        }

        #endregion

        #region /* PARSING */

        /// <summary>
        /// The token used to parse the code object.
        /// </summary>
        public const string ParseToken = "using";

        internal static void AddParsePoints()
        {
            // Use a parse-priority of 100 (Alias uses 0, Using uses 200)
            Parser.AddParsePoint(ParseToken, 100, Parse, typeof(NamespaceDecl));
        }

        /// <summary>
        /// Parse a <see cref="UsingDirective"/>.
        /// </summary>
        public static UsingDirective Parse(Parser parser, CodeObject parent, ParseFlags flags)
        {
            if (parser.PeekNextTokenText() != Expression.ParseTokenStartGroup)
                return new UsingDirective(parser, parent);
            return null;
        }

        protected UsingDirective(Parser parser, CodeObject parent)
            : base(parser, parent)
        {
            parser.NextToken();  // Move past 'using'
            SetField(ref _namespace, Expression.Parse(parser, this, true), false);
            ParseTerminator(parser);
        }

        #endregion

        #region /* FORMATTING */

        /// <summary>
        /// True if the <see cref="Statement"/> has parens around its argument.
        /// </summary>
        public override bool HasArgumentParens
        {
            get { return false; }
        }

        /// <summary>
        /// True if the <see cref="Statement"/> has a terminator character by default.
        /// </summary>
        public override bool HasTerminatorDefault
        {
            get { return true; }
        }

        /// <summary>
        /// Determine a default of 1 or 2 newlines when adding items to a <see cref="Block"/>.
        /// </summary>
        public override int DefaultNewLines(CodeObject previous)
        {
            // Default to a preceeding blank line if the object has first-on-line annotations
            if (HasFirstOnLineAnnotations)
                return 2;

            // Default to a preceeding blank line if the previous object was another using directive
            // with a different root namespace, otherwise use a single newline.
            if (previous is UsingDirective)
            {
                SymbolicRef symbolicRef = ((UsingDirective)previous).Namespace.FirstPrefix() as SymbolicRef;
                if (symbolicRef != null && !symbolicRef.IsSameRef(Namespace.FirstPrefix() as SymbolicRef))
                    return 2;
                return 1;
            }

            // Default to no preceeding blank line if the previous object was an alias directive with
            // the same root namespace, otherwise use a preceeding blank line.
            if (previous is Alias)
            {
                SymbolicRef symbolicRef = ((Alias)previous).Expression.FirstPrefix() as SymbolicRef;
                if (symbolicRef != null && symbolicRef.IsSameRef(Namespace.FirstPrefix() as SymbolicRef))
                    return 1;
                return 2;
            }

            // Default to a preceeding blank line if the object is a different type than the previous one
            if (previous.GetType() != GetType())
                return 2;
            return 1;
        }

        /// <summary>
        /// Determines if the code object only requires a single line for display.
        /// </summary>
        public override bool IsSingleLine
        {
            get { return (base.IsSingleLine && (_namespace == null || (!_namespace.IsFirstOnLine && _namespace.IsSingleLine))); }
            set
            {
                base.IsSingleLine = value;
                if (value && _namespace != null)
                {
                    _namespace.IsFirstOnLine = false;
                    _namespace.IsSingleLine = true;
                }
            }
        }

        #endregion

        #region /* RENDERING */

        protected override void AsTextArgument(CodeWriter writer, RenderFlags flags)
        {
            if (_namespace != null)
                _namespace.AsText(writer, flags);
        }

        #endregion
    }
}
