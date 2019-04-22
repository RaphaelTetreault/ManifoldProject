using StarkTools.IO;
using System.IO;
using UnityEngine;

[CreateAssetMenu(menuName = "Manifold/Import/" + "GMA Importer")]
public class GMAImporter : ImportSobjs<GMASobj>
{
    public override string ProcessMessage => $"{GetType().Name} process";

    public override string HelpBoxMessage => string.Empty;

    public override string TypeName => typeof(GMASobj).Name;

    protected override string DefaultQueryFormat => "*.gma";
}
