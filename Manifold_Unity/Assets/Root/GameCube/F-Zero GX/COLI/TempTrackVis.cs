using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameCube.FZeroGX.COLI_COURSE;

public class TempTrackVis : MonoBehaviour
{
    public float kRadius = 10f;

    [SerializeField] protected ColiSceneSobj coli;
    [SerializeField] protected Color debugColor = Color.white;

    private void OnDrawGizmos()
    {
        if (coli == null)
            return;

        Gizmos.color = debugColor;
        foreach (var node in coli.scene.trackNodes)
        {
            var from = node.point.positionStart;
            var to = node.point.positionEnd;
            Gizmos.DrawLine(from, to);
            Gizmos.DrawWireSphere(to, kRadius);
            Gizmos.DrawSphere(from, kRadius);

        }
    }
}
