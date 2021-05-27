namespace Nintendo64.FZX
{
    public class Course
    {
        public CreatorID creatorID;
        public Venue venue;
        public Sky sky;
        public byte[] zero_0x04; // 28
        public FzepControlPoint[] controlPoints; // 64
        public ushort[] bankings;
        public byte[] pitAreaMap;
        public byte[] dashPlateMap;
        public byte[] dirtZoneMap;
        public byte[] slipZoneMap;
        public byte[] jumpPlateMap;
        public byte[] trapFieldMap;
        public byte[] structureMap;
        public byte[] decorationMap;
    }
}
