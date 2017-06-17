using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breeze.Engine.Templates.Web.Pages
{
    public partial class CreatePagesImpl : ITemplate
    {
        private MetaDataDefinition Metadata;

        public CreatePagesImpl(MetaDataDefinition Metadata)
        {
            this.Metadata = Metadata;
        }

        public MetaDataDefinition GetMetadata()
        {
            return Metadata;
        }
    }
}
