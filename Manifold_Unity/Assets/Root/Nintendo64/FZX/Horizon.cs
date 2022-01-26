using System;
using System.IO;
using System.Text.RegularExpressions;
using Manifold;
using Manifold.IO;

namespace Nintendo64.FZX
{
    [Serializable]
    public struct Horizon : ITextSerializable
    {
        public string name;
        public Image image;
        public ushort zero_0x02;
        public float angleY;

        public void Deserialize(StreamReader reader)
        {
            name = reader.ReadLine();
            image = (Image)int.Parse(Regex.Match(reader.ReadLine(), Const.Regex.MatchIntegers).ToString());
            angleY = float.Parse(Regex.Match(reader.ReadLine(), Const.Regex.MatchFloat).ToString());
            Assert.IsTrue(string.IsNullOrEmpty(reader.ReadLine()));
        }

        public void Serialize(StreamWriter writer)
        {
            throw new NotImplementedException();
        }
    }

}
