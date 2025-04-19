using System.Collections.Generic;
using XSolana.Conventions;
using XSolana.Parsers.Models;

namespace XSolana.Parsers
{
    /// <summary>
    /// Parses type JSON models into type definitions.
    /// </summary>
    public static class TypeParser
    {
        /// <summary>
        /// Parses a list of type JSON models into type definitions.
        /// </summary>
        public static List<TypeDefinition> Parse(List<TypeJsonModel> source)
        {
            var result = new List<TypeDefinition>();

            foreach (var type in source)
                result.Add(Parse(type));

            return result;
        }

        /// <summary>
        /// Parses a single type JSON model into a type definition.
        /// </summary>
        public static TypeDefinition Parse(TypeJsonModel model)
        {
            var kind = model.Type.Kind;

            var def = new TypeDefinition
            {
                Name = model.Name,
                Discriminator = model.Discriminator
            };

            if (kind == "struct")
            {
                def.Struct = new StructDefinition
                {
                    Fields = FieldParser.Parse(model.Type.Fields)
                };
            }
            else if (kind == "enum")
            {
                var variants = new List<EnumVariant>();

                if (model.Type.Variants != null)
                {
                    foreach (var variant in model.Type.Variants)
                    {
                        var enumVariant = new EnumVariant
                        {
                            Name = variant.Name
                        };

                        if (variant.Fields != null)
                        {
                            enumVariant.Fields = EnumFieldParser.ParseVariantFields(variant.Fields);
                        }

                        variants.Add(enumVariant);
                    }
                }

                def.Enum = new EnumDefinition
                {
                    Variants = variants
                };
            }

            return def;
        }
    }
}
