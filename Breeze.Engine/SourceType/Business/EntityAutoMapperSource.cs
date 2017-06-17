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
    internal sealed class EntityAutoMapperSource : PortalBusinessSourceBase
    {
        public EntityAutoMapperSource(ITemplate template, IFileHelper fileHelper) : base(template, fileHelper)
        {
        }

        protected override string GetFileName()
        {
            return string.Concat(Template.GetMetadata().ClassName,"ProfileMap.cs");
        }

        public override bool OverrideFile
        {
            get { return false; }
        }
    }
}
