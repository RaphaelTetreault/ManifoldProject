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
        /// Number of elements in the dictionary. Represents the number of TPL files stored in
        /// this scriptable object.
        /// </summary>
        [field: SerializeField, ReadOnlyGUI] public int TplFileCount { get; private set; }
        /// <summary>
        /// Name of files whose texture hashes are recorded in the dictionary.
        /// </summary>
        [field: SerializeField, ReadOnlyGUI] public string[] FileNames { get; internal set; }
        /// <summary>
        /// Arrays of texture hashes, one for each file, contains multiple hashes sorted by index.
        /// </summary>
        /// <remarks>
        /// To be consistent with GFZ TPLs, can have null entries. An entry that is null will return
        /// a null or empty string.
        /// </remarks>
        [field: SerializeField, ReadOnlyGUI] public TplTextureHashes[] Hashes { get; internal set; }

        public Dictionary<string, TplTextureHashes> ReconstructDictionary()
        {
            var dictionary = new Dictionary<string, TplTextureHashes>();
            dictionary.AddArraysAsKeyValuePairs(FileNames, Hashes);
            TplFileCount = dictionary.Count;
            return dictionary;
        }

        private Dictionary<string, TplTextureHashes> dictionary;
        public Dictionary<string, TplTextureHashes> GetDictionary()
        {
            if (dictionary == null)
            {
                dictionary = ReconstructDictionary();
            }
            return dictionary;
        }
    }
}
