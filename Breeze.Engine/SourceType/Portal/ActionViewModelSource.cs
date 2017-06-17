using System;
using System.IO;
using System.Linq;
using Breeze.Engine.Helpers;
using Breeze.Engine.Templates;

namespace Breeze.Engine.SourceType.Portal
{
    internal sealed class ActionViewModelSource : SourceBase
    {
        private readonly string actionName;

        public ActionViewModelSource(ITemplate template, IFileHelper fileHelper, String actionName) : base(template, fileHelper)
        {
            this.actionName = actionName;
        }

        /// <summary>
        /// O Nome da área é determinado pelo namespace. 
        /// Maior 3 => Pego a 4 posição do namespace, ex: Sigfaz.Autorizador.Models.Financeiro. Area = Financeiro
        /// Menos 3 => Obtém a última posição do namespace, ex: Sigfaz.Autorizador.Financeiro
        /// </summary>
        /// <returns></returns>
        protected override string GetDirectory()
        {
            var portalBusinessPath = Template.GetMetadata().PortalBusinessPath;

            if (string.IsNullOrWhiteSpace(portalBusinessPath))
                throw new ArgumentNullException("PortalBusinessPath", "Não encontrado a propriedade PortalBusinessPath");

            var area = Template.GetAreaName();

            var fileDirectory = (!String.IsNullOrWhiteSpace(area)) ?
                Path.Combine(portalBusinessPath, area, Template.GetMetadata().ClassName, "ViewModels") :
                Path.Combine(portalBusinessPath, Template.GetMetadata().ClassName, "ViewModels");

            return fileDirectory;
        }

        public override bool OverrideFile
        {
            get { return true; }
        }

        protected override string GetFileName()
        {
            return string.Concat(actionName, Template.GetMetadata().ClassName, "ViewModel.cs");
        }
    }
}
