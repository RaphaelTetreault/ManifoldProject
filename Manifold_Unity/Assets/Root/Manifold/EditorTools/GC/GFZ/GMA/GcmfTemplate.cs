using GameCube.GFZ;
using GameCube.GFZ.GMA;
using GameCube.GFZ.Stage;
using GameCube.GX;
using Manifold.EditorTools.GC.GFZ.TPL;
using Manifold.EditorTools.GC.GFZ.Stage.Track;
using System.Collections.Generic;
using Unity.Mathematics;

namespace Manifold.EditorTools.GC.GFZ
{
    public class GcmfTemplate
    {
        public bool IsTranslucid { get; internal set; }
        public Submesh Submesh { get; internal set; } = null;
        public TevLayer[] TevLayers { get; internal set; } = new TevLayer[0];
        public string[] TextureHashes { get; internal set; } = new string[0];
        public TextureScrollField[] TextureScrollFields { get; internal set; } = null;


        private static Gcmf CreateFromTemplates(TplTextureContainer tplTextures, params GcmfTemplate[] templates)
        {
            ushort opaque = 0;
            ushort translucid = 0;
            var tevLayers = new List<TevLayer>();
            var submeshes = new Submesh[templates.Length];
            // todo: texture indexes and stuff

            ushort tevLayerOffset = 0;
            for (int templateIndex = 0; templateIndex < templates.Length; templateIndex++)
            {
                var template = templates[templateIndex];
                //var textureHashes = template.TextureHashes;

                // TEV LAYERS
                for (ushort tevIndex = 0; tevIndex < template.TevLayers.Length; tevIndex++)
                {
                    // Get texture index//
                    string textureHash = template.TextureHashes[tevIndex];
                    ushort textureIndex = GetTextureHashesIndex(textureHash, tplTextures);
                    // Get TEV index for this GCMF
                    ushort tevLayerIndex = checked((ushort)(tevIndex + tevLayerOffset));
                    TevLayer tevLayer = template.TevLayers[tevIndex];
                    // Assign indexes
                    tevLayer.TplTextureIndex = textureIndex;
                    tevLayer.TevLayerIndex = tevLayerIndex;

                    if (tevIndex == 0)
                        template.Submesh.Material.TevLayerIndex0 = (short)tevLayerIndex;
                    else if (tevIndex == 1)
                        template.Submesh.Material.TevLayerIndex1 = (short)tevLayerIndex;
                    else if (tevIndex == 2)
                        template.Submesh.Material.TevLayerIndex2 = (short)tevLayerIndex;
                }
                tevLayers.AddRange(template.TevLayers);
                tevLayerOffset = (ushort)tevLayers.Count;
                template.Submesh.Material.TevLayerCount = (byte)template.TevLayers.Length;

                // collect submesh
                submeshes[templateIndex] = template.Submesh;

                // TEMP: sort how many opaque / translucid
                opaque += (ushort)(template.IsTranslucid == false ? 1 : 0);
                translucid += (ushort)(template.IsTranslucid == true ? 1 : 0);
            }

            // Solved elsewhere: attributes, bounding sphere
            var gcmf = new Gcmf
            {
                TextureConfigsCount = (ushort)tevLayers.Count,
                OpaqueMaterialCount = opaque,
                TranslucidMaterialCount = translucid,
                TevLayers = tevLayers.ToArray(),
                Submeshes = submeshes,
            };
            gcmf.PatchTevLayerIndexes();

            return gcmf;
        }

        private static void AssignDisplayListsToGcmf(Gcmf gcmf, Tristrip[][] tristrips)
        {
            if (tristrips.Length != gcmf.Submeshes.Length)
                throw new System.ArgumentException("lengths do not match! Did you forget to assign equal tristrip and material templates?");

            for (int i = 0; i < gcmf.Submeshes.Length; i++)
            {
                var frontfacing = new List<Tristrip>();
                var backfacing = new List<Tristrip>();
                foreach (var tristrip in tristrips[i])
                {
                    if (tristrip.isBackFacing)
                        backfacing.Add(tristrip);
                    else
                        frontfacing.Add(tristrip);
                }
                var submesh = gcmf.Submeshes[i];
                submesh.PrimaryFrontFacing = TristripGenerator.TristripsToDisplayLists(frontfacing.ToArray(), GameCube.GFZ.GfzGX.VAT);
                submesh.PrimaryBackFacing = TristripGenerator.TristripsToDisplayLists(backfacing.ToArray(), GameCube.GFZ.GfzGX.VAT);
                submesh.VertexAttributes = TristripToAttribute(tristrips[i]);
            }
        }

