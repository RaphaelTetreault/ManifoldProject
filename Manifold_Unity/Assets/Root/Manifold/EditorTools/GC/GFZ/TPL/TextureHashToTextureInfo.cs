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
        /// All texture hashes stored in this object.
        /// </summary>
        [field: SerializeField] public string[] Hashes { get; internal set; }
        /// <summary>
        /// Description of each hashed texture.
        /// </summary>
        [field: SerializeField] public TextureInfo[] TextureInfos { get; internal set; }

        public Dictionary<string, TextureInfo> GetDictionary()
        {
            var dictionary = new Dictionary<string, TextureInfo>();
            dictionary.AddArraysAsKeyValuePairs(Hashes, TextureInfos);
            return dictionary;
        }
    }
}
