using System;
using System.IO;
using Breeze.Engine.Helpers;
using Breeze.Engine.Templates;

namespace Breeze.Engine.SourceType.Model
{
    public class MappingSource : SourceBase
    {
        public MappingSource(ITemplate template, IFileHelper fileHelper) : base(template, fileHelper) { }

        protected override string GetDirectory()
        {
            return Path.Combine(Template.GetMetadata().ClassPath, "Mappings");
        }

        protected override string GetFileName()
        {
            return String.Concat(this.Template.GetMetadata().ClassName, "Mapping.cs");
        }

        public override bool OverrideFile
        {
            get { return true; }
        }
    }
}
