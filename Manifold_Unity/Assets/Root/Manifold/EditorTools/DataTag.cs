using UnityEngine;

namespace Manifold.EditorTools
{
    public class DataTag<T> : MonoBehaviour
    {
        [SerializeField]
        private T data;

        public T Data
        {
            get => data;
            set => data = value;
        }
    }
}
