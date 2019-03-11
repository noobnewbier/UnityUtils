//
// Unityちゃん用の三人称カメラ
// 
// 2013/06/07 N.Kobyasahi
//
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] Transform _overviewPosition;

    public float smooth = 3f;       // カメラモーションのスムーズ化用変数
    Transform _standardPos;          // the usual position for the camera, specified by a transform in the game
    Transform _frontPos;         // Front Camera locater
    Transform _jumpPos;          // Jump Camera locater
    bool _isOverview = false;
    Camera _camera;


    void Start()
    {
        if (GameObject.Find("CamPos") == null)
        {
            Destroy(this);
            return;
        }
        // 各参照の初期化
        _standardPos = GameObject.Find("CamPos").transform;
        _camera = GetComponent<Camera>();

        transform.position = _standardPos.position;
        transform.forward = _standardPos.forward;
    }


    void FixedUpdate()  // このカメラ切り替えはFixedUpdate()内でないと正常に動かない
    {
        if (Input.GetMouseButtonDown(1))
        {
            ToggleCameraState();
        }
        SetCameraPosition(_isOverview ? _overviewPosition : _standardPos);
    }

    void ToggleCameraState()
    {
        _isOverview = !_isOverview;
        int shootAreaLayer = 1 << LayerMask.NameToLayer("ShootArea");

        if (_isOverview)
        {
            _camera.cullingMask |= shootAreaLayer;
        }
        else
        {
            _camera.cullingMask ^= shootAreaLayer;
        }
    }

    void SetCameraPosition(Transform targetTransform)
    {
        transform.position = Vector3.Lerp(transform.position, targetTransform.position, Time.fixedDeltaTime * smooth);
        transform.forward = Vector3.Lerp(transform.forward, targetTransform.forward, Time.fixedDeltaTime * smooth);
    }
  
}
