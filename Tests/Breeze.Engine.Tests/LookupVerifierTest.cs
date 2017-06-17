using System.Collections.Generic;
using Breeze.Engine.Helpers;
using Nova.CodeDOM;
using NUnit.Framework;

namespace Breeze.Engine.Tests
{
    [TestFixture]
    public class LookupVerifierTest
    {
        [Test]
        public void
            Deve_Encontrar_Propriedade_Reference_Na_Lista()
        {
            var propriedades = new List<PropertyMetadaDefinition>();

            var intType = IntegerType.Create();

            var item1 = new PropertyMetadaDefinition("HandleProp1", true, intType);
            var item2 = new PropertyMetadaDefinition("Prop1", false, intType);
            var item3 = new PropertyMetadaDefinition("Prop3", true, intType);
            var item4 = new PropertyMetadaDefinition("HandleProp4", true, intType);
            var item5 = new PropertyMetadaDefinition("Prop4", false, intType);
            var item6 = new PropertyMetadaDefinition("Prop5", true, intType);
            propriedades.Add(item1);
            propriedades.Add(item2);
            propriedades.Add(item3);
            propriedades.Add(item4);
            propriedades.Add(item5);
            propriedades.Add(item6);

            LookupVerifier.CheckForeignKeyReferences(propriedades.AsReadOnly());
            
            Assert.True(true);

        }
    }
}
