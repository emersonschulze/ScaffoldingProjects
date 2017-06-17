using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Breeze.Engine;
using Breeze.Engine.SourceType;
using Nova.CodeDOM;

namespace Breeze.Ui.ViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {
        public string TableName { get; set; }

        public DelegateCommand GenerateCommand { get; private set; }

        public Action CloseAction { get; set; }

        private List<PropertyMetadaDefinition> propertiesMetadata;
        public List<PropertyMetadaDefinition> PropertiesMetadata
        {
            get { return propertiesMetadata; }
            set
            {
                propertiesMetadata = value;
                OnPropertyChanged();
            }
        }

        private MetaDataDefinition classMetadata;
        public MetaDataDefinition ClassMetadata
        {
            get { return classMetadata; }
            set
            {
                classMetadata = value;
                OnPropertyChanged();
            }
        }

        private Project portalProject;
        private Project modelProject;
        private Project portalBusinessProject;
        private Project webPagesProject;

        private Solution portalSolution;
        private Solution integrationTestsSoluction;
        

        public MainWindowViewModel(string projectName, string entityFullName)
        {
            GenerateCommand = new DelegateCommand(GenerateCommandExecute, GenerateCanExecute);

            var fileInformation = new FileInfo(entityFullName);
            PrepareMetadatas(fileInformation, projectName);
        }

        private void PrepareMetadatas(FileInfo entityFileInfo, string projectName)
        {
            var entityName = entityFileInfo.Name.Split('.')[0];

            this.modelProject = Project.Load(projectName);

            var children = modelProject.GetAllDeclaredTypeDecls().FirstOrDefault(name => name.Name.ToLower() == entityName.ToLower());

            var properties = children.Find<PropertyDecl>();
            var namespaceEntity = children.GetNamespace();

            PropertiesMetadata = properties.Select(prop => new PropertyMetadaDefinition(prop)).ToList();
            TableName = entityName.ToUpper();
            
            DiscoverMvcProjects(projectName);

            ClassMetadata = new MetaDataDefinition(entityName, namespaceEntity.FullName, projectName, "")
            {
                ClassName = children.Name,
                ClassPath = entityFileInfo.Directory.FullName,
                Namespace = namespaceEntity.FullName,
                ProjectPath = projectName,
                PortalPath = this.portalProject.GetDirectory(),
                PortalNamespace = this.portalProject.RootNamespace,
                PortalBusinessPath = this.portalBusinessProject.GetDirectory(),
                PortalBusinessNamespace = this.portalBusinessProject.RootNamespace,
                IntegrationTestPath = this.webPagesProject.GetDirectory(),
                IntegrationTestNamespace = this.webPagesProject.RootNamespace
            };
        }

        private string DiscoverMvcProjects(string projectReference)
        {
            DiscoverSolutionPath(projectReference);
            
            this.portalProject = portalSolution.Projects.Find(proj => proj.Name.ToLower().EndsWith("mvc") || proj.Name.ToLower().EndsWith("portal"));
            this.portalBusinessProject = portalSolution.Projects.Find(proj => proj.Name.ToLower().EndsWith("mvc.business") || proj.Name.ToLower().EndsWith("portal.business"));

            this.webPagesProject = integrationTestsSoluction.Projects.Find(proj => proj.Name.ToLower().EndsWith("webpages"));
            
            return portalProject.GetDirectory();
        }

        private void DiscoverSolutionPath(string projecPath)
        {
            var info = Directory.GetParent(projecPath);

            var solutionCount = info.Parent.GetFiles("*.sln");

            if (!solutionCount.Any())
            {
                var dialog = new FolderBrowserDialog
                {
                    RootFolder = Environment.SpecialFolder.MyComputer,
                    Description = @"Selecione o diretório da Solution",
                    ShowNewFolderButton = false
                };
                dialog.ShowDialog();

                if (!string.IsNullOrEmpty(dialog.SelectedPath))
                {
                    Directory.GetFiles(dialog.SelectedPath, "*.sln").First();
                    return;
                }
            }

            this.portalSolution = Solution.Load(solutionCount.First(sol => !sol.FullName.EndsWith("IntegrationTest.sln")). FullName);
            this.integrationTestsSoluction =  Solution.Load(solutionCount.First(sol => sol.FullName.EndsWith("IntegrationTest.sln")). FullName);
        }

        private void GenerateCommandExecute(object parameter)
        {
            ClassMetadata.SetProperties(PropertiesMetadata);
            ClassMetadata.SetTable(this.TableName);

            var createdFiles = new Factory().StartEngines(ClassMetadata).Run();

            AddGenerateFilesToProject(createdFiles);
        }

        void AddFile(string file, Project.BuildActions buildAction, Project project)
        {
            if (!project.FileItems.Any(item => item.FileName == file))
            {
                project.FileItems.Add(new Project.FileItem(buildAction, file));
            }
        }

        /// <summary>
        /// Adiciona os arquivos gerados nos projetos de Portal e Model
        /// </summary>
        /// <param name="createdFiles"></param>
        private void AddGenerateFilesToProject(Dictionary<String,TypeFile> createdFiles)
        {
            createdFiles.Where(file => file.Value == TypeFile.Model).ToList().ForEach(file =>
            {
                AddFile(file.Key, Project.BuildActions.Compile, modelProject);
            });

            createdFiles.Where(file => file.Value == TypeFile.Portal).ToList().ForEach(file =>
            {
                if(file.Key.Contains("cshtml"))
                    AddFile(file.Key, Project.BuildActions.Resource, portalProject);
                else
                    AddFile(file.Key, Project.BuildActions.Compile, portalProject);
            });

            createdFiles.Where(file => file.Value == TypeFile.PortalBusiness).ToList().ForEach(file =>
            {
                AddFile(file.Key, Project.BuildActions.Compile, portalBusinessProject);
            });

            createdFiles.Where(file => file.Value == TypeFile.WebDriverPages).ToList().ForEach(file =>
            {
                AddFile(file.Key, Project.BuildActions.Compile, webPagesProject);
            });

            modelProject.Save();
            portalProject.Save();
            portalBusinessProject.Save();
            webPagesProject.Save();
            CloseAction.Invoke();
        }

        private bool GenerateCanExecute(object parameter)
        {
            return true;
        }

    }
}
