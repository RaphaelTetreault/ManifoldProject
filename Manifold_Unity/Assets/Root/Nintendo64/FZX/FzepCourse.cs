using Manifold;
using Manifold.IO;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Nintendo64.FZX
{
    [Serializable]
    public class FzepCourse : ITextSerializable
    {
        public string name;
        public string descriptionEn;
        public string descriptionJp;
        public string source;

        public Venue venue;
        public Sky sky;
        public CartridgeMusic romMusic;
        public DiskDriveMusic ekMusic;

        public Horizon[] horizons;
        public FzepControlPoint[] controlPoints;

        public void Deserialize(StreamReader reader)
        {
            // Read header strings
            name = reader.ReadLineSkipPrefix("Name = ");
            descriptionEn = reader.ReadLineSkipPrefix("Description = ");
            descriptionJp = reader.ReadLineSkipPrefix("JDescription = ");
            source = reader.ReadLineSkipPrefix("Source = ");

            Assert.IsTrue(string.IsNullOrEmpty(reader.ReadLine()));

            //
            venue = (Venue)int.Parse(Regex.Match(reader.ReadLine(), ConstRegex.MatchIntegers).ToString());
            sky = (Sky)int.Parse(Regex.Match(reader.ReadLine(), ConstRegex.MatchIntegers).ToString());
            romMusic = (CartridgeMusic)int.Parse(Regex.Match(reader.ReadLine(), ConstRegex.MatchIntegers).ToString());
            ekMusic = (DiskDriveMusic)int.Parse(Regex.Match(reader.ReadLine(), ConstRegex.MatchIntegers).ToString());

            Assert.IsTrue(string.IsNullOrEmpty(reader.ReadLine()));

            // 
            int horizonCount = int.Parse(Regex.Match(reader.ReadLine(), ConstRegex.MatchIntegers).ToString());
            int controlPointCount = int.Parse(Regex.Match(reader.ReadLine(), ConstRegex.MatchIntegers).ToString());

            Assert.IsTrue(string.IsNullOrEmpty(reader.ReadLine()));
            Assert.IsTrue(string.IsNullOrEmpty(reader.ReadLine()));

            // HORIZONS
            horizons = new Horizon[horizonCount];
            for (int i = 0; i < horizons.Length; i++)
            {
                horizons[i].Deserialize(reader);
            }

            if (horizonCount > 0)
                Assert.IsTrue(string.IsNullOrEmpty(reader.ReadLine()));

            // CONTROL POINTS
            controlPoints = new FzepControlPoint[controlPointCount];
            for (int i = 0; i < controlPoints.Length; i++)
            {
                controlPoints[i] = new FzepControlPoint();
                controlPoints[i].Deserialize(reader);
            }

            // END OF FILE
        }

        public void Serialize(StreamWriter writer)
        {
            throw new NotImplementedException();
        }
    }

}
