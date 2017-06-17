using System;

namespace Breeze.Engine
{
    public abstract class TypeBase
    {
        private const string ANNOTATION_DISPLAY = @"[Display(Name = """", Description = """")]";
        private const string ANNOTATION_REQUIRED = "[Required]";
        protected string TypeName;

        public Boolean IsNullable { get; protected set; }
        public abstract string GetDescriptionType();

        public string GetAnnotations()
        {
            string annotations;
        
            if (IsNullable)
                annotations = String.Format("{0}{1}{2}{3}", ANNOTATION_REQUIRED, Environment.NewLine,
                                                            ANNOTATION_DISPLAY,  CustomAnnotation());
            else
                annotations = String.Format("{0}{1}", ANNOTATION_DISPLAY, CustomAnnotation());


            return annotations;
        }

        public void SetTypeName(string typeName)
        {
            this.TypeName = typeName;
        }

        protected virtual string CustomAnnotation()
        {
            return string.Empty;
        }

        public override string ToString()
        {
            return GetDescriptionType();
        }
    }

    internal sealed class DateType : TypeBase
    {
        private DateType(bool isNullable)
        {
            this.IsNullable = isNullable;
        }

        public static DateType Create() { return new DateType(false); }
        public static DateType CreateNullable() { return new DateType(true); }


        public override string GetDescriptionType()
        {
            return (IsNullable) ? "DateTime?" : "DateTime";
        }
    }

    internal sealed class IntegerType : TypeBase
    {
        private IntegerType(bool isNullable)
        {
            this.IsNullable = isNullable;
        }

        public static IntegerType Create() { return new IntegerType(false); }
        public static IntegerType CreateNullable() { return new IntegerType(true); }


        public override string GetDescriptionType()
        {
            return (IsNullable) ? "int?" : "int";
        }
    }

    internal sealed class LongType : TypeBase
    {
        private LongType(bool isNullable)
        {
            this.IsNullable = isNullable;
        }

        public static LongType Create() { return new LongType(false); }
        public static LongType CreateNullable() { return new LongType(true); }


        public override string GetDescriptionType()
        {
            return (IsNullable) ? "long?" : "long";
        }
    }

    internal sealed class StringType : TypeBase
    {
        private StringType(bool isNullable)
        {
            this.IsNullable = isNullable;
        }

        public static StringType Create() { return new StringType(false); }
        public static StringType CreateNullable() { return new StringType(true); }


        public override string GetDescriptionType()
        {
            return "String";
        }
    }

    internal sealed class ReferenceType : TypeBase
    {
        private const string ANNOTATION_LOOKUP = @"[Lookup(TipoComplexo=typeof({0}),Campos="""", CamposDescricao ="""")]";
        private ReferenceType(bool isNullable)
        {
            this.IsNullable = isNullable;
        }

        public static ReferenceType Create()         {  return new ReferenceType(false);    }
        public static ReferenceType CreateNullable() {  return new ReferenceType(true);     }

        protected override string CustomAnnotation()
        {
            var annotations = String.Format("{0}{1}", Environment.NewLine,String.Format(ANNOTATION_LOOKUP,this.TypeName));

            return annotations;
        }

        public override string GetDescriptionType()
        {
            return (IsNullable) ? "long?" : "long";
        }
    }

    internal sealed class BoolType : TypeBase
    {
        private BoolType(bool isNullable)
        {
            this.IsNullable = isNullable;
        }

        public static BoolType Create() { return new BoolType(false); }
        public static BoolType CreateNullable() { return new BoolType(true); }


        public override string GetDescriptionType()
        {
            return (IsNullable) ? "bool?" : "bool";
        }
    }

}