using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public abstract class GfzUpdateable : MonoBehaviour
    {
        public abstract void OnUpdate();

        protected virtual void Reset()
        {
            OnUpdate();
        }

        protected virtual void OnValidate()
        {
            OnUpdate();
        }
    }
}
