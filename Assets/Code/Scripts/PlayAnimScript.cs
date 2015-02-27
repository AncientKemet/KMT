using UnityEngine;
using System.Collections;

public class PlayAnimScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Animator anim = GetComponent<Animator> ();
		anim.Play(0);
	}

}
