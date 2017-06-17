using System;

namespace Breeze.Engine.Templates.Web.Models
{
    public partial class EntityViewModelBase : ITemplate
    {
        private readonly MetaDataDefinition MetaData;

        public EntityViewModelBase(MetaDataDefinition MetaData)
        {
            this.MetaData = MetaData;
        }

        public MetaDataDefinition GetMetadata()
        {
            return this.MetaData;
        }
    }
}
