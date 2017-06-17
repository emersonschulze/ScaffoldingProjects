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
    internal class InterfaceRepositorioSource : PortalBusinessSourceBase
    {
        public InterfaceRepositorioSource(ITemplate template, IFileHelper fileHelper) : base(template, fileHelper)
        {
        }

        public override bool OverrideFile
        {
            get { return false; }
        }


        protected override string GetFileName()
        {
            return string.Concat("IRepositorio", Template.GetMetadata().ClassName,".cs");
        }
    }
}
