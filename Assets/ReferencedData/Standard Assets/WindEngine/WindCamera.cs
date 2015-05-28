using UnityEngine;

[ExecuteInEditMode]
public class WindCamera : MonoBehaviour {
    private Camera _mainCamera;

    public Camera MainCamera
    {
        get { return _mainCamera ?? (_mainCamera = Camera.main); }
    }

    void Update ()
    {
        if(MainCamera != null)
        transform.position = MainCamera.transform.position + Vector3.up;
    }
}
