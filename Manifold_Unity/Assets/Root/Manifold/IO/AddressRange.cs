namespace Manifold.IO
{
    [System.Serializable]
    public struct AddressRange
    {
        [Hex(8)]
        public long startAddress;

        [Hex(8)]
        public long endAddress;
    }
}
