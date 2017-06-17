using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using Breeze.Engine.Templates;

namespace Breeze.Engine.Helpers
{
    internal static class TemplateExtensions
    {
        public static void PrintProperty(this ITemplate template,
                                         PropertyMetadaDefinition propriedade,
                                         MetaDataDefinition metadata)
        {

            template.WriteLine(propriedade.GetAnnotations());
            template.WriteLine(propriedade.ToString());
        }

        public static void PrintDetailsViewElements(this ITemplate template, IEnumerable<PropertyMetadaDefinition> metadata)
        {
            PrintElement(template,metadata, IsReadonly:true);
        }

        public static void PrintEditViewElements(this ITemplate template, IEnumerable<PropertyMetadaDefinition> metadata)
        {
            PrintElement(template,metadata, IsReadonly: false);
        }

        public static void PrintGridColumns(this ITemplate template, IEnumerable<PropertyMetadaDefinition> metadata)
        {
            template.PushIndent("\t");
            foreach (var property in metadata.Take(5))
            {
                template.WriteLine(".AddColumn(m => m.{0})",property.Name);    
            }
            template.PopIndent();
        }

        private static void PrintElement(ITemplate template, IEnumerable<PropertyMetadaDefinition> metadata, bool IsReadonly)
        {
            string htmlComponent = (IsReadonly) ? "@Html.DisplayFieldFor" : "@Html.EditorFieldFor";
            
            var breakLine = 1;
            template.PushIndent("\t");
            foreach (var property in metadata)
            {
                template.WriteLine("{0}(m => m.{1})", htmlComponent, property.Name);
                if (breakLine == 6)
                {
                    template.WriteLine("@Html.NewLine()");
                    breakLine = 0;
                    continue;
                }
                breakLine++;
            }
            template.PopIndent();
        }

        public static string GetAreaName(this ITemplate template)
        {
            var tokens = template.GetMetadata().Namespace.Split('.');
            var area = (tokens.Length > 3) ? tokens.Skip(3).ElementAt(0) : tokens.Last();
            return area;
        }

    }
}
