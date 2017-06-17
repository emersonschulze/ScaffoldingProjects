using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Breeze.Engine.Helpers
{
    public class LookupVerifier
    {
        public static void CheckForeignKeyReferences(IEnumerable<PropertyMetadaDefinition> properties)
        {
            var referenceTypes = properties.Where(prop => prop.IsReferenceType);

            foreach (var reference in referenceTypes.Where(prop => !prop.IsReferred))
            {
                var possibleName = Regex.Replace(reference.Name, @"Handle", string.Empty);
                var referredType = properties.FirstOrDefault(prop => prop.Name == possibleName);
                
                if (referredType == null) continue;
                
                referredType.IsReferred = true;
                reference.ReferenceName = referredType.Name;
            }
        }
    }
}
