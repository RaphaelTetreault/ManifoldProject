namespace Manifold.IO
{
    public interface IPointer
    {
        int Address { get; }
        bool IsNotNull { get; }
        bool IsNull { get; }
    }
}
