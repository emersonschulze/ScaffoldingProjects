using System;
using System.IO;
using System.Linq;
using Breeze.Engine.Helpers;
using Breeze.Engine.Templates;

namespace Breeze.Engine.SourceType.WebPages
{
    class CreatePagesSource : SourceBase
    {
        private readonly string sufix;

        public CreatePagesSource(ITemplate template, IFileHelper fileHelper)
            : base(template, fileHelper)
        {
            this.sufix = sufix;
        }

        protected override string GetDirectory()
        {
            var integrationTestPath = Template.GetMetadata().IntegrationTestPath;

            var tokens = Template.GetMetadata().Namespace.Split('.');
            var area = (tokens.Length > 3) ? tokens.Skip(3).ElementAt(0) : tokens.Last();

            var fileDirectory = (!String.IsNullOrWhiteSpace(area)) ? Path.Combine(integrationTestPath, area) : Path.Combine(integrationTestPath);

            return fileDirectory;
        }

        public override bool OverrideFile
        {
            get { return true; }
        }

        protected override string GetFileName()
        {
            return string.Concat(Template.GetMetadata().ClassName, "Page.cs");
        }
    }

    class CreatePagesSourceImpl : CreatePagesSource
    {
        public CreatePagesSourceImpl(ITemplate template, IFileHelper fileHelper)
            : base(template, fileHelper)
        {
        }

        public override bool OverrideFile { get { return false; } }

        protected override string GetFileName()
        {
            return string.Concat(Template.GetMetadata().ClassName, "Page_Impl.cs");
        }
    }
}