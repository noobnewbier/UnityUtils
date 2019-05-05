//
// Unityちゃん用の三人称カメラ
// 
// 2013/06/07 N.Kobyasahi
//

using UnityEngine;

namespace Utils
{
    [RequireComponent(typeof(Camera))]
    public class ThirdPersonCamera : MonoBehaviour
    {
        [SerializeField] Transform _cameraPosition;          // the usual position for the camera, specified by a transform in the game
        [SerializeField] float _smooth = 3f;       // カメラモーションのスムーズ化用変数

        void Start()
        {
            transform.position = _cameraPosition.position;
            transform.forward = _cameraPosition.forward;
        }


        void FixedUpdate()  // このカメラ切り替えはFixedUpdate()内でないと正常に動かない
        {
            SetCameraPosition(_cameraPosition);
        }
    
        void SetCameraPosition(Transform targetTransform)
        {
            transform.position = Vector3.Lerp(transform.position, targetTransform.position, Time.fixedDeltaTime * _smooth);
            transform.forward = Vector3.Lerp(transform.forward, targetTransform.forward, Time.fixedDeltaTime * _smooth);
        }
  
    }
}
