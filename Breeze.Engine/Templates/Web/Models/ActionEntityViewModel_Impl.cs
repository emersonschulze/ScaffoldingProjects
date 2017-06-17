using System;

namespace Breeze.Engine.Templates.Web.Models
{
    public partial class ActionEntityViewModel : ITemplate
    {
        private readonly MetaDataDefinition Metadata;
        protected readonly string actionName;

        public ActionEntityViewModel(MetaDataDefinition Metadata, String actionName)
        {
            this.Metadata = Metadata;
            this.actionName = actionName;
        }

        public MetaDataDefinition GetMetadata()
        {
            return this.Metadata;
        }
    }
}
