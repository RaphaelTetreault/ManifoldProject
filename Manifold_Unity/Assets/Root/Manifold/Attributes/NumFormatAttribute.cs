public class NumFormat : UnityEngine.PropertyAttribute
{
    public int NumBase { get; private set; }
    public int NumDigits { get; private set; }
    public string Format0 { get; private set; }

    public NumFormat(string format0 = "{0}", int numDigits = 0, int numBase = 10)
    {
        NumDigits = numDigits;
        NumBase = numBase;
        Format0 = format0;
    }
}