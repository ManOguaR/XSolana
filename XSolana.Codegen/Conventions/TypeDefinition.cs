using System;

namespace XSolana.Conventions
{
    /// <summary>
    /// Represents the kind of type definition, either a struct or an enum.
    /// </summary>
    public enum TypeKind 
    {
        /// <summary>
        /// A structure type definition.
        /// </summary>
        Struct,
        /// <summary>
        /// Enum type definition.
        /// </summary>
        Enum
    }

    /// <summary>
    /// Represents a type definition in a Solana account or instruction.
    /// </summary>
    public class TypeDefinition
    {
        /// <summary>
        /// The name of the type.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The type of the definition, which can be either a struct or an enum.
        /// </summary>
        public TypeKind Kind { get; set; }

        /// <summary>
        /// The type of the definition, which can be either a struct or an enum.
        /// </summary>
        public StructDefinition Struct { get; set; }

        /// <summary>
        /// The type of the definition, which can be either a struct or an enum.
        /// </summary>
        public EnumDefinition Enum { get; set; }

        /// <summary>
        /// Gets the body of the type definition, which can be either a struct or an enum.
        /// </summary>
        /// <returns>
        /// The body of the type definition, which can be either a struct or an enum.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Arises when the <see cref="Kind"/> is not a valid type kind.
        /// </exception>
        public object GetBody()
        {
            switch (Kind)
            {
                case TypeKind.Struct:
                    return Struct;
                case TypeKind.Enum:
                    return Enum;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Kind), $"Unknown type kind: {Kind}");
            }
        }
    }
}
