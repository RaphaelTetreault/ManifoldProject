using System;
using System.Text;

namespace Manifold.IO
{
    /// <summary>
    /// Simple wrapper class for string to encode and decode a C-style null-terminated
    /// string in SHIFT_JIS format.
    /// </summary>
    [Serializable]
    public class ShiftJisCString : CString
    {
        // CONSTANTS
        public const int shiftJisCodepage = 932;
        public static readonly Encoding shiftJis = Encoding.GetEncoding(shiftJisCodepage);


        // CONSTRUCTORS
        public ShiftJisCString() : base() { }
        public ShiftJisCString(string value) : base(value) { }


        // PROPERTIES
        public override Encoding Encoding => shiftJis;


        // OPERATORS
        public static implicit operator string(ShiftJisCString cstr)
        {
            return cstr.value;
        }

        public static implicit operator ShiftJisCString(string str)
        {
            return new ShiftJisCString(str);
        }

    }
}
