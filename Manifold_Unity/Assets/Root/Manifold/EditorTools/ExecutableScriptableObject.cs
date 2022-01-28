using Manifold.IO;
using UnityEngine;

namespace Manifold.EditorTools
{
    public abstract class ExecutableScriptableObject : ScriptableObject,
        IExecutable
    {
        public abstract void Execute();

        public abstract string ExecuteText { get; }
    }
}
