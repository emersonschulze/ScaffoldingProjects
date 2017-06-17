using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breeze.Engine.Templates
{
    public partial class Mapping : ITemplate
    {
        private readonly MetaDataDefinition MetaData;
        
        public Mapping(MetaDataDefinition MetaData)
        {
            this.MetaData = MetaData;
        }

        public MetaDataDefinition GetMetadata()
        {
            return MetaData;
        }
    }
}