        private static AttributeFlags TristripToAttribute(params Tristrip[] tristrips)
        {
            // This function is a big'ol hack

            // Return empty descriptor if nothing
            if (tristrips.IsNullOrEmpty())
                return 0;

            // Else grab attributes from first entry
            var dl = TristripGenerator.TristripsToDisplayLists(new Tristrip[] { tristrips[0] }, GameCube.GFZ.GfzGX.VAT);
            var attributes = dl[0].Attributes;
            return attributes;
        }

        // Modified from: http://www.technologicalutopia.com/sourcecode/xnageometry/boundingsphere.cs.htm
        private static GameCube.GFZ.BoundingSphere CreateBoundingSphereFromPoints(IEnumerable<UnityEngine.Vector3> points, int length)
        {
            if (points == null)
                throw new System.ArgumentNullException(nameof(points));
            if (length <= 0)
                throw new System.ArgumentOutOfRangeException(nameof(length));

            float radius = 0;
            float3 center = new float3();
            float lengthReciprocal = 1f / length;

            // First, we'll find the center of gravity for the point 'cloud'.
            foreach (var point in points)
            {
                float3 pointWeighted = point * lengthReciprocal;
                center += pointWeighted;
            }

            // Calculate the radius of the needed sphere (it equals the distance between the center and the point further away).
            foreach (var point in points)
            {
                float3 centerToPoint = (float3)point - center;
                float distance = math.length(centerToPoint);

                if (distance > radius)
                    radius = distance;
            }

            return new BoundingSphere(center, radius);
        }
        private static BoundingSphere CreateBoundingSphereFromTristrips(IEnumerable<Tristrip> tristrips)
        {
            var points = new List<UnityEngine.Vector3>();
            foreach (var tristrip in tristrips)
                points.AddRange(tristrip.positions);

            var boundingSphere = CreateBoundingSphereFromPoints(points, points.Count);
            return boundingSphere;
        }
        private static BoundingSphere CreateBoundingSphere(Tristrip[][] tristripsCollections)
        {
            // Linearize tristrips
            var allTristrips = new List<Tristrip>();
            foreach (var tristrips in tristripsCollections)
                allTristrips.AddRange(tristrips);

            var boundingSphere = CreateBoundingSphereFromTristrips(allTristrips);

            return boundingSphere;
        }

        public static Gcmf CreateGcmf(GcmfTemplate[] gcmfTemplates, Tristrip[][] tristripsCollection, TplTextureContainer tpl)
        {
            // TODO: assert GcmfTemplates.length == tristripsCollection.length

            // Combine templates. This means we have all TEV layers and stuff in place
            var gcmf = CreateFromTemplates(tpl, gcmfTemplates);

            // Assign display lists: now each submesh has it's geometry, too
            AssignDisplayListsToGcmf(gcmf, tristripsCollection);

            // Create a bounding sphere and assign data everywhere it is used
            var boundingSphere = CreateBoundingSphere(tristripsCollection);
            gcmf.BoundingSphere = boundingSphere;
            foreach (var submesh in gcmf.Submeshes)
                submesh.UnkAlphaOptions.Origin = boundingSphere.origin;

            // We done!
            return gcmf;
        }

        public static Gcmf CreateGcmf(GcmfTemplate gcmfTemplate, Tristrip[] tristripCollection, TplTextureContainer tpl)
        {
            var templates = new GcmfTemplate[] { gcmfTemplate };
            var tristrips = new Tristrip[][] { tristripCollection };
            var gcmf = CreateGcmf(templates, tristrips, tpl);
            return gcmf;
        }


        // Get the index of the texture for a future TPL. If texture exists, get existing index. If not, add to list, update index.
        private static ushort GetTextureHashesIndex(string textureHash, TplTextureContainer tpl)
        {
            ushort index = tpl.ContainsHash(textureHash)
                ? tpl.GetTextureHashIndex(textureHash)
                : tpl.AddTextureHash(textureHash);
            return index;
        }

        public static TextureScroll CombineTextureScrolls(GcmfTemplate[] templates)
        {
            int index = 0;
            var textureScrollFields = new TextureScrollField[TextureScroll.kCount];
            foreach (var template in templates)
            {
                if (template.TextureScrollFields == null)
                    continue;

                foreach (var field in template.TextureScrollFields)
                    textureScrollFields[index++] = field;

                if (index == TextureScroll.kCount)
                {
                    var msg = $"Maximum texture scroll fields exceeded. ({index}/{TextureScroll.kCount})";
                    throw new System.ArgumentException(msg);
                }
            }

            if (index > TextureScroll.kUsedCount)
            {
                var msg = $"Maximum _used_ texture scroll fields exceeded. ({index}/{4})";
                throw new System.ArgumentException(msg);
            }

            var textureScrolls = new TextureScroll()
            {
                Fields = textureScrollFields,
            };

            return textureScrolls;
        }

    }
}
