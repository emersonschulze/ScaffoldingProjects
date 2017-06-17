﻿// The Nova Project by Ken Beckett.
// Copyright (C) 2007-2012 Inevitable Software, all rights reserved.
// Released under the Common Development and Distribution License, CDDL-1.0: http://opensource.org/licenses/cddl1.php

using System;
using System.Collections.Generic;

using Nova.Rendering;

namespace Nova.CodeDOM
{
    /// <summary>
    /// Represents a namespace of type declarations and optional child namespaces.
    /// </summary>
    /// <remarks>
    /// Unlike <see cref="NamespaceDecl"/>, which models individual namespace declaration statements and their contained statements,
    /// there is only a single instance of this class for a namespace, and it contains all of the <see cref="TypeDecl"/>
    /// (for types declared in the same solution) and/or <see cref="Type"/> (for external types) and
    /// child <see cref="Namespace"/> objects that currently exist in the namespace.
    /// The Parent of a Namespace instance should only be another Namespace, but the Parent of a <see cref="RootNamespace"/>
    /// can be either a <see cref="Project"/> (global namespace) or a <see cref="Reference"/> (extern alias namespace).
    /// </remarks>
    public class Namespace : CodeObject, INamedCodeObject, INamespace
    {
        #region /* FIELDS */

        /// <summary>
        /// The name of the Namespace (does not include any parent Namespaces).
        /// </summary>
        protected string _name;

        /// <summary>
        /// The full name of the Namespace (including any parent Namespaces).
        /// </summary>
        /// <remarks>
        /// This full name must be re-generated if any Namespace in it is renamed or moved.
        /// It's used to improve performance, so that the same string doesn't have to be
        /// generated many times during symbol resolution.
        /// </remarks>
        protected string _fullName;

        /// <summary>
        /// True if this namespace has <see cref="NamespaceDecl"/> declarations in the current project,
        /// otherwise false (meaning items in the namespace exist only in imported assemblies
        /// and projects).
        /// </summary>
        protected bool _hasDeclarationsInProject;

        /// <summary>
        /// Dictionary of child namespaces and types (<see cref="Namespace"/>s, <see cref="TypeDecl"/>s, and <see cref="Type"/>s) by name.
        /// </summary>
        protected NamespaceTypeDictionary _children = new NamespaceTypeDictionary();

        #endregion

        #region /* CONSTRUCTORS */

        /// <summary>
        /// Create a <see cref="Namespace"/> with the specified name and parent.
        /// </summary>
        public Namespace(string name, CodeObject parent)
        {
            _name = name;
            Parent = parent;
        }

        #endregion

        #region /* PROPERTIES */

