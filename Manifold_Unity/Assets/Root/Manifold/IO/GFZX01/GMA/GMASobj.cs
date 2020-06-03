using GameCube.GFZX01.GMA;

namespace Manifold.IO.GFZX01.GMA
{
    [UnityEngine.CreateAssetMenu(menuName = MenuConst.GFZX01_GMA + "GMA")]
    // TODO: refactor and properly commit this as GmaSobj
    // git doesn't know how to push this as it is case insensitive.
    public class GMASobj : FileAssetWrapper<Gma> { }
}