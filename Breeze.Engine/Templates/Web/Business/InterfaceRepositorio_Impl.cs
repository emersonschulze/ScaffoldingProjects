using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breeze.Engine.Templates.Web.Business
{
    public partial class InterfaceRepositorio : ITemplate
    {
        private readonly MetaDataDefinition Metadata;

        public InterfaceRepositorio(MetaDataDefinition metadata)
        {
            this.Metadata = metadata;
        }

        public MetaDataDefinition GetMetadata()
        {
            return this.Metadata;
        }
    }
}
