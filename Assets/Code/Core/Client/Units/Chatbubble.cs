using Client.Net;
using Client.Units;
using Code.Libaries.UnityExtensions;
using Libaries.UnityExtensions.Independent;
using UnityEngine;
using System.Collections;

public class Chatbubble : MonoBehaviour {


    public static Chatbubble Create(PlayerUnit unit, string text)
    {
        foreach (var b in unit.GetComponentsInChildren<Chatbubble>())
        {
            b.transform.localPosition += Vector3.up * 0.3f;
        }

        Chatbubble i = Instantiate((GameObject)Resources.Load("Chat/Chatbubble")).GetComponent<Chatbubble>();
        i.Text = text;
        i.transform.parent = unit.transform;
        i.transform.localPosition = Vector3.up*2.4f;
        
        return i;
    }

    public string Text
    {
        get { return _text; }
        set
        {
            _text = value;
            textMesh.text = value;
            textMesh.ForceBuild();
        }
    }

    [SerializeField] private tk2dTextMesh textMesh;
    private string _text;

    void Start ()
    {
        ClientCommunicator.Instance.StartCoroutine(Ease.Vector(Vector3.zero, new Vector3(1, -1, 1), vector3 => transform.localScale = vector3, null, 0.33f));
        ClientCommunicator.Instance.StartCoroutine(Ease.Vector(transform.localScale, new Vector3(0, 0, 0), vector3 => transform.localScale = vector3, () => Destroy(gameObject), 0.5f, 3f));
	}
	
	void LateUpdate () {
	    transform.LookAt(Camera.main.transform.position, Vector3.down);
	}
}
