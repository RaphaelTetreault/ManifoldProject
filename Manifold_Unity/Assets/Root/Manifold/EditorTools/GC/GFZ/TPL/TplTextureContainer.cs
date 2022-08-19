using System.Collections.Generic;
using System.Linq;

namespace Manifold.EditorTools.GC.GFZ.TPL
{
    /// <summary>
    /// Used for constructing a TPL file using texture hashes as the means not to duplicate
    /// texture data.
    /// </summary>
    public class TplTextureContainer
    {
        private readonly Dictionary<string, ushort> textureHashesToIndex = new();

        public ushort AddTextureHash(string textureHash)
        {
            ushort textureIndex = GetNextTextureIndex();
            textureHashesToIndex.Add(textureHash, textureIndex);
            return textureIndex;
        }

        public bool ContainsHash(string textureHash)
        {
            bool contains = textureHashesToIndex.ContainsKey(textureHash);
            return contains;
        }

        public ushort GetTextureHashIndex(string textureHash)
        {
            ushort index = textureHashesToIndex[textureHash];
            return index;
        }

        public string[] GetTextureHashes()
        {
            var textureHashes = textureHashesToIndex.Keys.ToArray();
            return textureHashes;
        }

        private ushort GetNextTextureIndex()
        {
            ushort textureCount = (ushort)textureHashesToIndex.Count;
            return textureCount;
        }
    }
}
