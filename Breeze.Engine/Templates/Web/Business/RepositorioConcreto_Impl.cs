using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breeze.Engine.Templates.Web.Business
{
    public partial class RepositorioConcreto : ITemplate
    {
        private readonly MetaDataDefinition Metadata;

        public RepositorioConcreto(MetaDataDefinition Metadata)
        {
            this.Metadata = Metadata;
        }

        public MetaDataDefinition GetMetadata()
        {
            return this.Metadata;
        }
    }
}
