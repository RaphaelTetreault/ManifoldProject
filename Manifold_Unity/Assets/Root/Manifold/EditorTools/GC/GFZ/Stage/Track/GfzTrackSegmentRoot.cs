using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Manifold.EditorTools.GC.GFZ
{
    [ExecuteInEditMode]
    public class GfzTrackSegmentRoot : MonoBehaviour
    {
        private void Awake()
        {
            DisableTransform();
        }

        private void OnEnable()
        {
            DisableTransform();
        }

        private void OnDestroy()
        {
            EnableTransform();
        }


        private void DisableTransform()
        {
            if (transform.hideFlags.HasFlag(HideFlags.HideInInspector))
            {
                // Forcibly hide transform component
                transform.hideFlags |= HideFlags.HideInInspector;
                Debug.Log($"Transform of {name} has been disabled by {nameof(GfzTrackSegmentRoot)} script to enforce editor rules.");
            }
        }

        private void EnableTransform()
        {
            if (!transform.hideFlags.HasFlag(HideFlags.HideInInspector))
            {
                // Unhide the component
                transform.hideFlags &= ~HideFlags.HideInInspector;
                Debug.Log($"Transform of {name} has been re-enabled by {nameof(GfzTrackSegmentRoot)} script being destroyed.");
            }
        }


        private void OnValidate()
        {
            // The first track segment must be from f3(0,0,0)
            if (transform.position != Vector3.zero)
                transform.position = Vector3.zero;
        
            // There should be no rotation so we point forward (+Z)
            if (transform.rotation != Quaternion.identity)
                transform.rotation = Quaternion.identity;
        }
    }
}
