namespace Breeze.Engine.Templates.Web.Views
{
    public partial class DetalhesView : ITemplate
    {
        protected MetaDataDefinition MetaData;

        public DetalhesView(MetaDataDefinition MetaData)
        {
            this.MetaData = MetaData;
        }

        public MetaDataDefinition GetMetadata()
        {
            return this.MetaData;
        }
    }
}
