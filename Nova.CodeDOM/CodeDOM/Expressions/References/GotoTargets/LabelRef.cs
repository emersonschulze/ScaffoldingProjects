﻿// The Nova Project by Ken Beckett.
// Copyright (C) 2007-2012 Inevitable Software, all rights reserved.
// Released under the Common Development and Distribution License, CDDL-1.0: http://opensource.org/licenses/cddl1.php

namespace Nova.CodeDOM
{
    /// <summary>
    /// Represents a reference to a <see cref="Label"/>.
    /// </summary>
    public class LabelRef : GotoTargetRef
    {
        #region /* CONSTRUCTORS */

        /// <summary>
        /// Create a <see cref="LabelRef"/>.
        /// </summary>
        public LabelRef(Label declaration, bool isFirstOnLine)
            : base(declaration, isFirstOnLine)
        { }

        /// <summary>
        /// Create a <see cref="LabelRef"/>.
        /// </summary>
        public LabelRef(Label declaration)
            : base(declaration, false)
        { }

        #endregion
    }
}
