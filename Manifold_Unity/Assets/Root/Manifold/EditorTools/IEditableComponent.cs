namespace Manifold.EditorTools
{
    public interface IEditableComponent<T>
    {
        // Define delegates
        public delegate void OnEditCallback(T value);

        // Events
        public event OnEditCallback OnEdited;
    }
}
