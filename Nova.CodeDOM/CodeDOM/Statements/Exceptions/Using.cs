﻿// The Nova Project by Ken Beckett.
// Copyright (C) 2007-2012 Inevitable Software, all rights reserved.
// Released under the Common Development and Distribution License, CDDL-1.0: http://opensource.org/licenses/cddl1.php

using System;

using Nova.Parsing;
using Nova.Rendering;

namespace Nova.CodeDOM
{
    /// <summary>
    /// Enforces the disposal of an object when it's no longer needed.  It accepts either an <see cref="Expression"/> or a
    /// <see cref="LocalDecl"/>, and internally wraps this in a try/finally that calls Dispose() on the object (which must
    /// implement the <see cref="IDisposable"/> interface).
    /// </summary>
    public class Using : BlockStatement
    {
        #region /* FIELDS */

        /// <summary>
        /// Can be an Expression that evaluates to a VariableRef of a type that implements IDisposable, or
        /// a LocalDecl of a type that implements IDisposable.
        /// </summary>
        protected CodeObject _target;

        #endregion

        #region /* CONSTRUCTORS */

        /// <summary>
        /// Create a <see cref="Using"/>.
        /// </summary>
        public Using(LocalDecl localDecl, CodeObject body)
            : base(body, false)
        {
            Target = localDecl;
        }

        /// <summary>
        /// Create a <see cref="Using"/>.
        /// </summary>
        public Using(LocalDecl localDecl)
            : base(null, false)
        {
            Target = localDecl;
        }

        /// <summary>
        /// Create a <see cref="Using"/>.
        /// </summary>
        public Using(Expression expression, CodeObject body)
            : base(body, false)
        {
            Target = expression;
        }

        /// <summary>
        /// Create a <see cref="Using"/>.
        /// </summary>
        public Using(Expression expression)
            : base(null, false)
        {
            Target = expression;
        }

        #endregion

        #region /* PROPERTIES */

        /// <summary>
        /// The target code object.
        /// </summary>
        public CodeObject Target
        {
            get { return _target; }
            set
            {
                // If the target is a LocalDecl and it already has a parent, then assume it's an existing local
                // and create a ref to it, otherwise assume it's a child-target local (or other expression).
                SetField(ref _target, (value is LocalDecl && value.Parent != null ? value.CreateRef() : value), true);
            }
        }

        /// <summary>
        /// True if contains a single nested Using statement as a child.
        /// </summary>
        public bool HasNestedUsing
        {
            get { return (_body != null && !_body.HasBraces && _body.Count == 1 && _body[0] is Using); }
        }

        /// <summary>
        /// True if this is a nested using.
        /// </summary>
        public bool IsNestedUsing
        {
            get { return (_parent is Using && !((Using)_parent).Body.HasBraces && ((Using)_parent).Body.Count == 1); }
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

        protected override bool IsChildIndented(CodeObject obj)
        {
            // The child object can only be indented if it's the first thing on the line
            if (obj.IsFirstOnLine)
            {
                // If the child isn't a nested using and isn't a prefix, it should be indented
                return !(HasNestedUsing && _body[0] == obj) && !IsChildPrefix(obj);
            }
            return false;
        }

        /// <summary>
        /// Deep-clone the code object.
        /// </summary>
        public override CodeObject Clone()
        {
            Using clone = (Using)base.Clone();
            clone.CloneField(ref clone._target, _target);
            return clone;
        }

        #endregion

        #region /* PARSING */

        /// <summary>
        /// True for multi-part statements, such as try/catch/finally or if/else.
        /// </summary>
        public const string ParseToken = "using";

        internal static void AddParsePoints()
        {
            // Use a parse-priority of 200 (Alias uses 0, UsingDirective uses 100)
            Parser.AddParsePoint(ParseToken, 200, Parse, typeof(IBlock));
        }

        /// <summary>
        /// Pase a <see cref="Using"/>.
        /// </summary>
        public static Using Parse(Parser parser, CodeObject parent, ParseFlags flags)
        {
            return new Using(parser, parent);
        }

        protected Using(Parser parser, CodeObject parent)
            : base(parser, parent)
        {
            parser.NextToken();  // Move past 'using'
            ParseExpectedToken(parser, Expression.ParseTokenStartGroup);  // Move past '('

            // Parse either LocalDecl or Expression (object)
            if (LocalDecl.PeekLocalDecl(parser))
                SetField(ref _target, LocalDecl.Parse(parser, this, false, true), false);
            else
                SetField(ref _target, Expression.Parse(parser, this, true, Expression.ParseTokenEndGroup), false);

            ParseExpectedToken(parser, Expression.ParseTokenEndGroup);  // Move past ')'

            new Block(out _body, parser, this, false);  // Parse the body
        }

        #endregion

        #region /* FORMATTING */

        /// <summary>
        /// True if the <see cref="Statement"/> has an argument.
        /// </summary>
        public override bool HasArgument
        {
            get { return true; }
        }

        /// <summary>
        /// True if the <see cref="BlockStatement"/> always requires braces.
        /// </summary>
        public override bool HasBracesAlways
        {
            get { return false; }
        }

        /// <summary>
        /// Determines if the body of the <see cref="BlockStatement"/> should be formatted with braces.
        /// </summary>
        public override bool ShouldHaveBraces()
        {
            // Turn off braces if we have a nested child using
            return (base.ShouldHaveBraces() && !HasNestedUsing);
        }

        /// <summary>
        /// Determines if the code object only requires a single line for display.
        /// </summary>
        public override bool IsSingleLine
        {
            get { return (base.IsSingleLine && (_target == null || (!_target.IsFirstOnLine && _target.IsSingleLine))); }
            set
            {
                base.IsSingleLine = value;
                if (_target != null)
                {
                    if (value)
                        _target.IsFirstOnLine = false;
                    _target.IsSingleLine = value;
                }
            }
        }

        #endregion

        #region /* RENDERING */

        protected override void AsTextAfter(CodeWriter writer, RenderFlags flags)
        {
            base.AsTextAfter(writer, flags | (HasNestedUsing ? RenderFlags.NoBlockIndent : 0));
        }

        protected override void AsTextArgument(CodeWriter writer, RenderFlags flags)
        {
            if (_target != null)
                _target.AsText(writer, flags);
        }

        #endregion
    }
}
