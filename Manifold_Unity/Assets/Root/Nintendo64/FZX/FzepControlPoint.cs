using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using Manifold.IO;

namespace Nintendo64.FZX
{
    [Serializable]
    public class FzepControlPoint : ITextSerializable
    {
        public string name;
        public float x;
        public float y;
        public float z;
        public float widthL;
        public float widthR;
        public int banking;
        public int trackType;
        // Need an enum? What is correct order?
        public Gate gate = Gate.None;
        public Structure structure = Structure.None;
        public StructureSide structureSide = StructureSide.None;
        public Feature dashPlate = Feature.None;
        public Feature pitArea = Feature.None;
        public Feature decoration = Feature.None;
        public Feature dirtZone = Feature.None;
        public Feature slipZone = Feature.None;
        public Feature trapField = Feature.None;
        public Feature jumpPlate = Feature.None;
        public int texture = -1;

        public Vector3 Position => new Vector3(x, y, z);
        public float FullWidth => widthL + widthR;

        public void Deserialize(StreamReader reader)
        {
            name = reader.ReadLine();
            x = float.Parse(Regex.Match(reader.ReadLine(), Const.Regex.MatchFloat).ToString());
            y = float.Parse(Regex.Match(reader.ReadLine(), Const.Regex.MatchFloat).ToString());
            z = float.Parse(Regex.Match(reader.ReadLine(), Const.Regex.MatchFloat).ToString());
            widthL = float.Parse(Regex.Match(reader.ReadLine(), Const.Regex.MatchFloat).ToString());
            widthR = float.Parse(Regex.Match(reader.ReadLine(), Const.Regex.MatchFloat).ToString());
            banking = int.Parse(Regex.Match(reader.ReadLine(), Const.Regex.MatchIntegers).ToString());
            trackType = int.Parse(Regex.Match(reader.ReadLine(), Const.Regex.MatchIntegers).ToString());

            string nextLine;
            while (!string.IsNullOrEmpty(nextLine = reader.ReadLine()))
            {
                // split on space, read first value
                var split = nextLine.Split(' ');
                switch (split[0])
                {
                    case "DartZone":
                        dirtZone = (Feature)int.Parse(Regex.Match(nextLine, Const.Regex.MatchIntegers).ToString());
                        break;

                    case "DashPlate":
                        dashPlate = (Feature)int.Parse(Regex.Match(nextLine, Const.Regex.MatchIntegers).ToString());
                        break;

                    case "Decoration":
                        decoration = (Feature)int.Parse(Regex.Match(nextLine, Const.Regex.MatchIntegers).ToString());
                        break;

                    case "Gate":
                        gate = (Gate)int.Parse(Regex.Match(nextLine, Const.Regex.MatchIntegers).ToString());
                        break;

                    case "JumpPlate":
                        jumpPlate = (Feature)int.Parse(Regex.Match(nextLine, Const.Regex.MatchIntegers).ToString());
                        break;

                    case "PitArea":
                        pitArea = (Feature)int.Parse(Regex.Match(nextLine, Const.Regex.MatchIntegers).ToString());
                        break;

                    case "SlipZone":
                        slipZone = (Feature)int.Parse(Regex.Match(nextLine, Const.Regex.MatchIntegers).ToString());
                        break;

                    case "Structure":
                        structure = (Structure)int.Parse(Regex.Match(nextLine, Const.Regex.MatchIntegers).ToString());
                        var side = Regex.Match(nextLine, Const.Regex.MatchWithinParenthesis).ToString();
                        structureSide = side.Contains("Right") ? StructureSide.Right : side.Contains("Left") ? StructureSide.Left : StructureSide.None;
                        break;

                    case "Texture":
                        texture = int.Parse(Regex.Match(nextLine, Const.Regex.MatchIntegers).ToString());
                        break;

                    case "TrapField":
                        trapField = (Feature)int.Parse(Regex.Match(nextLine, Const.Regex.MatchIntegers).ToString());
                        break;

                    default:
                        throw new NotImplementedException(split[0]);
                }
            }
        }

        public void Serialize(StreamWriter writer)
        {
            throw new NotImplementedException();
        }
    }

}
