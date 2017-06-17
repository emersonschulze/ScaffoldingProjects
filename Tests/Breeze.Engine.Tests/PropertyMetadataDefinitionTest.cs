using NUnit.Framework;

namespace Breeze.Engine.Tests
{
    [TestFixture]
    public class PropertyMetadataDefinitionTest
    {
        [Test]
        public void
            Deve_Possuir_DataBaseColumnName_IGual_PropertyName_UpperCase_Para_Ate_30_Caracteres()
        {
            string propertyName = "Descricao";
            string expectedTableName = propertyName.ToUpper();

            var intType = IntegerType.CreateNullable();
            var metaData = new PropertyMetadaDefinition(propertyName, true, intType);

            Assert.AreEqual(expectedTableName, metaData.DataBaseColumnName);
        }

        [Test]
        public void
            Deve_Ter_DatabaseColumnName_Com_Primeiros_30_Caracteres_De_PropertyName()
        {
            string propertyName = "DescricaoDeUmaPropriedadeCOmMaisDeTrintaCaracteres";
            string expectedTableName = propertyName.Substring(0, 30).ToUpper();

            var intType = IntegerType.CreateNullable();
            var metaData = new PropertyMetadaDefinition(propertyName, true, intType);

            Assert.AreEqual(expectedTableName, metaData.DataBaseColumnName);
        }

    }
}
