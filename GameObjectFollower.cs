using UnityEngine;
using UnityEngine.Serialization;

namespace UnityUtils
{
    public class GameObjectFollower : MonoBehaviour
    {
        [FormerlySerializedAs("_smooth")] [SerializeField]
        private float smooth = 15f; // カメラモーションのスムーズ化用変数

        [FormerlySerializedAs("_transformToFollow")] [SerializeField]
        private Transform transformToFollow;

        private void FixedUpdate() // このカメラ切り替えはFixedUpdate()内でないと正常に動かない
        {
            SetPosition(transformToFollow.position);
            SetRotation(transformToFollow.rotation);
        }

        private void SetPosition(Vector3 targetPosition)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smooth);
        }

        private void SetRotation(Quaternion targetRotation)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * smooth);
        }
    }
}