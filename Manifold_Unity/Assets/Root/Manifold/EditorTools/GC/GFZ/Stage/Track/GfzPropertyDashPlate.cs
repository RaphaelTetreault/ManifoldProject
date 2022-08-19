using GameCube.GFZ;
using GameCube.GFZ.GMA;
using GameCube.GFZ.Stage;
using Manifold.EditorTools.GC.GFZ.TPL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public sealed class GfzPropertyDashPlate : GfzPropertyObject
    {
        const string dashBoard = "DASH_BOARD";
        const string dashCircle = "DASH_SIGN_CIRCLE";
        const string dashSignBolt = "DASH_SIGN";

        [SerializeField, Min(10f)] private float width = 15f;
        [SerializeField, Min(10f)] private float length = 29f;


        public override EmbeddedTrackPropertyType Type => EmbeddedTrackPropertyType.BoostPad;
        public override float PropertyWidth => width;
        public override float PropertyLength => length;


        public override string[] GetModelNames()
        {

            string[] modelNames = new string[]
            {
                dashBoard,
                dashCircle,
                dashSignBolt,
            };
            return modelNames;
        }

        public override Model[] GetModels(TplTextureContainer tpl)
        {
            var models = new Model[]
            {
                MakeBoard(width, length, tpl),
                //MakeCircle(),
                //MakeSign(),
            };
            return models;
        }

        public override SceneObjectDynamic[] GetSceneObjectDynamics()
        {
            var objects = new SceneObjectDynamic[]
            {
                CreateBoardInstance(),
            };
            return objects;
        }

        // Create models (ie: one time packing in output).
        private static Model MakeBoard(float width, float length, TplTextureContainer tpl)
        {
            var gcmfTemplate = GcmfTemplates.Objects.DashPlateBoard();
            var tristrips = TristripTemplates.Objects.DashPlateBoard(width, length);
            var gcmf = GcmfTemplate.CreateGcmf(gcmfTemplate, tristrips, tpl);

            var model = new Model();
            model.Name = dashBoard;
            model.Gcmf = gcmf;

            return model;
        }

        private static Model MakeCircle()
        {
            throw new NotImplementedException();
        }
        private static Model MakeSign()
        {
            throw new NotImplementedException();
        }

        // Create scene object dynamics
        private SceneObjectDynamic CreateBoardInstance()
        {
            var sceneObject = GetSharedBoardSceneObject();

            // TODO: F*CK. Maybe now is a good time to fix object placement?
            var transform = new TransformMatrix3x4();

            var textureScroll = new TextureScroll();
            textureScroll.Fields = new TextureScrollField[TextureScroll.kCount];
            textureScroll.Fields[0] = new TextureScrollField(0, 3);

            var sceneObjectDynamic = new SceneObjectDynamic
            {
                SceneObject = sceneObject,
                TransformMatrix3x4 = transform,
                TextureScroll = textureScroll,
            };

            return sceneObjectDynamic;
        }
        private SceneObjectDynamic MakeCircle1Instance()
        {
            throw new NotImplementedException();
        }
        private SceneObjectDynamic MakeCircle2Instance()
        {
            throw new NotImplementedException();
        }
        private SceneObjectDynamic MakeSignInstance()
        {
            throw new NotImplementedException();
        }



        private SceneObject SharedBoardSceneObject;
        private SceneObject SharedCircleSceneObject;
        private SceneObject SharedSignSceneObject;
        private SceneObject[] SharedSceneObjectReferences;

        private SceneObject GetSharedBoardSceneObject()
        {
            if (SharedBoardSceneObject == null)
            {
                var sceneObjectLODs = new SceneObjectLOD[]
                {
                    new SceneObjectLOD
                    {
                        Name = dashBoard,
                        LodDistance = 0f,
                    }
                };
                var sceneObject = new SceneObject
                {
                    LODs = sceneObjectLODs,
                    LodRenderFlags = 0,
                };
                SharedBoardSceneObject = sceneObject;
            }
            return SharedBoardSceneObject;
        }
        public SceneObject[] GetSharedSceneObjects()
        {
            // If null (ie: cold, do lazy init)
            if (SharedSceneObjectReferences == null)
            {
                SharedSceneObjectReferences = new SceneObject[]
                {
                    GetSharedBoardSceneObject(),
                };
            }
            // Now that it is init, we can return it each call.
            return SharedSceneObjectReferences;
        }

    }
}
