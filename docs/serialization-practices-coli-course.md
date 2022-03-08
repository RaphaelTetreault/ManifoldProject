# Serialization Practices (COLI_COURSE)

Generalized advice:

* Write comments about structure ONLY in the parent structure.
  * Rationale: let's assume a structure we want to serialize called `struct`. If we serialize it with a comment from within the structure itself, it will be `comment` +`struct` when serialized. We can still get a pointer which skips the `comment` and points to `struct`. However, _many_ of the structures in the files are `struct[]`. The issue then is serialization will go `comment`+`struct`,`comment`+`struct`, ... n times. This is problematic since the code has a single array pointer which points to the first item in the collection _only_. Thus, if a comment is serialized from within `struct`, then it has no concept of whether or not it is in an array and the comments will get picked up be the pointer since it will do `pointer.address + sizeof(struct) * index` when indexing into the array.
  * Another rationale: to differentiate between some instances, a hash i used. The way it works is the stream is 
* When overwriting a structure to updates pointers, always call `writer.SeekEnd()` at the end of `Serialize(writer)`
  * Rationale: subsequent calls to write in the stream will very likely overwrite the structures which came after it.



## Deserialization

Type deserialize subtypes, break object references.

## Serialization

Type does _not_ serialize subtypes to maintain references. Root structure handles serializing substructures, pointers are rewritten to maintain references regardless of serialization order.

