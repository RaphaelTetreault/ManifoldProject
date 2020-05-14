using GameCube.GFZX01.GMA;

namespace Manifold.IO.GFZX01.GMA
{
    [UnityEngine.CreateAssetMenu(menuName = MenuConst.GFZX01_GMA + "GMA")]
    public class GmaSobj : FileAssetWrapper<Gma>
    {
        public static implicit operator Gma(GmaSobj sobj)
        {
            return sobj.Value;
        }
    }
}