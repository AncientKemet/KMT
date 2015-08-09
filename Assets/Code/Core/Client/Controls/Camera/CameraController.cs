using Client.Units;
using Code.Libaries.Generic;
using UnityEngine;

namespace Code.Core.Client.Controls.Camera
{
    public class CameraController : MonoSingleton<CameraController>
    {

        [SerializeField]
        private GameObject
            _objectToFollow;
        [SerializeField]
        private float
            _cameraY = 10;
        [SerializeField]
        private float
            _cameraToObjectDistance = 10;
        [SerializeField]
        private float
            _rotation;

        private Vector3 lastObjectPosition;
        private Vector3 objectLookVector3;

        private float _zoomFactor = 1f;

        public float rotation
        {
            get
            {
                return _rotation;
            }
            set
            {
                _rotation = value;
            }
        }

        public float CameraY
        {
            get { return _cameraY; }
            set { _cameraY = value; }
        }

        public float CameraToObjectDistance
        {
            get { return _cameraToObjectDistance; }
            set { _cameraToObjectDistance = value; }
        }

        void LateUpdate()
        {
            if (_objectToFollow == null && PlayerUnit.MyPlayerUnit != null)
            {
                _objectToFollow = PlayerUnit.MyPlayerUnit.gameObject;
                lastObjectPosition = _objectToFollow.transform.position;
            }
            if (_objectToFollow != null)
            {
                Vector3 objectPos = _objectToFollow.transform.position;

                objectLookVector3 = Vector3.Lerp(objectLookVector3, _objectToFollow.transform.forward * (objectPos - lastObjectPosition).magnitude, Time.deltaTime * 2f);

                float x = objectPos.x + CameraToObjectDistance * (_zoomFactor) * Mathf.Cos(_rotation);
                float z = objectPos.z + CameraToObjectDistance * (_zoomFactor) * Mathf.Sin(_rotation);

                Vector3 lookAtOffset = objectLookVector3;
                Vector3 _targetPos = new Vector3(x, objectPos.y + CameraY * (Input.GetMouseButton(2) ? 0.5f : 1f), z) + objectLookVector3;
                transform.position = Vector3.Lerp(transform.position, _targetPos, Time.deltaTime * 50);
                lastObjectPosition = objectPos;

                transform.LookAt(objectPos + Vector3.up + lookAtOffset);
            }
        }

    }
}
