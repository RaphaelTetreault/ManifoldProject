namespace Manifold
{
    public static class DebugConsole
    {
        public static void Log(string message)
        {
            // TODO: write pipeline around this and save to text
            UnityEngine.Debug.Log(message);
            //throw new System.NotImplementedException();
        }
    }
}
