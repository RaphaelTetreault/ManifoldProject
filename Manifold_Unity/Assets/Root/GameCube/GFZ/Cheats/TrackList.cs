namespace GameCube.GFZ.Cheats
{
    public struct TrackList
    {
        public TrackList(byte track1, byte track2, byte track3, byte track4, byte track5)
        {
            this.track1 = track1;
            this.track2 = track2;
            this.track3 = track3;
            this.track4 = track4;
            this.track5 = track5;
        }

        public ulong track1;
        public ulong track2;
        public ulong track3;
        public ulong track4;
        public ulong track5;
    }
}
