using UnityEngine;

public class PlayAnimScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Animator anim = GetComponent<Animator> ();
		anim.Play(0);
	}

}
