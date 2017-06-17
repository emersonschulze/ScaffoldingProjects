namespace Breeze.Engine.Templates.Web.Models
{
    public partial class IndexViewModel : ITemplate
    {
        private readonly MetaDataDefinition MetaData;

        public IndexViewModel(MetaDataDefinition MetaData)
        {
            this.MetaData = MetaData;
        }

        public MetaDataDefinition GetMetadata()
        {
            return this.MetaData;
        }
    }
}
