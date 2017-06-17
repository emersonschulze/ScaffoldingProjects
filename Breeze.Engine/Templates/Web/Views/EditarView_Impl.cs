namespace Breeze.Engine.Templates.Web.Views
{
    public partial class EditarView : ITemplate
    {
        protected MetaDataDefinition MetaData;

        public EditarView(MetaDataDefinition MetaData)
        {
            this.MetaData = MetaData;
        }

        public MetaDataDefinition GetMetadata()
        {
            return this.MetaData;
        }
    }
}
