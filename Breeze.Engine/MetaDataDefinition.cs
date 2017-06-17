using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Breeze.Engine
{
    public class MetaDataDefinition
    {
        public string ClassName { get; set; }
        public string ClassPath { get; set; }
        public string TableName { get; private set; }
        public uint SequenceHash { get; private set; }
        public string Namespace { get; set; }
        public string PortalNamespace { get; set; }
        public string PortalBusinessNamespace { get; set; }

        public string IntegrationTestPath { get; set; }
        public string IntegrationTestNamespace { get; set; }

        public string ProjectPath { get; set; }
        public string PortalPath { get; set; }
        public string PortalBusinessPath { get; set; }

        public bool GenMapping { get; set; }
        public bool GenViewModels { get; set; }
        public bool GenViews { get; set; }
        public bool GenPages { get; set; }

        public IQueryable<PropertyMetadaDefinition> Properties { get; private set; }
        public IEnumerable<PropertyMetadaDefinition> MapProperties { get { return Properties.Where(prop => !prop.IsReferred); } }
        


        public MetaDataDefinition(string className, string nameSpace, string projectPath, string tableName)
        {
            ClassName = className;
            Namespace = nameSpace;
            ProjectPath = projectPath;
            TableName = String.IsNullOrWhiteSpace(tableName) ? className.ToUpper() : tableName.ToUpper();
            this.GenMapping = true;
            this.GenViewModels = true;
            this.GenViews = true;
            this.GenPages = true;
            ComputeHash();
        }

        public void SetProperties(IEnumerable<PropertyMetadaDefinition> properties)
        {
            Properties = new EnumerableQuery<PropertyMetadaDefinition>(properties);
        }

        public void SetTable(string tableName)
        {
            this.TableName = tableName;
            ComputeHash();
        }



        private void ComputeHash()
        {
            SequenceHash = Crc32Utils.CrcUtils.CRC32String(TableName);
        }
    }
}
