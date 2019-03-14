using UnityEngine;

[CreateAssetMenu(menuName = "Manifold/Export/" + "GMA Exporter")]
public class GMAExporter : ExportSobjs<GMASobj>
{
    public override string ProcessMessage => "Export successful";

    public override string HelpBoxMessage => "";

    public override string OutputFileExtension => "gma";
}
