using UnityEngine;

namespace Code.Core.Client.Controls.Camera
{
    public class cameracontroly : MonoBehaviour {
        float zoomSpeed = 20.0f,distanceMin=200.0f,distanceMax=500.0f;
        float maindir=0;
        float offdir=0;
        // Use this for initialization
        void Start () {
            transform.position =new Vector3 (0, (distanceMin + distanceMax) / 2f , 0);
        }

	
        // in function

	
        // Update is called once per frame
        void FixedUpdate () {
            offdir= zoomSpeed * Mathf.Cos (transform.rotation.eulerAngles[1] * (Mathf.PI / 180));
            maindir= zoomSpeed * Mathf.Sin (transform.rotation.eulerAngles[1] * (Mathf.PI / 180));

            if (transform.position.y + zoomSpeed< distanceMax &&Input.GetAxis ("Mouse ScrollWheel")<0) {
                transform.position = transform.position + new Vector3 (-maindir,  zoomSpeed, -offdir);
            } else {
                //debugg if previous check fails

            }
            if (transform.position.y - zoomSpeed > distanceMin && Input.GetAxis ("Mouse ScrollWheel") > 0) {
                transform.position = transform.position + new Vector3 (maindir, -zoomSpeed, offdir);
            } else {
            }}}
}
