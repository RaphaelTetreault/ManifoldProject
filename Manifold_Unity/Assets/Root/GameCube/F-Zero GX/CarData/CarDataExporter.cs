using StarkTools.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;

namespace GameCube.FZeroGX.CarData
{
    [CreateAssetMenu(menuName = "Manifold/Export/" + "CarData Exporter")]
    public class CarDataExporter : ExportSobjs<CarDataSobj>
    {
        public override string HelpBoxMessage
            => null;

        public override string OutputFileExtension
            => null;
    }
}