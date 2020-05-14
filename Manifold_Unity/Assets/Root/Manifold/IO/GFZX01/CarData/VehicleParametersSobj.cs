using GameCube.GFZX01.CarData;
using UnityEngine;

namespace Manifold.IO.GFZX01.CarData
{
    [CreateAssetMenu(menuName = MenuConst.GFZX01_CarData + "Vehicle Parameters")]
    public class VehicleParametersSobj : SerializableAssetWrapper<VehicleParameters> { }
}