namespace Manifold.IO
{
    public interface IDeepCopyable<T>
    {
        T CreateDeepCopy();
    }
}
