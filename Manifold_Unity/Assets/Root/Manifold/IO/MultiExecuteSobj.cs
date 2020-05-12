using Manifold.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.IO
{
    [CreateAssetMenu(menuName = "Manifold/Execute/" + "Multi-Execute Sobj")]
    public class MultiExecuteSobj : ExecutableScriptableObject
    {
        [SerializeField] protected ExecutableScriptableObject[] execSobjs;

        public override string ExecuteText => "Execute all In Order";

        public override void Execute()
        {
            foreach (var execSobj in execSobjs)
            {
                // Skip any nulls
                if (execSobj == null)
                    continue;

                execSobj.Execute();
            }
        }
    }
}