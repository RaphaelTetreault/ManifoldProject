using System;
using System.Collections;
using System.Collections.Generic;

namespace Manifold.IO
{
    public static class Assert
    {
        public class AssertException : Exception
        {
            public AssertException() { }
            public AssertException(string message) : base(message) { }
            public AssertException(string message, Exception inner) : base(message, inner) { }
        }


        public static void IsTrue(bool value, string message = null)
        {
            if (value)
                return;

            if (string.IsNullOrEmpty(message))
            {
                throw new AssertException();
            }
            else
            {
                throw new AssertException(message);
            }
        }

        public static void IsFalse(bool value, string message = null)
            => IsTrue(!value, message);

        public static void ValidateReferencePointer(object reference, IPointer pointer)
        {
            bool referenceIsNull = reference == null;
            // There is an issue if one of the two are set, but when both are the same, no issue
            bool invalidState = referenceIsNull ^ pointer.IsNull;

            var msg = referenceIsNull
                ? $"Reference is null but pointer is not! Ptr(0x{pointer.Address:x8})"
                : $"Reference exists but pointer is null!";

            IsFalse(invalidState, msg);
        }


        public static void ReferencePointer(IBinaryAddressable reference, IPointer pointer)
        {
            // Validates reference-pointer null/instance connection
            ValidateReferencePointer(reference, pointer);

            // Validates that pointer and reference share the same address
            if (reference != null)
            {
                var refPtr = reference.GetPointer();
                var isSamePointer = pointer.Address == refPtr.Address;
                const string msg = "Reference's pointer and supplied pointer do not match!";
                IsTrue(isSamePointer, msg);
            }
        }

        public static void ReferencePointer(IBinaryAddressable[] reference, ArrayPointer pointer)
        {
            // Validates reference-pointer null/instance connection
            ValidateReferencePointer(reference, pointer);

            // Validates that pointer and reference share the same address
            if (reference != null && reference.Length > 0)
            {
                var refPtr = reference[0].GetPointer();
                var isSamePointer = pointer.Address == refPtr.Address;
                IsTrue(isSamePointer);
            }
        }

        public static void ContainsNoNulls<T>(T ienumerable)
            where T : IEnumerable
        {
            foreach (var item in ienumerable)
            {
                IsTrue(item != null, $"Value null found in ienumerable type {nameof(T)}!");
            }
        }
    }
}
