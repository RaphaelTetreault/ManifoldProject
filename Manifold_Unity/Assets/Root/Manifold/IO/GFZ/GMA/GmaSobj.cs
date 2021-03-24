using GameCube.GFZ.GMA;

namespace Manifold.IO.GFZ.GMA
{
    [UnityEngine.CreateAssetMenu(menuName = Const.Menu.GfzGMA + "GMA")]
    // TODO: refactor and properly commit this as GmaSobj
    // git doesn't know how to push this as it is case insensitive.
    public class GmaSobj : FileAssetWrapper<Gma> { }
}