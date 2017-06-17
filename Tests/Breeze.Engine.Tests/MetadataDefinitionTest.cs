using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Breeze.Engine.Tests
{
    [TestFixture]
    public class MetadataDefinitionTest
    {
        private MetaDataDefinition CriarMetadata(string name, string namespacea, string path)
        {
            return new MetaDataDefinition(name,namespacea,path,string.Empty);
        }

        [Test]
        public void Deve_Calcular_SequenceName_Baseado_No_Nome_Da_Tabela()
        {
            string ClassName = "EntidadeTeste";
            string TableName = "ENTIDADETESTE";
            string NameSpace = "Breeze.Engine.Tests.Entities";
            string ProjectPath = @"C:\Temp\Breeze";

            uint hashExpected = Crc32Utils.CrcUtils.CRC32String(TableName);

            var metaData = CriarMetadata(ClassName, NameSpace, ProjectPath);

            Assert.AreEqual(hashExpected,metaData.SequenceHash);
        }

        

    }
}
