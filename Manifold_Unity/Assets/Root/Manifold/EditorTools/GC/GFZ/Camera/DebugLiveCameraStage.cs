using GameCube.GFZ.Camera;
using System.Collections;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Camera
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
                var cameraPos = pan.From.CameraPosition;
                var lookatPos = pan.From.LookAtPosition;
                var fov = pan.From.FieldOfView;
                for (int frame = 0; frame < pan.FrameCount; frame++)
                {
                    cameraPos = Vector3.Lerp(cameraPos, pan.To.CameraPosition, pan.LerpSpeed);
                    lookatPos = Vector3.Lerp(lookatPos, pan.To.LookAtPosition, pan.LerpSpeed);
                    fov = Mathf.Lerp(fov, pan.To.FieldOfView, pan.LerpSpeed);

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
                var cameraPos = pan.From.CameraPosition;
                var lookatPos = pan.From.LookAtPosition;
                var fov = pan.From.FieldOfView;
                for (int frame = 0; frame < pan.FrameCount; frame++)
                {
                    cameraPos = Vector3.Lerp(cameraPos, pan.To.CameraPosition, pan.LerpSpeed);
                    lookatPos = Vector3.Lerp(lookatPos, pan.To.LookAtPosition, pan.LerpSpeed);
                    fov = Mathf.Lerp(fov, pan.To.FieldOfView, pan.LerpSpeed);

                    transform.position = cameraPos;
                    transform.LookAt(lookatPos, Vector3.up);

                    Gizmos.DrawLine(cameraPos, lookatPos);
                    Gizmos.DrawSphere(cameraPos, gizmosScale * 0.5f);
                    Gizmos.DrawWireSphere(lookatPos, gizmosScale * 0.5f);
                    Gizmos.DrawLine(cameraPos, (Vector3)cameraPos + transform.up * gizmosScale);
                }
            }

            transform.position = startPos;
        }
    }
}
