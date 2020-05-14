# Reading and Writing Binary using pure C#

The .NET framework provides the System.IO namespace classes [BinaryReader](https://docs.microsoft.com/en-us/dotnet/api/system.io.binaryreader?view=netframework-4.7.2) and [BinaryWriter](https://docs.microsoft.com/en-us/dotnet/api/system.io.binarywriter?view=netframework-4.7.2) for this task. Below is an example of how one would serialize data purely in C#.

```c#
using System.IO;

public class CSharpSerialization
{
    public const string filePath = "C:\\path\\file.txt";
    // Read
    public const FileMode   readMode    = FileMode.Open;
    public const FileAccess readAccess  = FileAccess.Read;
    public const FileShare  readShare   = FileShare.Read;
    // Write
    public const FileMode   writeMode   = FileMode.Open;
    public const FileAccess writeAccess = FileAccess.Write;
    public const FileShare  writeShare  = FileShare.None;

    public bool myBool;
    public int myInt;
    public string myString;

    void Read()
    {
        using (var file = File.Open(filePath, readMode, readAccess, readShare))
        {
            using (var reader = new BinaryReader(file))
            {
                myBool      = reader.ReadBoolean();
                myInt       = reader.ReadInt32();
                myString    = reader.ReadString();
            }
        }
    }

    void Write()
    {
        using (var file = File.Open(filePath, writeMode, writeAccess, writeShare))
        {
            using (var writer = new BinaryWriter(file))
            {
                writer.Write(myBool);
                writer.Write(myInt);
                writer.Write(myString);
            }
        }
    }
}
```

Note that for each individual variable we wish to serialize, we need to explicitly read it from the stream by type, in this case, via `ReadBoolean()`, `ReadInt32()`, and `ReadString()`. It is possible to serialize a struct or class using [System.Runtime.Serialization.Formatters.Binary](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.serialization.formatters.binary?view=netframework-4.7.2) namespace class [BinaryFormatter](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.serialization.formatters.binary.binaryformatter?view=netframework-4.7.2). However, since this class automatically serializes a structure for us we lose any ability to optimize serialization size, add or remove serialized values, and prevents us from mimicking structures or classes that can deserialize data from other applications. Furthermore, there is overhead to this process and metadata generated in the binary, rendering it less ideal if we are dealing with larger file sizes.

Furthermore, there is no built-in method for serializing or deserializing arrays. To achieve this, we would need to write our own iterators and store array size.

```C#
using System.IO;
using UnityEngine;

public class ManagedType : MonoBehaviour
{
    public bool myBool;
    public int myInt;
    public string myString;
}

public class CSharpComplexSerialization
{
    public const string filePath = "C:\\path\\file.txt";
    // Read
    public const FileMode   readMode    = FileMode.Open;
    public const FileAccess readAccess  = FileAccess.Read;
    public const FileShare  readShare   = FileShare.Read;
    // Write
    public const FileMode   writeMode   = FileMode.Open;
    public const FileAccess writeAccess = FileAccess.Write;
    public const FileShare  writeShare  = FileShare.None;

    public Vector3 position;
    public ManagedType managedType;
    public int[] ints = new int[100];

    void Read()
    {
        using (var file = File.Open(filePath, readMode, readAccess, readShare))
        {
            using (var reader = new BinaryReader(file))
            {
                // Read Vector3
                position.x = reader.ReadSingle();
                position.y = reader.ReadSingle();
                position.z = reader.ReadSingle();

                // Read ManagedType
                managedType.myBool = reader.ReadBoolean();
                managedType.myInt = reader.ReadInt32();
                managedType.myString = reader.ReadString();

                // Read ints
                int count = reader.ReadInt32();
                ints = new int[count];
                for (int i = 0; i < ints.Length; i++)
                    ints[i] = reader.ReadInt32();
            }
        }
    }

    void Write()
    {
        using (var file = File.Open(filePath, writeMode, writeAccess, writeShare))
        {
            using (var writer = new BinaryWriter(file))
            {
                // Write Vector3
                writer.Write(position.x);
                writer.Write(position.y);
                writer.Write(position.z);

                // Write ManagedType
                writer.Write(managedType.myBool);
                writer.Write(managedType.myInt);
                writer.Write(managedType.myString);

                // Write ints
                writer.Write(ints.Length);
                for (int i = 0; i < ints.Length; i++)
                    writer.Write(ints[i]);
            }
        }
    }
}
```

Essentially, `System.IO` provides use with the ability to read and write, but the task of making scalable input/output code is up to the user. `StarkTools.IO` was made for this purpose; to provide a robust API for easily reading and writing binary with scalability and flexibility.

# Reading and Writing Binary using StarkTools.IO

StarkTools.IO has 5 main classes for reading and writing binary. Listed below is each class and their function.

### IBinarySerializable.cs

`IBinarySerializable` is an interface and as such can be inherited by any struct or class we wish to serialize and deserialize. Inheritance to this interface forces it to implement `Deserialize(BinaryReader)` and `Serialize(BinaryWriter)`. In practice, this means when serializing, we write the contents of the current item into the `BinaryWriter` and reciprocally read to the item from the `BinaryReader` when deserializing.

### BinaryIoUtility.cs

This class defines all of the binary reading and writing functionality for value types, arrays, and managed types. The methods in this class are all static and as such can be called through the class definition. However, aside from a few properties, this is not the purpose of this class. `ReadX` and `WriteX` methods are intended to be called via extensions to `BinaryReader` and `BinaryWriter` defined in `BinaryIoExtensions`, `BinaryIoExtensionsCSharp`, and `BinaryIoExtensionsUnity`.

BinaryIoExtensions.cs

This class is an extension suite for the .NET System.IO namespace classes [BinaryReader](https://docs.microsoft.com/en-us/dotnet/api/system.io.binaryreader?view=netframework-4.7.2) and [BinaryWriter](https://docs.microsoft.com/en-us/dotnet/api/system.io.binarywriter?view=netframework-4.7.2). In essence, it is the primary low-level API of this tool and provides simple and intuitive API to read from `BinaryReaders` and write into `BinaryWriters`.

The extensions for all value types, arrays, and managed types share the same overloaded function `ReadX(ref value)` and `WriteX(value)`.

## BinaryIoExtensionsCSharp.cs

This class is an extension suite for the .NET System.IO namespace classes [BinaryReader](https://docs.microsoft.com/en-us/dotnet/api/system.io.binaryreader?view=netframework-4.7.2) and [BinaryWriter](https://docs.microsoft.com/en-us/dotnet/api/system.io.binarywriter?view=netframework-4.7.2) which implements reading and writing functionality for common C# data types such as `DateTime`.

### BinaryIoExtensionsUnity.cs

This class is an extension suite for the .NET System.IO namespace classes [BinaryReader](https://docs.microsoft.com/en-us/dotnet/api/system.io.binaryreader?view=netframework-4.7.2) and [BinaryWriter](https://docs.microsoft.com/en-us/dotnet/api/system.io.binarywriter?view=netframework-4.7.2) which implements reading and writing functionality for common `UnityEngine` data types such as `Vector2`, `Vector3`, `Color`, `Color32`, and more.

## Implementation

With that in mind, here is how we could structure our example data do read and write effectively. Below are example data structures, followed by a class to manage reading and writing. Note that we can pass our custom-made `IBinarySerializable` struct and class into the methods `ReadX` and `WriteX` without the need of specifying their type.

```c#
using StarkTools.IO;
using System.IO;
using UnityEngine;


public class ManagedStruct : IBinarySerializable
{
    public bool myBool;
    public int myInt;
    public string myString;

    public void Deserialize(BinaryReader reader)
    {
        reader.ReadX(ref myBool);
        reader.ReadX(ref myInt);
        // We read length from disk for this array
        reader.ReadX(ref myString);
    }

    public void Serialize(BinaryWriter writer)
    {
        writer.WriteX(myBool);
        writer.WriteX(myInt);
        // For this array, we specify that, yes,
        // we do wish to write size to disk
        writer.WriteX(myString, true);
    }
}

public class ManagedClass : MonoBehaviour, IBinarySerializable
{
    public const int NumInts = 100;

    public Vector3 position;
    public ManagedStruct managedStruct;
    public int[] ints = new int[NumInts];

    public void Deserialize(BinaryReader reader)
    {
        // Note that types are inferred
        reader.ReadX(ref position);
        reader.ReadX(ref managedStruct);
        // Pass constant number of ints to read
        reader.ReadX(NumInts, ref ints);
    }

    public void Serialize(BinaryWriter writer)
    {
        writer.WriteX(position);
        writer.WriteX(managedStruct);
        // For this array, we specify that, no,
        // don't need to write length to disk 
        writer.WriteX(ints, false);
    }
}
```

To manage this data, a class can open a file and request the `IBinarySerializable` deserialize from the file stream. Inversely, we can request it serialize to the file stream. Due to the hierarchy of the interface, we can call a single method and it will “dig into” the structure and pull out the appropriate binary data.

```c#
public class BinaryHandler
{
    public const string filePath = "C:\\path\\file.txt";
    // Read
    public const FileMode   readMode    = FileMode.Open;
    public const FileAccess readAccess  = FileAccess.Read;
    public const FileShare  readShare   = FileShare.Read;
    // Write
    public const FileMode   writeMode   = FileMode.Open;
    public const FileAccess writeAccess = FileAccess.Write;
    public const FileShare  writeShare  = FileShare.None;

    public ManagedClass managedClass = new ManagedClass();

    void Read()
    {
        using (var file = File.Open(filePath, readMode, readAccess, readShare))
        {
            using (var reader = new BinaryReader(file))
            {
                reader.ReadX(ref managedClass);
            }
        }
    }

    void Write()
    {
        using (var file = File.Open(filePath, writeMode, writeAccess, writeShare))
        {
            using (var writer = new BinaryWriter(file))
            {
                writer.WriteX(managedClass);
            }
        }
    }
}
```

# Complications and Solutions

## Character and String Encodings

Characters and string can use a wide array of encodings. Understanding encodings is crucial to saving and loading strings. For instance, while ASCII uses 1 byte per character, Unicode uses 2. Because of this, ASCII can only support 256 characters (less in fact due to how each entry is used) while Unicode can support 65,535 characters. ASCII is limited to Latin characters, digits, and common symbols while Unicode can support many language characters, glyphs, and emoji on top of what ASCII can support.

`BinaryIoUtility` takes this into account when serializing and deserializing characters and strings. The static property `BinaryIoUtility._Encoding` can be modified to whichever encoding is needed. However, if not handled properly, it is possible to change encoding for the needs of one structure and not reset it for the next which could lead to catastrophic reading errors. To mitigate this, `BinaryIoUtility` has 2 methods to manage `_Encoding`, `PushEnconding(System.Text.Encoding)` and `PopEncoding()`. If you need to modify the encoding (which defaults to Unicode), push a new encoding at the start of the operation and pop it once complete. This will ensure the proper encoding is only used for the item you wish it to be used for.

## Endianness

One thing yet to be discussed is [Endianness](https://en.wikipedia.org/wiki/Endianness). “Endianness is the sequential order in which bytes are arranged into larger numerical values when stored in memory.” There are 2 types of endiannesses:

- Big Endian

- - The most significant byte is the one farthest left.

- Little Endian

- - The least significant byte is the one farthest left.

A way to think of endianness is as right-handed or left-handed bytes. The ordering of bytes significance is either left-to-right (Big Endian) or right-to-left (Little Endian).

In Mono C#, there is no way to expressly guarantee that binary be serialized in either endianness. It is possible to use the `System` class `BitConverter` to swap endianness of values types around, but there isn’t a system tied in `BinaryReader` or `BinaryWriter` to do that for us.

Complications may arise because different operating systems and programs can use different endianness. I cannot stress enough, Mono C# *cannot* guarantee any endianness.

For this reason, `BinaryIoUtility` provides an internal system for reading and writing data in a specific endianness. The static property `BinaryIoUtility._IsLittleEndian` can be modified to whichever endianness is needed. However, if not handled properly, it is possible to change endianness for the needs of one structure and not reset it for the next which could lead to catastrophic reading errors. To mitigate this, `BinaryIoUtility` has 2 methods to manage `_IsLittleEndian`, `PushEndianness(System.Text.Encoding)` and `PopEndianness()`. If you need to modify the endianness (which defaults to false), push a new bool at the start of the operation and pop it once complete. This will ensure the proper endianness is only used for the item you wish it to be used for.

## Enumerations

C# handles enumerations as special `int` types. Because of this, there is no dedicated read or write operations for enumerations within `System.IO`. However, with the introduction of NET 4.7.3 in Unity 2018.3, generic parameters of type constraint `System.Enum` are possible.

For reasons I’m uncertain, the define for `NET_4_7_3` does not exist in Unity 2018.3.6 (last checked). However, it can be manually added to the defines in:

1. Player Settings
2. Other Settings
3. Configuration
4. Script Define Symbols

Adding either define in Script Define Symbols will activate the `ReadX` and `WriteX` methods for `enum` in `BinarIoUtility` and `BinaryIoExtensions`.

To note, the methods for `enums` take an additional parameter of type `EnumCompression`. As stated before, `enum` are stored by C# as an `int`. However, it can be beneficial to compress the space needed by an `enum` to be less than 4 bytes. `EnumCompression` provides options to store the `enum` as:

| **EnumCompression** | **Bytes** | **Min Value**  | **Max Value** |
| ------------------- | --------- | -------------- | ------------- |
| int32               | 4         | -2,147,483,648 | 2,147,483,647 |
| int16               | 2         | -32,768        | 32,767        |
| int8                | 1         | -128           | 127           |
| uint16              | 2         | 0              | 65,535        |
| uint8               | 1         | 0              | 255           |

Remember that each integer type has minimum and maximum values based on its sign. Do not save an `enum` that exceeds the selected option's range. Select `int32` if you’re uncertain which to choose as it will encompass all possible values.

## Refactoring

Refactoring code is a normal part of any development process. When dealing with binary, it becomes very easy to desync read and write operations leading to dysfunctional code and binaries. For instance, with the basic System.IO classes, it is possible to save an int in binary and then accidentally load as a float. Since there is implicit conversion, the line of code breaking won’t throw and error but you’ll notice the variable being improbable values.

`ReadX` was expressly made to mitigate refactoring errors as it will implicitly solve the type it is meant to read.

## Security

Storing binary is an efficient way to store data. While it is certainly more secure than human-readable file formats like JSON, they aren’t secure by default. There are a number of ways to protect your binary files, techniques for which are discussed in **Reading and Writing SaveData**.

In principle, there are a few things that can be done

### Checksums

A checksum is an algorithm that, given a fixed input, produces and a fixed length output. A small change in input leads to a large output randomness, making it difficult to predict changes.

The class `HashedBinarySerializable` uses `System.Security.Cryptography` hash algorithms to create the checksum, but other operations could be created. 

### Encryption

Encryption is when the entirety of a file's contents are "scrambled" in a non-destructive way wherein the data can be processed to retrieve it's original form. It would be possible to essentially do another write pass taking in the written binary data as an argument and rewriting it encrypted.

## System Paths and Unity Paths

To save a file we need a `string` path to store it. Unity stores and loads files relative to the Assets directory while a computer system will store and load relative to the root of a drive. Because of this, managing files between both paths can become cumbersome. Moreover, Unity’s Mono C# runtime uses Unix path separator characters while the computer system you’re using may use Unix or Windows style separators.

To remove the hassle of converting between paths, the utility script `UnityPathUtility` was created. It offers simple API to convert System and Unity paths to one another and provides easy was to get the path you are looking for. You can find out more in the API document.