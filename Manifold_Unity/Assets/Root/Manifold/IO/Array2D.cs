using System;

namespace Manifold.IO
{
    [Serializable]
    [UnityEngine.SerializeField]
    public class Array2D<T>
    {
        [UnityEngine.SerializeField] private T[] array = new T[0];
        [UnityEngine.SerializeField] private int[] lengths = new int[0];
        [UnityEngine.SerializeField] private int[] indexes = new int[0];

        public int Length => lengths.Length;


        public T[] GetArray(int index)
        {
            var length = lengths[index];
            var sourceIndex = indexes[index];
            var subArray = new T[length];
            Array.Copy(array, sourceIndex, subArray, 0, length);

            return subArray;
        }

        public T[][] GetArrays()
        {
            var array2D = new T[lengths.Length][];
            for(int i = 0; i < array2D.Length; i++)
                array2D[i] = GetArray(i);

            return array2D;
        }

        public void AppendArray(T[] source)
        {
            // Append length of source to array
            lengths = ConcatValue(lengths, source.Length);
            // Append start index of new array to end of array
            indexes = ConcatValue(indexes, array.Length);
            // THEN concatenate the 2 arrays (would mutate length above if done sooner)
            array = ConcatArray(array, source);
        }

        private TValue[] ConcatValue<TValue>(TValue[] source, TValue value)
        {
            var destination = new TValue[source.Length + 1];
            source.CopyTo(destination, 0);
            destination[source.Length] = value;

            return destination;
        }

        private TValue[] ConcatArray<TValue>(TValue[] source1, TValue[] source2)
        {
            var destination = new TValue[source1.Length + source2.Length];
            source1.CopyTo(destination, 0);
            source2.CopyTo(destination, source1.Length);

            return destination;
        }
    }
}
