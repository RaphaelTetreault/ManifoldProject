using GameCube.GFZ.Stage;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public class GfzTextureScroll : MonoBehaviour,
        IGfzConvertable<TextureScroll>
    {
        [SerializeField] private Vector2[] textureScrollFields;

        public TextureScroll ExportGfz()
        {
            // If we get here, we have data. Let's build it.
            var textureScroll = new TextureScroll();
            textureScroll.Fields = new TextureScrollField[TextureScroll.kCount];
            for (int i = 0; i < textureScrollFields.Length; i++)
            {
                var field = textureScrollFields[i];
                textureScroll.Fields[i] = new TextureScrollField()
                {
                    x = field.x,
                    y = field.y,
                };
            }
            return textureScroll;
        }

        public void ImportGfz(TextureScroll textureScroll)
        {
            textureScrollFields = new Vector2[TextureScroll.kCount];
            // Only iterate if we have data to iterate over
            if (textureScroll != null)
            {
                Assert.IsTrue(textureScrollFields.Length == textureScroll.Fields.Length);

                for (int i = 0; i < textureScroll.Fields.Length; i++)
                {
                    var field = textureScroll.Fields[i];

                    if (field == null)
                        continue;

                    textureScrollFields[i] = new Vector2(field.x, field.y);
                    //DebugConsole.Log($"{textureScrollFields[i]}");
                }
            }
        }
    }
}
