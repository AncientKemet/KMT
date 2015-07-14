using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Client.Net;
using Client.UI.Interfaces;
using Client.Units;
using Code.Core.Client.UI.Controls;
using Code.Core.Client.UI.Interfaces.UpperLeft;
using Code.Libaries.Generic;
using Code.Libaries.Net.Packets.ForServer;
using UnityEngine;

namespace Client.Enviroment
{
    [RequireComponent(typeof(Clickable))]
    [ExecuteInEditMode]
    public class KemetMap : MonoSingleton<KemetMap>
    {

        private static Dictionary<string, KemetMap> activeMaps = new Dictionary<string, KemetMap>();

        public RaycastHit MouseAt;

        public static KemetMap GetMap(string id)
        {
            if (activeMaps.ContainsKey(id))
            {
                if (activeMaps[id] != null)
                {
                    return activeMaps[id];
                }
            }
            KemetMap newMap = ((GameObject)Instantiate(Resources.Load("Maps/" + id))).GetComponent<KemetMap>();
            return newMap;
        }

        [SerializeField]
        private TerrainCollider
            _cachedTerrainColliderReference;
        [HideInInspector]
        public List<PrefabInstance> PrefabInstances;

        public TerrainCollider TerrainCollider
        {
            get
            {
                if (_cachedTerrainColliderReference == null)
                {
                    _cachedTerrainColliderReference = GetComponent<TerrainCollider>();
                }
                return _cachedTerrainColliderReference;
            }
        }

        private void Awake()
        {
            activeMaps.Add(name, this);
        }
        private void Start()
        {
            if(!Application.isPlaying)
                return;

            GetComponent<Clickable>().AddAction(new RightClickAction("Walk here", () =>
            {
                
                if (PlayerUnit.MyPlayerUnit != null)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    float distance = 100;
                    const int layerMask = 1 << 8;
                    
                    if (Physics.Raycast(ray, out MouseAt, distance, layerMask))
                    {
                        WalkOrTurn(MouseAt.point, true);
                    }
                }
            }));
        }


#if UNITY_EDITOR
        private List<PrefabInstance> _currentList;
        private int _ptr = 0;
#endif
        private void Update()
        {
            if (!CreateCharacterInterface.IsNull)
                return;

            if (PlayerUnit.MyPlayerUnit != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                float distance = 100;
                const int layerMask = 1 << 8;
                if (Physics.Raycast(ray, out MouseAt, distance, layerMask))
                {
                }
            }
        }
        
        public void WalkOrTurn(Vector3 point, bool walk)
        {
            if (MovementArrow.Instance != null)
            MovementArrow.Instance.Dismiss();
            if (walk)
            {
                MovementArrow.SpawnArrow(point);
                if (!UnitSelectionInterface.IsNull)
                    UnitSelectionInterface.I.Unit = null;
            }
            SendPacket(point, walk);
        }

        private void SendPacket(Vector3 hit, bool Walk)
        {
            Vector3 myPos = PlayerUnit.MyPlayerUnit.MovementTargetPosition;
            var update = new WalkRequestPacket();

            update.Mask = new BitArray(new[] { Walk });
            update.DirecionVector = hit - myPos;

            ClientCommunicator.Instance.WorldServerConnection.SendPacket(update);
            ClientCommunicator.Instance.WorldServerConnection.ReadAndExecute();
        }


    }
}
