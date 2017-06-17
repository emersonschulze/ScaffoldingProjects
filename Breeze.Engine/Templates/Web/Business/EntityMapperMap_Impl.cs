using System.Linq;

namespace Breeze.Engine.Templates.Web.Business
{
    public partial class EntityMapperMap : ITemplate
    {
        private readonly MetaDataDefinition MetaData;

        public EntityMapperMap(MetaDataDefinition metadata)
        {
            this.MetaData = metadata;
        }
        
        public void IgnoreViewModelReferenceTypes()
        {
            this.PushIndent("\t");
            foreach(var property in this.MetaData.Properties.Where(prop => prop.IsReferred))
            {
                this.Write(".ForMember(c => c.Descricao{0}, opt => opt.Ignore())",property.Name);
            }
            this.PopIndent();
        }


        public void IgnoreMapReferenceTypes()
        {
            this.PushIndent("\t");
            foreach(var property in this.MetaData.Properties.Where(prop => prop.IsReferenceType))
            {
                this.Write(".ForMember(c => c.{0}, opt => opt.Ignore())",property.Name.Replace("Handle",""));
            }
            this.PopIndent();
        }

        public MetaDataDefinition GetMetadata()
        {
            return this.MetaData;
        }
    }
}
