using Code.Core.Client.Settings;
using Server;
using Server.Model.Entities;
using Server.Servers;
using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class RelationShip : MonoBehaviour
{
    [SerializeField]
    private PrefabInstance Other;

    [SerializeField] private int Id = -1;
    private ServerUnit _unit;
    public ServerUnit Unit
    {
        get
        {
            if (_unit == null)
            if (Application.isPlaying)
                if (!ServerSingleton.IsNull &&
                    ServerSingleton.Instance.GetComponent<WorldServer>() != null &&
                    ServerSingleton.Instance.GetComponent<WorldServer>().World != null &&
                    ServerSingleton.Instance.GetComponent<WorldServer>().World.Units.Count > Id &&
                    ServerSingleton.Instance.GetComponent<WorldServer>().World.Units[Id + GlobalConstants.Instance.STATIC_UNIT_OFFSET] != null)
                    _unit = ServerSingleton.Instance.GetComponent<WorldServer>().World.Units[Id + GlobalConstants.Instance.STATIC_UNIT_OFFSET];
            return _unit;
        }
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (!Application.isPlaying)
        {
            if (Other != null)
            {
                gameObject.transform.position = (transform.parent.position + Other.transform.position)/2f;
                if (Other == transform.parent.GetComponent<PrefabInstance>())
                    Other = null;
                Id = Other.UnitId;
            }
            else
            {
                Id = -1;
            }
        }
    }
#endif
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            Gizmos.color = Color.red;
            if (Other != null)
                Gizmos.DrawLine(Other.transform.position, transform.position);
            if (transform.parent != null)
                Gizmos.DrawLine(transform.parent.position, transform.position);
        }
        else
        {
            Gizmos.color = Color.green;
            if (Unit != null)
                Gizmos.DrawLine(_unit.Movement.Position, transform.position);
            if (transform.parent != null)
                Gizmos.DrawLine(transform.parent.position, transform.position);
        }
    }
#endif
}
