using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Breeze.Engine.SourceType;

namespace Breeze.Engine.Templates.Web.Views
{
    public partial class IndexView : ITemplate
    {
        protected readonly MetaDataDefinition MetaData;

        public IndexView(MetaDataDefinition metadata)
        {
            this.MetaData = metadata;
        }

        public MetaDataDefinition GetMetadata()
        {
            return this.MetaData;
        }
    }
}
