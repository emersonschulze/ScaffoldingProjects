using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Breeze.Engine.Helpers;
using Breeze.Engine.SourceType.Portal;
using Breeze.Engine.Templates.Web.Models;
using Breeze.Engine.Templates.Web.Views;
using Nova.CodeDOM;
using NUnit.Framework;

namespace Breeze.Engine.Tests.SourceType.WebType
{
    [TestFixture]
    public sealed class ViewSourceTest
    {
        MetaDataDefinition metadata;
        [SetUp]
        public void SetupMetadata()
        {
            metadata = new MetaDataDefinition("TipoLancamento", "Sigfaz.Autorizador.Models.Financeiro", @"C:\Temp", "FIN_TIPOLANC");
            metadata.PortalPath = @"C:\Temp";
            var propriedades = new List<PropertyMetadaDefinition>();

            Nullable<long> nullableLong = new long?(999);
            var typeRef = TypeRef.Create(nullableLong.GetType(),true);
            
            var intType = TypeFactory.Create(typeRef, isReferenceType: false);

            var item1 = new PropertyMetadaDefinition("HandleProp1", true, intType);
            var item2 = new PropertyMetadaDefinition("Prop1", false, intType);
            var item3 = new PropertyMetadaDefinition("Prop3", false, intType);
            var item4 = new PropertyMetadaDefinition("HandleProp4", true, intType);
            var item5 = new PropertyMetadaDefinition("HandleProp1", false, intType);
            var item6 = new PropertyMetadaDefinition("HandleProp2", true, intType);
            propriedades.Add(item1);
            propriedades.Add(item2);
            propriedades.Add(item3);
            propriedades.Add(item4);
            propriedades.Add(item5);
            propriedades.Add(item6);
            metadata.SetProperties(propriedades);
            LookupVerifier.CheckForeignKeyReferences(propriedades);
        }
        /// <summary>
        /// 
        /// </summary>
        [Test]
        public void
            Gerar_conteudo_basico()
        {
            var source = new ViewSource(new EditarView(metadata), new FileHelper(), "Editar");
            var gerado = source.Content();
            var infro = source.Info();
            Assert.IsNotNull(gerado);
        }
    }
}
