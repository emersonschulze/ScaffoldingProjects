using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Breeze.Engine.Helpers;
using Breeze.Engine.SourceType;
using Breeze.Engine.SourceType.Business;
using Breeze.Engine.SourceType.Model;
using Breeze.Engine.SourceType.Portal;
using Breeze.Engine.SourceType.WebPages;
using Breeze.Engine.Templates;
using Breeze.Engine.Templates.Web.Business;
using Breeze.Engine.Templates.Web.Controllers;
using Breeze.Engine.Templates.Web.Models;
using Breeze.Engine.Templates.Web.Pages;
using Breeze.Engine.Templates.Web.Views;

namespace Breeze.Engine
{
    public class Factory
    {
        private static List<ISource> engines;

        public Factory StartEngines(MetaDataDefinition metaData)
        {
            CheckLoopkups(metaData);

            engines = new List<ISource>();

            CreateMapEngines(metaData);

            CreateViewModelEngines(metaData);

            CreateViewEngines(metaData);

            CreatePages(metaData);

            CreateRepositories(metaData);

            return this;
        }


        private static void CreateViewEngines(MetaDataDefinition metaData)
        {
            if (metaData.GenViews)
            {
                engines.Add(new ControllerSource(new Controller(metaData), new FileHelper()));
                engines.Add(new ViewSource(new IndexView(metaData),     new FileHelper(), actionName: "Index"));
				engines.Add(new ViewSource(new InserirView(metaData),   new FileHelper(), actionName: "Inserir"));
                engines.Add(new ViewSource(new EditarView(metaData),    new FileHelper(), actionName: "Editar"));
                engines.Add(new ViewSource(new DetalhesView(metaData),  new FileHelper(), actionName: "Detalhes"));
            }
        }


        private static void CreateViewModelEngines(MetaDataDefinition metaData)
        {
            if (metaData.GenViewModels)
            {
                engines.Add(new ViewModelSource(new EntityViewModelBase(metaData), new FileHelper()));
                engines.Add(new ActionViewModelSource(new IndexViewModel(metaData),new FileHelper(), actionName: "Index"));
            }
        }

        private static void CreateRepositories(MetaDataDefinition metaData)
        {
            engines.Add(new InterfaceRepositorioSource(new InterfaceRepositorio(metaData),  new FileHelper()));
            engines.Add(new RepositorioConcretoSource(new RepositorioConcreto(metaData),    new FileHelper()));
            engines.Add(new EntityAutoMapperSource(new EntityMapperMap(metaData),           new FileHelper()));
        }

        private static void CreateMapEngines(MetaDataDefinition metaData)
        {
            if (metaData.GenMapping)
                engines.Add(new MappingSource(new Mapping(metaData), new FileHelper()));
        }


        private static void CreatePages(MetaDataDefinition metaData)
        {
            if (metaData.GenPages)
            {
                engines.Add(new CreatePagesSource(new CreatePages(metaData), new FileHelper()));
                engines.Add(new CreatePagesSourceImpl(new CreatePagesImpl(metaData), new FileHelper()));
            }
                
        }


        public Dictionary<String,TypeFile> Run()
        {
            if (engines == null)
                throw new InvalidOperationException("Deve ser executado StartEngines() para iniciar o Factory!");
            
            var generatedFiles = new Dictionary<string,TypeFile>();

            foreach (var engine in engines)
            {
                var pathFile = engine.Gen();

                CheckCreatedFile(pathFile, engine, generatedFiles);
            }

            return generatedFiles;
        }


        private static void CheckCreatedFile(string pathFile, ISource engine, Dictionary<string, TypeFile> generatedFiles)
        {
            if (string.IsNullOrWhiteSpace(pathFile))
                return;

            
            if (engine is MappingSource)
                generatedFiles.Add(pathFile, TypeFile.Model);
            else if(engine is ViewSource || engine is ControllerSource)
                generatedFiles.Add(pathFile, TypeFile.Portal);
            else if(engine is CreatePagesSource)
                generatedFiles.Add(pathFile, TypeFile.WebDriverPages);
            else
                generatedFiles.Add(pathFile, TypeFile.PortalBusiness);
        }


        private void CheckLoopkups(MetaDataDefinition metaData)
        {
            LookupVerifier.CheckForeignKeyReferences(metaData.Properties);
            LookupVerifier.CheckForeignKeyReferences(metaData.MapProperties);
        }
    }
}
