using UnityEngine;

namespace Utils
{
    public class GameObjectFollower : MonoBehaviour
    {

        [SerializeField] Transform _transformToFollow;
        [SerializeField] float _smooth = 15f;       // カメラモーションのスムーズ化用変数
        Vector3 _offset;


        void Start()
        {
            _offset = transform.position - _transformToFollow.position;
        }


        void FixedUpdate()  // このカメラ切り替えはFixedUpdate()内でないと正常に動かない
        {
            SetPosition((_transformToFollow.rotation * _offset) + _transformToFollow.position);
        }

        void SetPosition(Vector3 targetPosition)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * _smooth);
        }
    }
}
