using System;
using System.IO;
using System.Linq;
using Breeze.Engine.Helpers;
using Breeze.Engine.Templates;

namespace Breeze.Engine.SourceType.Business
{
    internal sealed class RepositorioConcretoSource : InterfaceRepositorioSource
    {
        public RepositorioConcretoSource(ITemplate template, IFileHelper fileHelper) : base(template, fileHelper)
        {
        }

        protected override string GetFileName()
        {
            return string.Concat("Repositorio", Template.GetMetadata().ClassName,".cs");
        }
    }
}
