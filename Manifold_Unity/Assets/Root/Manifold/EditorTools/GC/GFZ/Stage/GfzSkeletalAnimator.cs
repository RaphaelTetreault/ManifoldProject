﻿using GameCube.GFZ.Stage;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public class GfzSkeletalAnimator : MonoBehaviour,
        IGfzConvertable<SkeletalAnimator>
    {
        [SerializeField] private bool exportSkeletalAnimator;
        [SerializeField] private SkeletalAnimator srcSkeletalAnimator;

        public SkeletalAnimator ExportGfz()
        {
            if (exportSkeletalAnimator)
            {
                return srcSkeletalAnimator;
            }
            else
            {
                return null;
            }
        }

        public void ImportGfz(SkeletalAnimator value)
        {
            bool hasSkeletalAnimator = value != null;
            if (hasSkeletalAnimator)
            {
                srcSkeletalAnimator = value;
            }
            exportSkeletalAnimator = hasSkeletalAnimator;
        }
    }
}
