using GameCube.GFZ.Camera;
using System.Collections;
using UnityEngine;

namespace Manifold.IO.GFZ.Camera
{
    public sealed class DebugLiveCameraStage : MonoBehaviour
    {
        [SerializeField]
        private LiveCameraStageSobj lcsSobj;
        [SerializeField]
        private Color32 gizmosColor = Color.blue;
        private float gizmosScale = 10f;

        private void Start()
        {
            StartCoroutine(CoPlayLiveCameraStage(lcsSobj));
        }

        public IEnumerator CoPlayLiveCameraStage(LiveCameraStage liveCameraStage)
        {
            var camera = UnityEngine.Camera.main;

            foreach (var pan in liveCameraStage.Pans)
            {
                var cameraPos = pan.from.cameraPosition;
                var lookatPos = pan.from.lookatPosition;
                var fov = pan.from.fov;
                for (int frame = 0; frame < pan.frameCount; frame++)
                {
                    cameraPos = Vector3.Lerp(cameraPos, pan.to.cameraPosition, pan.lerpSpeed);
                    lookatPos = Vector3.Lerp(lookatPos, pan.to.lookatPosition, pan.lerpSpeed);
                    fov = Mathf.Lerp(fov, pan.to.fov, pan.lerpSpeed);

                    camera.transform.position = cameraPos;
                    camera.transform.LookAt(lookatPos, Vector3.up);
                    camera.fieldOfView = fov;

                    // Wait 1/60 of a second
                    yield return new WaitForSeconds(1f / 60f);
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = gizmosColor;
            var liveCameraStage = lcsSobj.Value;
            var startPos = transform.position;

            foreach (var pan in liveCameraStage.Pans)
            {
                var cameraPos = pan.from.cameraPosition;
                var lookatPos = pan.from.lookatPosition;
                var fov = pan.from.fov;
                for (int frame = 0; frame < pan.frameCount; frame++)
                {
                    cameraPos = Vector3.Lerp(cameraPos, pan.to.cameraPosition, pan.lerpSpeed);
                    lookatPos = Vector3.Lerp(lookatPos, pan.to.lookatPosition, pan.lerpSpeed);
                    fov = Mathf.Lerp(fov, pan.to.fov, pan.lerpSpeed);

                    transform.position = cameraPos;
                    transform.LookAt(lookatPos, Vector3.up);

                    Gizmos.DrawLine(cameraPos, lookatPos);
                    Gizmos.DrawSphere(cameraPos, gizmosScale * 0.5f);
                    Gizmos.DrawWireSphere(lookatPos, gizmosScale * 0.5f);
                    Gizmos.DrawLine(cameraPos, cameraPos + transform.up * gizmosScale);
                }
            }

            transform.position = startPos;
        }
    }
}
