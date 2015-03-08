using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class GridMovement : MonoBehaviour
{

    public float Range = 0.5f;
    public float Rot = 0.01111111f;

	// Update is called once per frame
	void Update () {
        transform.localPosition = new Vector3((int)(transform.localPosition.x * Range) / Range, transform.localPosition.y, (int)(transform.localPosition.z * Range) / Range);
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, (int)(transform.localEulerAngles.y * Rot) / Rot, transform.localEulerAngles.z);
    }
}
