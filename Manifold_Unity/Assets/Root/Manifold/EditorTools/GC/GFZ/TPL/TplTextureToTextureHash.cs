using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.TPL
{
    /// <summary>
    /// A scriptable object which stores a dictionary that maps TPL files by name to
    /// a list of texture hashes by index. Use this type to request TPL texture hashes
    /// for a given file and index into the returned array to get texture[index]'s hash.
    /// </summary>
    public class TplTextureToTextureHash : ScriptableObject
    {
        /// <summary>
        /// Name of files whose texture hashes are recorded in the dictionary.
        /// </summary>
        [field: SerializeField] public string[] FileNames { get; internal set; }
        /// <summary>
        /// Arrays of texture hashes, one for each file, contains multiple hashes sorted by index.
        /// </summary>
        /// <remarks>
        /// To be consistent with GFZ TPLs, can have null entries. An entry that is null will return
        /// a null or empty string.
        /// </remarks>
        [field: SerializeField] public TplTextureHashes[] Hashes { get; internal set; }

        public Dictionary<string, TplTextureHashes> GetDictionary()
        {
            var dictionary = new Dictionary<string, TplTextureHashes>();
            dictionary.AddArraysAsKeyValuePairs(FileNames, Hashes);
            return dictionary;
        }
    }
}
