using UnityEngine;

namespace Code.Core.Client.Controls.Camera
{
    public class cameracontrolxz : MonoBehaviour {
        int width=Screen.width, height=Screen.height;
        int cameraspeed=10 , maxxpos=1000,maxzpos=1000,minxpos=-1000,minzpos=-1000;
        bool midmouseisdown=false;
        float maindir=0;
        float offdir=0;
        // Use this for initialization
        void Start () {
	
        }
	
        // Update is called once per frame
        void Update () {
            width = Screen.width;
            height = Screen.height;
            Vector3 oldpos = transform.position;
            maindir = cameraspeed * Mathf.Cos (transform.rotation.eulerAngles[1] * (Mathf.PI / 180));
            //Debug.Log (maindir+" "+transform.rotation.eulerAngles[1] * (Mathf.PI / 180));
            offdir = cameraspeed * Mathf.Sin (transform.rotation.eulerAngles[1] * (Mathf.PI / 180));
            if (Input.GetMouseButtonUp (2) == true)
                midmouseisdown = false;
            if(Input.GetMouseButtonDown(2)==true)
                midmouseisdown = true;
            Vector2 mousepos = Input.mousePosition;
            if(mousepos.x<(float)width*0.01f ){
                if(  midmouseisdown==true){
                    transform.Rotate (0,- cameraspeed/10f, 0,Space.World);
                }
                else{
                    transform.position = transform.position + new Vector3( -maindir, 0, offdir);}}
            if(mousepos.x>(float)width*0.99f){
                if( midmouseisdown==true){
                    transform.Rotate(0,cameraspeed/10f,0,Space.World);}
                else{
                    transform.position = transform.position + new Vector3( maindir, 0, -offdir);}}
            if(mousepos.y<(float)height*0.01f)
                transform.position = transform.position + new Vector3( -offdir,0 , -maindir);
            if(mousepos.y>(float)height*0.99f)
                transform.position = transform.position + new Vector3( offdir, 0, maindir);
            if (transform.position.x > maxxpos || transform.position.x < minxpos || transform.position.z < minzpos || transform.position.z > maxzpos) {
                transform.position=oldpos;	}
        }
    }
}
