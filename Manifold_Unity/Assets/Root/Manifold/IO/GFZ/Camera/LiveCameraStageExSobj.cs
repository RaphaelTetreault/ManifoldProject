using Manifold.IO;
using System.IO;
using UnityEngine;

namespace Manifold.IO.GFZ.Camera
{
    [CreateAssetMenu(menuName = MenuConst.GfzCamera + "livecam_stage EX")]
    public class LiveCameraStageExSobj : ScriptableObject, IBinarySerializable, IFile
    {
        #region

        [Header("Livecam Stage")]
        [SerializeField]
        string name;

        [SerializeField]
        public CameraPanSobj[] pans;

        #endregion

        #region PROPERTIES

        public string FileName
        {
            get => name;
            set => name = value;
        }

        public CameraPanSobj[] Pans => pans;

        #endregion

        public void SetCameraPans(CameraPanSobj[] pans)
        {
            this.pans = pans;
        }

        public void Deserialize(BinaryReader reader)
        {
            // See the importer to perhaps understand that deserializing this as Sobjs isn't so simple.
            throw new System.Exception("Cannot deserialize this Sobj without writing to AssetDatabase.");
        }

        public void Serialize(BinaryWriter writer)
        {
            foreach (var pan in pans)
            {
                pan.Serialize(writer);
            }
        }
    }
}
