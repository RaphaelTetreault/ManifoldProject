using UnityEngine;

namespace Manifold.IO
{
    public abstract class ExecutableScriptableObject : ScriptableObject,
        IExecutable
    {
        public abstract void Execute();

        public abstract string ExecuteText { get; }
    }
}
