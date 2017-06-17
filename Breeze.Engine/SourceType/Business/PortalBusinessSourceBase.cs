using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Breeze.Engine.Helpers;
using Breeze.Engine.Templates;

namespace Breeze.Engine.SourceType.Business
{
    internal abstract class PortalBusinessSourceBase : SourceBase
    {
        protected PortalBusinessSourceBase(ITemplate template, IFileHelper fileHelper) : base(template, fileHelper)
        {
        }

        protected override string GetDirectory()
        {
             var portalBusinessPath = Template.GetMetadata().PortalBusinessPath;

            if (string.IsNullOrWhiteSpace(portalBusinessPath))
                throw new ArgumentNullException("PortalBusinessPath", "Não encontrado a propriedade PortalBusinessPath");

            var area = Template.GetAreaName();

            var fileDirectory = (!String.IsNullOrWhiteSpace(area)) ?
                Path.Combine(portalBusinessPath, area, Template.GetMetadata().ClassName) : Path.Combine(portalBusinessPath, Template.GetMetadata().ClassName);

            return fileDirectory;
        }

        protected abstract override string GetFileName();
    }
}
