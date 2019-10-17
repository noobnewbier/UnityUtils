//
// Unityちゃん用の三人称カメラ
// 
// 2013/06/07 N.Kobyasahi
//

using UnityEngine;

namespace UnityUtils
{
    [RequireComponent(typeof(Camera))]
    public class ThirdPersonCamera : MonoBehaviour
    {
        [SerializeField] private Transform cameraPosition; // the usual position for the camera, specified by a transform in the game

        [SerializeField] private float smooth = 3f; // カメラモーションのスムーズ化用変数

        private void Start()
        {
            var selfTransform = transform;
            selfTransform.position = cameraPosition.position;
            selfTransform.forward = cameraPosition.forward;
        }


        private void FixedUpdate() // このカメラ切り替えはFixedUpdate()内でないと正常に動かない
        {
            SetCameraPosition(cameraPosition);
        }

        private void SetCameraPosition(Transform targetTransform)
        {
            var selfTransform = transform;
            selfTransform.position = Vector3.Lerp(selfTransform.position, targetTransform.position, Time.fixedDeltaTime * smooth);
            selfTransform.forward = Vector3.Lerp(selfTransform.forward, targetTransform.forward, Time.fixedDeltaTime * smooth);
        }
    }
}