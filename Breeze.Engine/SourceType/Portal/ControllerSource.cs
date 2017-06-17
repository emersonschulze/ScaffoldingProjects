using System;
using System.IO;
using Breeze.Engine.Helpers;
using Breeze.Engine.Templates;

namespace Breeze.Engine.SourceType.Portal
{
    internal sealed class ControllerSource : SourceBase
    {
        public ControllerSource(ITemplate template, IFileHelper fileHelper) : base(template, fileHelper)
        {
        }

        protected override string GetDirectory()
        {
            var webProjectPath = Template.GetMetadata().PortalPath;

            var area = Template.GetAreaName();

            var fileDirectory = (!String.IsNullOrWhiteSpace(area)) ?
                Path.Combine(webProjectPath, "Areas", area, "Controllers") : Path.Combine(webProjectPath, "Controllers");

            return fileDirectory;
        }

        public override bool OverrideFile
        {
            get { return true; }
        }

        protected override string GetFileName()
        {
            return string.Concat(Template.GetMetadata().ClassName, "Controller.cs");
        }
    }
}
