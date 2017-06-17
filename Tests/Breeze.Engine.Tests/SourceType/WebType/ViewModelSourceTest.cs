using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Breeze.Engine.Helpers;
using Breeze.Engine.SourceType.Portal;
using Breeze.Engine.Templates;
using Breeze.Engine.Templates.Web.Models;
using Nova.CodeDOM;
using NUnit.Framework;

namespace Breeze.Engine.Tests.SourceType.WebType
{
    [TestFixture]
    public class ViewModelSourceTest
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
        public void
            Nome_da_area_deve_ser_quarta_posicao_do_namespace_informado()
        {
            const string NOME_AREA_ESPERADO = @"\Areas\Financeiro";
            var fileInfo = GerarFileInfo();

            Assert.True(fileInfo.DirectoryName.Contains(NOME_AREA_ESPERADO));
        }

        [Test]
        public void
            Nome_do_diretorio_de_modelo_deve_ser_className_adicionado_da_palavra_ViewModels()
        {
            const string NOME_DIRETORIO_ESPERADO = @"\Models\TipoLancamentoViewModels";
            var fileInfo = GerarFileInfo();

            Assert.True(fileInfo.DirectoryName.Contains(NOME_DIRETORIO_ESPERADO));
        }

        [Test]
        public void
            Nome_da_classe_deve_ser_nome_da_entidade_mais_palavra_viewModel()
        {
            const string NOME_ARQUIVO_ESPERADO = @"TipoLancamentoViewModel.cs";
            var fileInfo = GerarFileInfo();

            Assert.True(fileInfo.Name.Equals(NOME_ARQUIVO_ESPERADO));
        }

        [Test]
        public void
            Deve_gerar_path_completo_do_diretorio_ViewModel()
        {
            const string DIRETORIO_ESPERADO = @"\Areas\Financeiro\Models\TipoLancamentoViewModels";
            var fileInfo = GerarFileInfo();
            
            var mensagemFalha = String.Format("Esperado:{0} {1}Recebido:{2}",DIRETORIO_ESPERADO, Environment.NewLine, fileInfo.DirectoryName);
            Assert.True(fileInfo.DirectoryName.Contains(DIRETORIO_ESPERADO),mensagemFalha);
        }

        [Test]
        public void
            Deve_gerar_path_completo_do_para_classe_de_ViewModel()
        {
            const string FULLNAME_ESPERADO = @"\Areas\Financeiro\Models\TipoLancamentoViewModels\TipoLancamentoViewModel.cs";
            var fileInfo = GerarFileInfo();

            var mensagemFalha = String.Format("Esperado:{0} {1}Recebido:{2}",FULLNAME_ESPERADO, Environment.NewLine, fileInfo.DirectoryName);

            Assert.True(fileInfo.FullName.Contains(FULLNAME_ESPERADO), mensagemFalha);
        }

        [Test]
        public void
            Teste_Geracao_Propriedade()
        {
            var source = new ViewModelSource(new EntityViewModelBase(metadata), new FileHelper());
            var gerado = source.Content();
            Assert.IsNotNull(gerado);
        }

        
        private FileInfo GerarFileInfo()
        {
            var source = new ViewModelSource(new Mapping(metadata), new FileHelper());
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
