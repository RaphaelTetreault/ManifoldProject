using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.TPL
{
    /// <summary>
    /// A scriptable object which stores a dcitionary that maps texture hashes to a relevant
    /// TPL file that stores an instance of that texture. The returned type can be used to
    /// copy the texture data from an existing file into a new file with the appropriate
    /// metadata for a TextureDescription.
    /// </summary>
    public class TextureHashToTextureInfo : ScriptableObject
    {
        /// <summary>
        /// Number of elements in the dictionary. Represents number of textures (without mipmaps)
        /// stored in this scriptable object.
        /// </summary>
        [field: SerializeField, ReadOnlyGUI] public int TextureHashCount { get; private set; }
        /// <summary>
        /// All texture hashes stored in this object.
        /// </summary>
        [field: SerializeField, ReadOnlyGUI] public string[] Hashes { get; internal set; }
        /// <summary>
        /// Description of each hashed texture.
        /// </summary>
        [field: SerializeField, ReadOnlyGUI] public TextureInfo[] TextureInfos { get; internal set; }

        public Dictionary<string, TextureInfo> ReconstructDictionary()
        {
            var dictionary = new Dictionary<string, TextureInfo>();
            dictionary.AddArraysAsKeyValuePairs(Hashes, TextureInfos);
            TextureHashCount = dictionary.Count;
            return dictionary;
        }

        private Dictionary<string, TextureInfo> dictionary;
        public Dictionary<string, TextureInfo> GetDictionary()
        {
            if (dictionary == null)
            {
                dictionary = ReconstructDictionary();
            }
            return dictionary;
        }

    }
}
