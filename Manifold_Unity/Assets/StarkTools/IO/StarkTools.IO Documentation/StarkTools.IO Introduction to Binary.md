# Introduction to Binary

In this document, “saving” and “loading” are referred more commonly  as serializing and deserializing, respectively.

Before diving into the specifics of how to use StarkTools.IO, it is important to have an understanding of how values are serialized as binary. For our purposes, we can simplify these into 3 categories:

- Value types
- Arrays
- Managed types

## Value Types

C# has a range of *value* types. Values are mostly numerical, such as integers and decimals, but also includes text characters and Booleans. A string is *not* a value type but in fact a char[]. This is an important distinction to make as we will be managing its serialization and deserialization that way. Below is a full list of C#’s value types (excluding structs).

| **Value Type**                                               | **Category**                                                 | **Bytes** | **Bits** |
| ------------------------------------------------------------ | ------------------------------------------------------------ | --------- | -------- |
| [bool](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/bool) | Boolean                                                      | 1         | 8        |
| [byte](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/byte) | Unsigned, numeric, [integral](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/integral-types-table) | 1         | 8        |
| [char](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/char) | Unsigned, numeric, [integral](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/integral-types-table) | 1 or 2    | 8 or 16  |
| [decimal](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/decimal) | Numeric, [floating-point](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/floating-point-types-table) | 16        | 128      |
| [double](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/double) | Numeric, [floating-point](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/floating-point-types-table) | 8         | 64       |
| [enum](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/enum) | Enumeration                                                  | 4         | 32       |
| [float](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/float) | Numeric, [floating-point](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/floating-point-types-table) | 4         | 32       |
| [int](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/int) | Signed, numeric, [integral](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/integral-types-table) | 4         | 32       |
| [long](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/long) | Signed, numeric, [integral](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/integral-types-table) | 8         | 64       |
| [sbyte](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/sbyte) | Signed, numeric, [integral](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/integral-types-table) | 1         | 8        |
| [short](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/short) | Signed, numeric, [integral](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/integral-types-table) | 2         | 16       |
| [uint](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/uint) | Unsigned, numeric, [integral](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/integral-types-table) | 4         | 32       |
| [ulong](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/ulong) | Unsigned, numeric, [integral](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/integral-types-table) | 8         | 64       |
| [ushort](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/ushort) | Unsigned, numeric, [integral](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/integral-types-table) | 2         | 16       |

[Table Source](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/value-types-table)

C# allocates a certain amount of memory for each value. The table above lists the sizes in both bytes and bits. Both quantities are useful to know by hand. Conversion is simple too; multiply bytes by 8 to get the number of bits and divide bits by 8 to get the number of bytes.

Using bits is more common for value types as there are optimizations that can be such as packing bits. An example within Unity is `Color32` - the 32 indicates it is 32 bits (4 bytes), 8 bits (1 byte) per component (RGBA: Red, Green, Blue, Alpha). The `Color` struct on the other hand uses a float for each component. Since a float is 32 bits (4 bytes) and there are 4 components, it therefore uses 128 bits (16 bytes). This is critical to know when inspecting binary and attempting to reduce serialization size.

## Arrays

Serializing an array is essentially the same as serializing many of the same value in sequence. However, in order to recover the length of the data it is necessary to know the size of the array prior to reading in the contents of that array.

There are 2 ways to do this:

1. **Save the size of the array to disk**. The length can be read from disk prior to reading the array contents, thus the array can be initialized to that size. An iterator can then loop over the count, each time reading in the next array values until the array is filled.
2. **Reference constant size at compile time**. The size of the array may be known at compile time if it is a constant size. The array can be initialized to the constant size, then values read in accordingly. It is possible to save some disk space by referencing a constant when reading and writing the array.

In practice it is easiest and safest to always write the length of the array prior to writing its contents. Saving disk space is an optimization and so should only be done when necessary.

## Managed Types

For the purposes of this document, Managed Types are structs and classes where we explicitly decide what we wish to serialize. The StarkTools.IO namespace provides API via the `IBinarySerializable` interface. The inheritor of `IBinarySerializable` forced to implement void `Deserialize(BinaryReader)` and `void Serialize(BinaryWriter)` methods. Their function is to describe what to read and write. 

Managed types are essentially composite objects containing multiple value types and/or arrays. They describe which values are saved to disk and in which order. Due to the structure of the API, once a managed type describes how to serialize itself, it can be nested within other managed types, creating a serialization tree that effectively solves itself once defined.