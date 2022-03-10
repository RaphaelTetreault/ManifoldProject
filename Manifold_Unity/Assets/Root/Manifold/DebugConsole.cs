namespace Manifold
{
    public static class DebugConsole
    {
        public static void Log(string message)
        {
            // TODO: write pipeline around this and save to text?
            
            #if UNITY_EDITOR
            UnityEngine.Debug.Log(message);
            #else
            //throw new System.NotImplementedException();
            #endif
        }
    }
}
