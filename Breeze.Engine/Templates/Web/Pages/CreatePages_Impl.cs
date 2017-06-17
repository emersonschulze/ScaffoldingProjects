using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breeze.Engine.Templates.Web.Pages
{
    public partial class CreatePages : ITemplate
    {
        private MetaDataDefinition Metadata;

        public CreatePages(MetaDataDefinition Metadata)
        {
            this.Metadata = Metadata;
        }

        public MetaDataDefinition GetMetadata()
        {
            return Metadata;
        }
    }
}
