using System.IO;
using Breeze.Engine.Helpers;
using Breeze.Engine.Templates;

namespace Breeze.Engine.SourceType
{
    public abstract class SourceBase : ISource
    {
        protected readonly ITemplate Template;
        protected readonly IFileHelper FileHelper;

        protected SourceBase(ITemplate template, IFileHelper fileHelper)
        {
            this.Template = template;
            this.FileHelper = fileHelper;
        }

        public FileInfo Info()
        {
            string fileFullName = Path.Combine(GetDirectory(), GetFileName());
            return new FileInfo(fileFullName);
        }

        public string Content()
        {
            return Template.TransformText();
        }

        public string Gen()
        {
            return FileHelper.GenerateFile(this);
        }

        public virtual bool OverrideFile { get; private set; }

        protected abstract string GetDirectory();
        protected abstract string GetFileName();
    }
}