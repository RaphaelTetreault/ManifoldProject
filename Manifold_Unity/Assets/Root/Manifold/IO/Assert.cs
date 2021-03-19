using System;

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
            => IsFalse(!value, message);
    }
}
