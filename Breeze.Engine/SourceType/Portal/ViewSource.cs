using System;
using System.IO;
using System.Linq;
using Breeze.Engine.Helpers;
using Breeze.Engine.Templates;

namespace Breeze.Engine.SourceType.Portal
{
    internal sealed class ViewSource : SourceBase
    {
        private readonly string actionName;

        public ViewSource(ITemplate template, IFileHelper fileHelper, string actionName) : base(template, fileHelper)
        {
            this.actionName = actionName;
        }
        

        protected override string GetDirectory()
        {
            var webProjectPath = Template.GetMetadata().PortalPath;

            var area = Template.GetAreaName();

            var fileDirectory = (!String.IsNullOrWhiteSpace(area)) ?
                Path.Combine(webProjectPath, "Areas", area, "Views", Template.GetMetadata().ClassName) :
                Path.Combine(webProjectPath, "Views", Template.GetMetadata().ClassName);

            return fileDirectory;
        }

        public override bool OverrideFile
        {
            get { return true; }
        }

        protected override string GetFileName()
        {
            return string.Concat(actionName,".cshtml");
        }
    }
}
