using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breeze.Engine.Templates.Web.Controllers
{
    public partial class Controller : ITemplate
    {
        public Controller(MetaDataDefinition Metadata)
        {
            this.Metadata = Metadata;
        }

        public MetaDataDefinition Metadata { get; set; }

        public MetaDataDefinition GetMetadata()
        {
            return this.Metadata;
        }
    }
}
