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
    protected VehicleParameters vehicleParameters;

    public VehicleParameters Value
        => vehicleParameters;

    public static implicit operator VehicleParameters(VehicleParametersSobj sobj)
    {
        return sobj.Value;
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
