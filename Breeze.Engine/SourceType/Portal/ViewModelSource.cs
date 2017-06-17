using System;
using System.IO;
using System.Linq;
using Breeze.Engine.Helpers;
using Breeze.Engine.Templates;

namespace Breeze.Engine.SourceType.Portal
{
    internal sealed class ViewModelSource : SourceBase
    {
        public ViewModelSource(ITemplate template, IFileHelper fileHelper) : base(template, fileHelper) { }

        protected override string GetDirectory()
        {
           var portalBusinessPath = Template.GetMetadata().PortalBusinessPath;

            if (string.IsNullOrWhiteSpace(portalBusinessPath))
                throw new ArgumentNullException("PortalBusinessPath", "Não encontrado a propriedade PortalBusinessPath");

            var area = Template.GetAreaName();

            var fileDirectory = (!String.IsNullOrWhiteSpace(area)) ?
                Path.Combine(portalBusinessPath, area, Template.GetMetadata().ClassName, "ViewModels") :
                Path.Combine(portalBusinessPath, Template.GetMetadata().ClassName, "ViewModels");

            return fileDirectory;
        }

        public override bool OverrideFile
        {
            get { return false; }
        }

        protected override string GetFileName()
        {
            return string.Concat(Template.GetMetadata().ClassName, "ViewModel.cs");
        }
    }
}
