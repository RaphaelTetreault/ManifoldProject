using System;
using System.IO;
using UnityEngine;

namespace Manifold.IO
{
    [Serializable]
    public struct FileSystemSearchOption 
    {
        [SerializeField]
        public SearchOption fileSearchOption;

        [SerializeField, BrowseFolderField]
        public string importPath;

        [SerializeField]
        public string searchPattern;
    }
}