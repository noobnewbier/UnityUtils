using System;
using UnityEngine;

namespace UnityUtils
{
    public class FaceMainCamera : MonoBehaviour
    {
        [SerializeField] private Transform transformToFace;
        [SerializeField] private Vector3 offsetAngle;
        

        private Transform _cameraTransform;

        private void OnEnable()
        {
            if (Camera.main != null) _cameraTransform = Camera.main.transform;
            else throw new InvalidOperationException("No main camera to be faced");
        }

        private void Update()
        {
            transformToFace.LookAt(_cameraTransform);
            
            transformToFace.Rotate(offsetAngle);
        }
    }
}