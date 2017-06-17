﻿// The Nova Project by Ken Beckett.
// Copyright (C) 2007-2012 Inevitable Software, all rights reserved.
// Released under the Common Development and Distribution License, CDDL-1.0: http://opensource.org/licenses/cddl1.php

using Nova.Parsing;
using Nova.Rendering;

namespace Nova.CodeDOM
{
    /// <summary>
    /// The common base class of <see cref="BaseInitializer"/> and <see cref="ThisInitializer"/>.
    /// </summary>
    public abstract class ConstructorInitializer : Call
    {
        #region /* CONSTRUCTORS */

        protected ConstructorInitializer(SymbolicRef symbolicRef, params Expression[] parameters)
            : base(symbolicRef, parameters)
        { }

        protected ConstructorInitializer(ConstructorRef constructorRef, params Expression[] parameters)
            : base(constructorRef, parameters)
        { }

        protected ConstructorInitializer(ConstructorDecl constructorDecl, params Expression[] parameters)
            : base(constructorDecl.CreateRef(), parameters)
        { }

        #endregion

        #region /* PROPERTIES */

        /// <summary>
        /// The hidden <see cref="ConstructorRef"/> (or <see cref="UnresolvedRef"/>) that represents the constructor being called.
        /// </summary>
        public override SymbolicRef HiddenRef
        {
            get { return _expression as SymbolicRef; }
        }

        #endregion

        #region /* METHODS */

        #endregion

        #region /* PARSING */

        /// <summary>
        /// The token used to parse the code object.
        /// </summary>
        public const string ParseTokenInitializer = ":";

        /// <summary>
        /// Parse a <see cref="ConstructorInitializer"/>.
        /// </summary>
        public static new ConstructorInitializer Parse(Parser parser, CodeObject parent)
        {
            ConstructorInitializer initializer = null;
            if (parser.TokenText == ParseTokenInitializer)
            {
                Token next = parser.PeekNextToken();
                if (next != null)
                {
                    if (next.Text == ThisInitializer.ParseToken)
                        initializer = new ThisInitializer(parser, parent);
                    else if (next.Text == BaseInitializer.ParseToken)
                        initializer = new BaseInitializer(parser, parent);
                }
            }
            return initializer;
        }

        protected ConstructorInitializer(Parser parser, CodeObject parent, string keyword)
            : base(parser, parent, true)
        {
            Token lastHeaderToken = parser.LastToken;
            parser.NextToken();  // Move past ':'
            Token token = parser.Token;
            SetLineCol(token);
            MoveFormatting(token);                                         // Move formatting
            parser.NextToken();                                            // Move past 'this' or 'base'
            ParseArguments(parser, this, ParseTokenStart, ParseTokenEnd);  // Parse arguments

            MoveComments(lastHeaderToken);  // Move any regular comments from before the ':' this object

            // Set the expression to an unresolved reference using the keyword for the name
            Expression = new UnresolvedRef(keyword, LineNumber, ColumnNumber);

            // Force constructor initializers to start on a new line if auto-cleanup is on
            if (AutomaticFormattingCleanup && !parser.IsGenerated)
                IsFirstOnLine = true;
        }

        #endregion

        #region /* FORMATTING */

        /// <summary>
        /// True if the code object defaults to starting on a new line.
        /// </summary>
        public override bool IsFirstOnLineDefault
        {
            get { return true; }
        }

        #endregion

        #region /* RENDERING */

        protected override void AsTextName(CodeWriter writer, RenderFlags flags)
        {
            UpdateLineCol(writer, flags);
            writer.Write(Symbol);
        }

        public override void AsTextExpression(CodeWriter writer, RenderFlags flags)
        {
            writer.Write(ParseTokenInitializer + " ");
            base.AsTextExpression(writer, flags);
        }

        #endregion
    }
}
