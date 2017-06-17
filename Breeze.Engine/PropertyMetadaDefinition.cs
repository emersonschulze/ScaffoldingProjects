using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Breeze.Engine.Annotations;
using Breeze.Engine.Helpers;
using Nova.CodeDOM;

namespace Breeze.Engine
{
    public class PropertyMetadaDefinition : INotifyPropertyChanged
    {
        private const string START_FK_ID = "HANDLE";

        public TypeBase Type { get; set; }
        public string Name { get; set; }
        public string DataBaseColumnName { get; set; }
        public string IndexName { get; set; }

        private bool isreference;
        public bool IsReferenceType
        {
            get
            {
                return isreference;
            }
            set
            {
                isreference = value;
                OnPropertyChanged();
            }
        }

        public string ReferenceName { get; set; }
        public bool IsReferred { get; set; }

        private bool fk;
        public bool HasFk
        {
            get
            {
                return fk;
            }
            set
            {
                fk = value;
                OnPropertyChanged();
            }

        }

        public PropertyMetadaDefinition() { }

        private Visibility enable;
        public Visibility Enable
        {
            get { return enable; }
            set
            {
                if (enable == value)
                    return;

                enable = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Criado para facilitar na escrita de testes unitários.
        /// Meio "nhé", mas é pra evitar criar um tipo <see cref="PropertyDecl"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isReferenceType"></param>
        internal PropertyMetadaDefinition(string name, bool isReferenceType, TypeBase baseType)
        {
            this.Name = name;
            this.ReferenceName = string.Empty;
            this.DataBaseColumnName = (Name.Length > 30) ? Name.Substring(0, 30).ToUpper() : Name.ToUpper();
            this.IsReferenceType = isReferenceType;
            this.Enable = IsReferenceType ? Visibility.Visible : Visibility.Collapsed;
            this.IndexName = (IsReferenceType) ? Name.ToUpper() : String.Empty;
            this.HasFk = (IsReferenceType);
            this.Type = baseType;
        }

        public PropertyMetadaDefinition(PropertyDecl propertyCodeDOM)
        {
            this.Name = propertyCodeDOM.Name;
            this.ReferenceName = string.Empty;
            this.DataBaseColumnName = (Name.Length > 30) ? Name.Substring(0, 30).ToUpper() : Name.ToUpper();
            this.IsReferenceType = propertyCodeDOM.Name.ToUpper().StartsWith(START_FK_ID);
            this.Enable = IsReferenceType ? Visibility.Visible : Visibility.Collapsed;
            this.IndexName = (IsReferenceType) ? Name.ToUpper() : String.Empty;
            this.HasFk = (IsReferenceType);
            
            this.Type = TypeFactory.Create(propertyCodeDOM.Type, IsReferenceType);
        }

        public string GetAnnotations()
        {
            this.Type.SetTypeName(this.ReferenceName);
            return this.Type.GetAnnotations();
        }

        public override string ToString()
        {
            const string DECLARE = "public {0} {1} {{ get; set; }} \r\n";
            var normalDeclare = string.Format(DECLARE, this.Type ,this.Name);

            if (this.IsReferred)
            {
                var referenceName = String.Concat("Descricao",this.Name);
                var reference = string.Format(DECLARE, "string" ,referenceName);
                return reference;
            }
                
            return normalDeclare;

        }

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}