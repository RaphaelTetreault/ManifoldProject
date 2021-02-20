namespace Manifold.IO
{
    public interface IBinaryAddressable
    {
        long StartAddress { get; set; }
        long EndAddress { get; set; }
    }
}