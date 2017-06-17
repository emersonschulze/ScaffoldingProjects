namespace Breeze.Engine.Templates.Web.Views
{
    public partial class InserirView : ITemplate
    {
        private MetaDataDefinition MetaData;

        public InserirView(MetaDataDefinition Metadata)
        {
            this.MetaData = Metadata;
        }

        public MetaDataDefinition GetMetadata()
        {
            return MetaData;
        }
    }
}
