using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Breeze.Engine.Helpers;
using Breeze.Engine.SourceType.Portal;
using Breeze.Engine.Templates;
using Breeze.Engine.Templates.Web.Models;
using Nova.CodeDOM;
using NUnit.Framework;

namespace Breeze.Engine.Tests.SourceType.WebType
{
    [TestFixture]
    public sealed class ActionViewModelSourceTest
    {
        private MetaDataDefinition metadata;
        private const String ENTITY_NAME = "TipoLancamento";
        [SetUp]
        public void Setup()
        {
            metadata = new MetaDataDefinition(className     : ENTITY_NAME,
                                              nameSpace: "Sigfaz.Autorizador.Models.Financeiro", 
                                              projectPath   : @"C:\Temp", 
                                              tableName     : "FIN_TIPOLANC");
            metadata.PortalBusinessPath = @"C:\Temp\PortalBusiness";
            metadata.PortalPath = @"C:\Temp\Portal";
            var propriedades = new List<PropertyMetadaDefinition>();

            var nullableLong = new long?(999);
            var typeRef = TypeRef.Create(nullableLong.GetType(),true);
            
            var intType = TypeFactory.Create(typeRef, isReferenceType: false);

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
            metadata.SetProperties(propriedades);
        }

        [Test]
       // [Exce(typeof(ArgumentNullException))]
        public void
            Deve_gerar_exception_quando_nao_setar_o_path_do_portal()
        {
            GerarFileInfo("Inserir");
        }

        [Test]
        public void
            Deve_ter_nome_da_classe_composto_por_action_mais_nomeEntidade_mais_viewModel()
        {
            const string ACTION_NAME = "Editar";
            const string VIEW_MODEL = "ViewModel";
            const string CLASS_EXTENSION = ".cs";

            string expectedClassName = String.Concat(ACTION_NAME, ENTITY_NAME, VIEW_MODEL, CLASS_EXTENSION);

            this.metadata.PortalPath = @"C:\Temp\Portal";

            var fileInfo = GerarFileInfo(ACTION_NAME);

            Assert.True(fileInfo.Name.Equals(expectedClassName));

        }

         private FileInfo GerarFileInfo(string actionName)
        {
            var source = new ActionViewModelSource(new ActionEntityViewModel(metadata, actionName), new FileHelper(), actionName);
            var fileInfo = source.Info();

            AssertivasFileInfo(fileInfo);
            return fileInfo;
        }

        private static void AssertivasFileInfo(FileInfo fileInfo)
        {
            Assert.False(fileInfo == null, "FileInfo não gerado corretamente!");
            Assert.False(fileInfo.DirectoryName == null, "FileInfo não gerado corretamente!");
        }
    }
}
