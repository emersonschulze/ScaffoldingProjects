using NUnit.Framework;


namespace Breeze.Engine.Tests
{
    [TestFixture]
    public class FactoryEngineTest
    {
        const string CLASS_NAME = "ClassTeste";
        const string NAME_SPACE = "Classe.Namespace";
        const string PROJECT_PATH = "C:\temp";
        const string TABLE_NAME = "CLASSTESTE";

        private MetaDataDefinition metadata;

        [SetUp]
        public void SetupData()
        {
            this.metadata = new MetaDataDefinition(CLASS_NAME, NAME_SPACE, PROJECT_PATH, TABLE_NAME);
        }

        [Test]
        public void Green_Field()
        {
            Assert.True(true);
        }
    }
}