        /// <summary>
        /// The name of the <see cref="Namespace"/>.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                UpdateFullName();
            }
        }

        /// <summary>
        /// The full name of the <see cref="Namespace"/>, including any parent namespaces.
        /// </summary>
        public string FullName
        {
            get { return _fullName; }
        }

        /// <summary>
        /// The descriptive category of the code object.
        /// </summary>
        public string Category
        {
            get { return "namespace"; }
        }

        /// <summary>
        /// The dictionary of child objects in the <see cref="Namespace"/>.
        /// </summary>
        public NamespaceTypeDictionary Children
        {
            get { return _children; }
        }

        /// <summary>
        /// The parent <see cref="CodeObject"/>.
        /// </summary>
        public override CodeObject Parent
        {
            set
            {
                base.Parent = value;
                UpdateFullName();
            }
        }

        /// <summary>
        /// Determines if this <see cref="Namespace"/> is root-level (global or extern alias).
        /// </summary>
        public virtual bool IsRootLevel
        {
            get { return false; }
        }

        /// <summary>
        /// Determines if this <see cref="Namespace"/> is the project-global namespace.
        /// </summary>
        public virtual bool IsGlobal
        {
            get { return false; }
        }

        /// <summary>
        /// True if this <see cref="Namespace"/> has <see cref="NamespaceDecl"/> declarations in the current project, otherwise
        /// false (meaning items in the namespace exist only in imported assemblies and projects).
        /// </summary>
        public bool HasDeclarationsInProject
        {
            get { return _hasDeclarationsInProject; }
        }

        #endregion

        #region /* METHODS */

        /// <summary>
        /// Add the specified child <see cref="Namespace"/> to the current <see cref="Namespace"/>.
        /// </summary>
        public void Add(Namespace @namespace)
        {
            lock (this)
            {
                _children.Add(@namespace);
                @namespace.Parent = this;
            }
        }

        /// <summary>
        /// Add the specified <see cref="TypeDecl"/> to the current <see cref="Namespace"/>.
        /// </summary>
        public void Add(TypeDecl typeDecl)
        {
            lock (this)
                _children.Add(typeDecl);
        }

        /// <summary>
        /// Add the specified <see cref="Type"/> to the current <see cref="Namespace"/>.
        /// </summary>
        public void Add(Type type)
        {
            lock (this)
                _children.Add(type);
        }

        /// <summary>
        /// Remove the specified child <see cref="Namespace"/> from the current <see cref="Namespace"/>.
        /// </summary>
        public void Remove(Namespace @namespace)
        {
            lock (this)
                _children.Remove(@namespace);
        }

        /// <summary>
        /// Remove the specified <see cref="TypeDecl"/> from the current <see cref="Namespace"/>.
        /// </summary>
        public void Remove(TypeDecl typeDecl)
        {
            lock (this)
                _children.Remove(typeDecl);
        }

        /// <summary>
        /// Remove the specified <see cref="Type"/> from the current <see cref="Namespace"/>.
        /// </summary>
        public void Remove(Type type)
        {
            lock (this)
                _children.Remove(type);
        }

        /// <summary>
        /// Remove all items from the <see cref="Namespace"/>.
        /// </summary>
        public virtual void RemoveAll()
        {
            lock (this)
                _children.Clear();
        }

        /// <summary>
        /// Create a reference to the <see cref="Namespace"/>.
        /// </summary>
        /// <param name="isFirstOnLine">True if the reference should be displayed on a new line.</param>
        /// <returns>A <see cref="NamespaceRef"/>.</returns>
        public override SymbolicRef CreateRef(bool isFirstOnLine)
        {
            return new NamespaceRef(this, isFirstOnLine);
        }

        protected virtual void NamespaceCreated(Namespace @namespace)
        {
            if (_parent != null)
                ((Namespace)_parent).NamespaceCreated(@namespace);
        }

        /// <summary>
        /// Find or create a child <see cref="Namespace"/>, including any missing parent namespaces.
        /// </summary>
        public virtual Namespace FindOrCreateChildNamespace(string namespaceName)
        {
            Namespace @namespace = null;
            string prefix = RemovePrefix(ref namespaceName);

            lock (this)
            {
                object obj = _children.Find(prefix);
                if (obj is Namespace)
                    @namespace = (Namespace)obj;
                else if (obj is NamespaceTypeGroup)
                {
                    foreach (object childObj in (NamespaceTypeGroup)obj)
                    {
                        if (childObj is Namespace)
                        {
                            @namespace = (Namespace)childObj;
                            break;
                        }
                    }
                }
                if (@namespace == null)
                {
                    @namespace = new Namespace(prefix, this);
                    Add(@namespace);
                    NamespaceCreated(@namespace);
                }
            }

            if (!string.IsNullOrEmpty(namespaceName))
                @namespace = @namespace.FindOrCreateChildNamespace(namespaceName);
            return @namespace;
        }

        /// <summary>
        /// Find a child <see cref="Namespace"/>, <see cref="TypeDecl"/>, or <see cref="Type"/> with
        /// the specified name.
        /// </summary>
        /// <param name="name">The name of the child namespace or type (may contain namespace and/or parent type prefixes).</param>
        /// <returns>The matching <see cref="TypeDecl"/>, <see cref="Type"/>, <see cref="Namespace"/>,
        /// <see cref="NamespaceTypeGroup"/>, or null if not found.</returns>
        public object Find(string name)
        {
            string prefix = RemovePrefix(ref name);
            object obj;
            lock (this)
                obj = _children.Find(prefix);
            if (string.IsNullOrEmpty(name))
                return obj;

            // Handle names with namespace prefixes
            if (obj is Namespace)
                return ((Namespace)obj).Find(name);

            // Handle nested types
            if (obj is TypeDecl)
                return ((TypeDecl)obj).GetNestedType(name);
            if (obj is Type)
                return ((Type)obj).GetNestedType(name);

            return null;
        }

        /// <summary>
        /// Get an enumerator for all children objects of type <typeparamref name="T"/> in
        /// the <see cref="Namespace"/> and in any child Namespaces (recursively).
        /// </summary>
        /// <typeparam name="T">May be <see cref="TypeDecl"/> (or a derived type), <see cref="Type"/>,
        /// <see cref="Namespace"/>, or <see cref="object"/> to return objects of all types.</typeparam>
        public IEnumerable<T> GetAllChildren<T>()
        {
            foreach (object obj in _children)
            {
                if (obj is T)
                    yield return (T)obj;
                if (obj is Namespace)
                {
                    foreach (T typeDecl in ((Namespace)obj).GetAllChildren<T>())
                        yield return typeDecl;
                }
            }
        }

        /// <summary>
        /// Parse the specified name into a child <see cref="NamespaceRef"/> or <see cref="TypeRef"/> on the current namespace,
        /// or a <see cref="Dot"/> expression that evaluates to one.
        /// </summary>
        public virtual Expression ParseName(string name)
        {
            string prefix = RemovePrefix(ref name);
            SymbolicRef symbolicRef;
            object obj;
            lock (this)
                obj = _children.Find(prefix);
            if (obj != null)
            {
                if (obj is Namespace)
                    symbolicRef = new NamespaceRef((Namespace)obj);
                else if (obj is ITypeDecl)
                    symbolicRef = new TypeRef((ITypeDecl)obj);
                else //if (obj is Type)
                    symbolicRef = new TypeRef((Type)obj);
            }
            else
                symbolicRef = new UnresolvedRef(prefix);
            return (string.IsNullOrEmpty(name) ? symbolicRef : ParseName(symbolicRef, name));
        }

        /// <summary>
        /// Parse the specified name into a child <see cref="NamespaceRef"/> or <see cref="TypeRef"/> on the existing
        /// <see cref="Lookup"/> or <see cref="Dot"/> expression.
        /// </summary>
        protected Expression ParseName(Expression expression, string name)
        {
            Expression rightMost = expression.SkipPrefixes();
            do
            {
                string prefix = RemovePrefix(ref name);
                SymbolicRef newRight;
                if (rightMost is NamespaceRef)
                    newRight = (SymbolicRef)((NamespaceRef)rightMost).Namespace.ParseName(prefix);
                else if (rightMost is TypeRef)
                    newRight = ((TypeRef)rightMost).GetNestedType(prefix);
                else
                    newRight = new UnresolvedRef(prefix);
                expression = new Dot(expression, newRight);
                rightMost = newRight;
            }
            while (!string.IsNullOrEmpty(name));
            return expression;
        }

        /// <summary>
        /// Remove any prefix from the input string (delimited by a '.' or a '+').
        /// </summary>
        /// <param name="input">The input string - with prefix removed on output (empty string if no prefix existed).</param>
        /// <returns>The prefix if any, otherwise the input string.</returns>
        public static string RemovePrefix(ref string input)
        {
            string prefix;
            int dot = input.IndexOfAny(new[] { '.', '+' });
            if (dot > 0)
            {
                prefix = input.Substring(0, dot);
                input = input.Substring(dot + 1);
            }
            else
            {
                prefix = input;
                input = "";
            }
            return prefix;
        }

        protected internal void SetDeclarationsInProject(bool hasDeclarationsInProject)
        {
            _hasDeclarationsInProject = hasDeclarationsInProject;
        }

        /// <summary>
        /// Add the <see cref="CodeObject"/> to the specified dictionary.
        /// </summary>
        public void AddToDictionary(NamedCodeObjectDictionary dictionary)
        {
            dictionary.Add(Name, this);
        }

        /// <summary>
        /// Remove the <see cref="CodeObject"/> from the specified dictionary.
        /// </summary>
        public void RemoveFromDictionary(NamedCodeObjectDictionary dictionary)
        {
            dictionary.Remove(Name, this);
        }

        /// <summary>
        /// Update the FullName (called when the Name or Parent is changed).
        /// </summary>
        protected virtual void UpdateFullName()
        {
            _fullName = (((Namespace)_parent).IsRootLevel ? _name : ((Namespace)_parent).FullName + "." + _name);
        }

        /// <summary>
        /// Get the full name of the <see cref="Namespace"/>, including any parent namespace names.
        /// </summary>
        public string GetFullName(bool descriptive)
        {
            return _fullName;
        }

        /// <summary>
        /// Get the full name of the <see cref="Namespace"/>, including any parent namespace names.
        /// </summary>
        public string GetFullName()
        {
            return _fullName;
        }

        #endregion

        #region /* RENDERING */

        public override void AsText(CodeWriter writer, RenderFlags flags)
        {
            if (flags.HasFlag(RenderFlags.Description))
                writer.Write(NamespaceDecl.ParseToken + " ");

            writer.Write(FullName);
        }

        #endregion
    }
}
