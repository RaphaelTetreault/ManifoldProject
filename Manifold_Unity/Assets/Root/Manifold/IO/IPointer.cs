namespace Manifold.IO
{
    public interface IPointer
    {
        int Address { get; }
        bool IsNotNullPointer { get; }
    }
}
