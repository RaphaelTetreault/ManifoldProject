using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempDebugEmpty : MonoBehaviour
{
    public const float kSize = 100f;

    private void OnDrawGizmos()
    {
        var pos = transform.position;
        var up = transform.up * kSize;
        var fwd = transform.forward * kSize;
        var rgt = transform.right * kSize;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(pos, pos + up);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(pos, pos + fwd);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(pos, pos + rgt);
    }
}
