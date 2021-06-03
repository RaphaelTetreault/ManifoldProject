using Manifold.IO;
using System.IO;

namespace GameCube.GFZ.StaffGhost
{
    [System.Serializable]
    public struct StaffGhostData : IBinarySerializable, IFile
    {
        // METADATA
        [UnityEngine.SerializeField] private string name;
        public string timeDisplay;


        // FIELDS
        public MachineID machineID; // 0x00
        public CourseIndexGX courseID; // 0x01
        public byte[] unk_1;
        public CString username; // 0x08
        public byte timeMinutes; // 0x24
        public byte timeSeconds; // 0x25
        public short timeMilliseconds; // 0x26


        public string FileName
        {
            get => name;
            set => name = value;
        }

        public void Deserialize(BinaryReader reader)
        {
            byte temp = 0;
            reader.ReadX(ref temp);
            machineID = (MachineID)temp;

            reader.ReadX(ref temp);
            courseID = (CourseIndexGX)temp;

            reader.ReadX(ref unk_1, 6);
            reader.ReadX(ref username, true);

            reader.BaseStream.Seek(0x24, SeekOrigin.Begin);
            reader.ReadX(ref timeMinutes);
            reader.ReadX(ref timeSeconds);
            reader.ReadX(ref timeMilliseconds);

            timeDisplay = $"{timeMinutes:0}\'{timeSeconds:00}\"{timeMilliseconds:000}";
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }
    }
}
