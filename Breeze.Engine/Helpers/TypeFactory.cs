using System;
using Nova.CodeDOM;

namespace Breeze.Engine.Helpers
{
    internal sealed class TypeFactory
    {
        public static TypeBase Create(Expression type, bool isReferenceType)
        {
            TypeBase breezeType = null;
            
            breezeType= GetTypeFromValue(type as TypeRef);

            if(isReferenceType)
                breezeType = GetTypeFromReference(type as TypeRef);

            if (breezeType == null && type is UnresolvedRef)
                breezeType = GetTypeFromComplex(type as UnresolvedRef);


            return breezeType;
        }
        
        /// <summary>
        /// Cria um tipo Referencia, no final irá virar um longType, porém com annotations de lookup.
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        private static TypeBase GetTypeFromReference(TypeRef reference)
        {
            return reference.AsString().ToLower() == "long" ? ReferenceType.Create() : ReferenceType.CreateNullable();
        }

        /// <summary>
        /// O Tipo Complexo não resolvido, normalmente é um string descrição do lookup.
        /// </summary>
        /// <param name="complex"></param>
        /// <returns></returns>
        private static TypeBase GetTypeFromComplex(UnresolvedRef complex)
        {
            return complex != null ? StringType.Create() : null;
        }

        /// <summary>
        /// Tipos básicos não possuem transformações especiais.
        /// </summary>
        /// <param name="valueTypes"></param>
        /// <returns></returns>
        private static TypeBase GetTypeFromValue(TypeRef valueTypes)
        {
            if (valueTypes == null)
                return null;

            switch (valueTypes.AsString().ToLower())
            {
                case "byte[]":
                case "string":
                    return StringType.Create();
                case "int":
                    return IntegerType.Create();
                case "int?":
                    return IntegerType.CreateNullable();
                case "datetime":
                    return DateType.Create();
                case "datetime?":
                    return DateType.CreateNullable();
                case "long":
                    return LongType.Create();
                case "long?":
                    return LongType.CreateNullable();
                case "bool":
                    return BoolType.Create();
                case "bool?":
                    return BoolType.CreateNullable();
                default:
                    throw new ArgumentOutOfRangeException("valueTypes", "Não foi possível determinar o tipo da propriedade!");
            }
        }
    }
}
