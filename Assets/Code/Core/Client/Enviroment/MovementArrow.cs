using Client.Units;
using Libaries.UnityExtensions.Independent;
using Server.Model.Content.Spawns;
using UnityEngine;
using System.Collections;

public class MovementArrow : MonoBehaviour {
    private static MovementArrow _instance;

    public static MovementArrow Instance
    {
        get { return _instance; }
        private set
        {
            if(_instance != null)
                _instance.Dismiss();
            _instance = value;
        }
    }

    public static MovementArrow SpawnArrow(Vector3 worldPosition)
    {
        MovementArrow newArrow = Instantiate((GameObject)Resources.Load("MovementArrow/MovementArrow")).GetComponent<MovementArrow>();
        newArrow.transform.position = worldPosition;
        return newArrow;
    }

    private bool _dismissed = false;

    void Start ()
    {
        Instance = this;
    }
	
	void Update ()
	{
	    if (PlayerUnit.MyPlayerUnit != null)
	        transform.LookAt(PlayerUnit.MyPlayerUnit.transform.position);
	    else
	        Dismiss();
	}

    public void Dismiss()
    {
        if (_dismissed)
            return;
        _dismissed = true;
        StartCoroutine(Ease.Vector(transform.localScale, Vector3.zero, vector3 => transform.localScale = vector3,
                                   () => Destroy(gameObject), 0.1f));
    }
}
