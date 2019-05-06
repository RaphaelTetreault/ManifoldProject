using StarkTools.IO;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using GameCube.FZeroGX;
using GameCube.FZeroGX.CarData;

[CreateAssetMenu]
public class VehicleParametersSobj : ScriptableObject, IBinarySerializable
{
    [SerializeField]
    public VehicleParameters vehicleParameters;




    public static implicit operator VehicleParameters(VehicleParametersSobj sobj)
    {
        return sobj.vehicleParameters;
    }

    public void Deserialize(BinaryReader reader)
    {
        reader.ReadX(ref vehicleParameters, true);
    }

    public void Serialize(BinaryWriter writer)
    {
        writer.WriteX(vehicleParameters);
    }
}
