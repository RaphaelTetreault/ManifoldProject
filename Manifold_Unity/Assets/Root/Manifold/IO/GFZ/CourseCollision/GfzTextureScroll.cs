using GameCube.GFZ.CourseCollision;
using UnityEngine;

namespace Manifold.IO.GFZ.CourseCollision
{
    public class GfzTextureScroll : MonoBehaviour,
        IGfzConvertable<TextureScroll>
    {
        [SerializeField] private Vector3[] textureScrollFields;

        public TextureScroll ExportGfz()
        {
            var hasData = false;
            foreach (var field in textureScrollFields)
            {
                // See if the field has data
                //var isNotNull = field != Vector3.zero;
                var isNotNull = field.z > 0;
                if (isNotNull)
                {
                    hasData = true;
                    break;
                }
            }
            // We have no data to export
            if (!hasData)
            {
                DebugConsole.Log("??? " + name);

                //return null;
            }

            // If we get here, we have data. Let's build it.
            var textureScroll = new TextureScroll();
            textureScroll.fields = new TextureScrollField[TextureScroll.kCount];
            for (int i = 0; i < textureScrollFields.Length; i++)
            {
                var field = textureScrollFields[i];

                if (!(field.z > 0))
                    continue;

                Assert.IsTrue(field.z > 0);
                //if (field.x == 0 && field.y == 0)
                //    continue;

                textureScroll.fields[i] = new TextureScrollField()
                {
                    x = field.x,
                    y = field.y,
                };
            }
            return textureScroll;
        }

        public void ImportGfz(TextureScroll textureScroll)
        {
            textureScrollFields = new Vector3[TextureScroll.kCount];
            // Only iterate if we have data to iterate over
            if (textureScroll != null)
            {
                Assert.IsTrue(textureScrollFields.Length == textureScroll.fields.Length);

                for (int i = 0; i < textureScroll.fields.Length; i++)
                {
                    var field = textureScroll.fields[i];

                    if (field == null)
                        continue;

                    textureScrollFields[i] = new Vector3(field.x, field.y, 1000);
                    DebugConsole.Log($"{textureScrollFields[i]}");
                }
            }
        }
    }
}
