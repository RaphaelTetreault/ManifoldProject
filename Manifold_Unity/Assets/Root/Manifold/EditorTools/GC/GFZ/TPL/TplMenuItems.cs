using GameCube.GFZ.TPL;
using Manifold.IO;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.TPL
{
    public static class TplMenuItems
    {
        private static readonly MD5 md5 = MD5.Create();

        /// <summary>
        /// Imports all textures and mipmaps and creates scriptable objects which hold references
        /// between textures and their source TPLs via texture hashes.
        /// </summary>
        //[MenuItem("Manifold/TPL/Import all textures and mipmaps (with hash reference objects)")]
        [MenuItem(GfzMenuItems.TPL.BuildHashReferenceObject, priority = GfzMenuItems.TPL.Priority.BuildHashReferenceObject)]
        public static void BuildHashReferenceObject() => SobjsBasedTpls(false, false);

        [MenuItem(GfzMenuItems.TPL.ImportTexturesNoMipmips, priority = GfzMenuItems.TPL.Priority.ImportTexturesNoMipmips)]
        public static void ImportTexturesNoMipmips() => SobjsBasedTpls(true, false);

        [MenuItem(GfzMenuItems.TPL.ImportTexturesWithMipmips, priority = GfzMenuItems.TPL.Priority.ImportTexturesWithMipmips)]
        public static void ImportTexturesWithMipmips() => SobjsBasedTpls(true, true);


        public static void SobjsBasedTpls(bool saveTexture, bool doMipmaps)
        {
            var settings = GfzProjectWindow.GetSettings();
            var inputPath = settings.SourceDirectory;
            var inputPaths = Directory.GetFiles(inputPath, "*.tpl", SearchOption.AllDirectories);

            var outputPath = settings.AssetsWorkingDirectory;
            var textureRootOutputPath = $"./{outputPath}/tpl/textures/";
            var scriptableObjectPath = $"{outputPath}/tpl/";
            Directory.CreateDirectory(textureRootOutputPath);

            // This type maps a texture's hash to information about what the texture is and where it came from
            var textureHashesToTextureInfo = ScriptableObject.CreateInstance<TextureHashToTextureInfo>();
            var textureInfoHashes = new List<string>();
            var textureInfos = new List<TextureInfo>();
            // This type maps TPL's texture to their textures' hashes
            var tplTexturesToTextureHash = ScriptableObject.CreateInstance<TplTextureToTextureHash>();
            var tplFileNames = new List<string>();
            var tplTextureHashes = new List<TplTextureHashes>();

            // Iterate over all TPLs found
            int tplCount = 0;
            int tplTotal = inputPaths.Length;
            var tplEnumerable = BinarySerializableIO.LoadFile<Tpl>(inputPaths);
            foreach (var tpl in tplEnumerable)
            {
                // Progress bar
                {
                    Debug.Log(inputPaths[tplCount]);
                    bool cancel = ProgressBar.ShowIndexed(tplCount, tplTotal, "Reading TPL Files", inputPaths[tplCount]);
                    if (cancel)
                        break;
                    tplCount++;
                }

                // Setup for tex-desc to hash
                tplFileNames.Add(tpl.FileName);
                var textureHashes = new TplTextureHashes()
                {
                    TextureHashes = new string[tpl.TextureDescriptions.Length],
                };
                tplTextureHashes.Add(textureHashes);

                // Iterate over all textures - foreach texture in tpl
                Assert.IsTrue(tpl.TextureDescriptions.Length == tpl.TextureSeries.Length);
                for (int i = 0; i < tpl.TextureDescriptions.Length; i++)
                {
                    var textureDescription = tpl.TextureDescriptions[i];
                    var textureSeries = tpl.TextureSeries[i];
                    Assert.IsTrue(textureDescription.IsNull == textureSeries is null);

                    if (textureSeries is null)
                        continue;

                    // Save all textures in TPL
                    var filePaths = GetTextureHash(textureSeries, textureRootOutputPath, saveTexture, false, doMipmaps);
                    var hash = Path.GetFileNameWithoutExtension(filePaths[0]);

                    // Set TPL hash for this index. If skipped due to above `continue`, stays null.
                    // Unity will serialize this to string.Empty "".
                    textureHashes.TextureHashes[i] = hash;

                    // Create a description entry for this texture / hash
                    var textureInfo = new TextureInfo()
                    {
                        SourceFileName = tpl.FileName,
                        AddressRange = textureSeries.AddressRange,
                        TextureFormat = textureDescription.TextureFormat,
                        PixelWidth = textureDescription.Width,
                        PixelHeight = textureDescription.Height,
                        TextureLevels = textureDescription.MipmapLevels,
                    };

                    bool containsTexture = textureInfoHashes.Contains(hash);
                    if (!containsTexture)
                    {
                        textureInfoHashes.Add(hash);
                        textureInfos.Add(textureInfo);
                    }
                    //else
                    //{
                    //    Debug.Log($"Skipped {hash}");
                    //}
                }
            }

            // Create TextureHashesToTextureInfo
            textureHashesToTextureInfo.Hashes = textureInfoHashes.ToArray();
            textureHashesToTextureInfo.TextureInfos = textureInfos.ToArray();
            Assert.IsTrue(textureHashesToTextureInfo.Hashes.Length == textureHashesToTextureInfo.TextureInfos.Length);
            string textureHashToTextureInfoPath = $"{scriptableObjectPath}/TPL-TextureHash-to-TextureInfo.asset";
            AssetDatabase.DeleteAsset(textureHashToTextureInfoPath);
            AssetDatabase.CreateAsset(textureHashesToTextureInfo, textureHashToTextureInfoPath);

            // Create TplTexturesToTextureHash
            tplTexturesToTextureHash.FileNames = tplFileNames.ToArray();
            tplTexturesToTextureHash.Hashes = tplTextureHashes.ToArray();
            Assert.IsTrue(tplTexturesToTextureHash.FileNames.Length == tplTexturesToTextureHash.Hashes.Length);
            string tplTextureToHashPath = $"{scriptableObjectPath}/TPL-TextureDescription-to-Hash.asset";
            AssetDatabase.DeleteAsset(tplTextureToHashPath);
            AssetDatabase.CreateAsset(tplTexturesToTextureHash, tplTextureToHashPath);

            // Wrap up
            ProgressBar.Clear();
            AssetDatabase.Refresh();

            // Uncomment to print out all hashes in text file
            //tplTexturesToTextureHash.Print();
        }

        [MenuItem("Manifold/TPL/Test - Save textures to select folder (no mipmaps)")]
        public static void ImportMainTexturesOnly()
        {
            var settings = GfzProjectWindow.GetSettings();
            var inputPath = settings.SourceDirectory;
            var inputPaths = Directory.GetFiles(inputPath, "*.tpl", SearchOption.AllDirectories);

            var outputPath = EditorUtility.OpenFolderPanel("Select TPL Files Output Folder", "", "");
            if (string.IsNullOrEmpty(outputPath))
                return;

            // Iterate over all TPLs found
            int tplCount = 0;
            int tplTotal = inputPaths.Length;
            var tplEnumerable = BinarySerializableIO.LoadFile<Tpl>(inputPaths);
            foreach (var tpl in tplEnumerable)
            {
                // Progress bar
                {
                    bool cancel = ProgressBar.ShowIndexed(tplCount, tplTotal, "Reading TPL Files", inputPaths[tplCount]);
                    if (cancel)
                        break;
                    tplCount++;
                }

                int index = -1;
                int digitsFormat = tpl.TextureSeries.Length.ToString().Length;

                var fileRoot = $"{outputPath}/{tpl.FileName}/";
                Directory.CreateDirectory(fileRoot);

                // Iterate over all textures
                foreach (var textureSeries in tpl.TextureSeries)
                {
                    index++;
                    if (textureSeries is null)
                        continue;

                    var mainTexture = textureSeries[0].Texture;
                    var texture = CreateTexture2D(mainTexture);
                    var pngData = texture.EncodeToPNG();
                    var hash = md5.ComputeHash(pngData);
                    var hashName = HashToString(hash);

                    var filePath = $"{fileRoot}/[{index.PadLeft(digitsFormat, '0')}] {hashName}.png";

                    if (!File.Exists(filePath))
                        using (var writer = new BinaryWriter(File.Create(filePath)))
                            writer.Write(pngData);
                }
            }

            // Wrap up
            ProgressBar.Clear();
            AssetDatabase.Refresh();
        }


        public static Texture2D CreateTexture2D(GameCube.GX.Texture.Texture texture)
        {
            var texture2D = new Texture2D(texture.Width, texture.Height);
            for (int h = 0; h < texture.Height; h++)
            {
                for (int w = 0; w < texture.Width; w++)
                {
                    var texColor = texture.Pixels[w + h * texture.Width];
                    var color = new Color32(texColor.r, texColor.g, texColor.b, texColor.a);
                    texture2D.SetPixel(w, (texture.Height - 1 - h), color); // unsure if flip is error from here
                }
            }
            return texture2D;
        }

        public static void SaveData(string filePath, byte[] data, bool overwrite)
        {
            if (!overwrite)
                if (File.Exists(filePath))
                    return;

            using (var writer = new BinaryWriter(File.Create(filePath)))
            {
                writer.Write(data);
            }
        }

        private static string HashToString(byte[] hash)
        {
            var builder = new StringBuilder();
            foreach (var value in hash)
            {
                builder.Append($"{value:x2}");
            }
            return builder.ToString();
        }

        private static string[] GetTextureHash(TextureSeries textureSeries, string rootPath, bool saveTextures, bool overwrite, bool doMipmaps)
        {
            var outputNames = new string[textureSeries.Length];

            var mainTex = CreateTexture2D(textureSeries[0].Texture);
            byte[] png = mainTex.EncodeToPNG();
            var name = HashToString(md5.ComputeHash(png, 0, png.Length));
            var filePath = $"{rootPath}{name}.png";
            outputNames[0] = filePath;

            if (saveTextures)
            {
                SaveData(filePath, png, overwrite);

                if (doMipmaps)
                {
                    for (int i = 1; i < textureSeries.Length; i++)
                    {
                        var mipmap = CreateTexture2D(textureSeries[i].Texture);
                        var mipmapPng = mipmap.EncodeToPNG();
                        var mipmapName = $"{name}_mipmap{i}";
                        filePath = $"{rootPath}{mipmapName}.png";
                        SaveData(filePath, mipmapPng, overwrite);
                        outputNames[i] = filePath;
                    }
                }
            }

            return outputNames;
        }
    }
}
