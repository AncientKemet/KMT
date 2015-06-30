using System.Collections;
using System.Collections.Generic;
using Client.Controls;
using Client.Net;
using Client.UI.Interfaces;
using Client.Units;
using Code.Core.Client.Controls;
using Code.Core.Client.UI.Controls;
using Code.Core.Client.UI.Interfaces.UpperLeft;
using Code.Core.Shared.NET;
using Code.Libaries.Generic;
using Code.Libaries.Net.Packets.ForServer;
using UnityEngine;

namespace Client.Enviroment
{
    [RequireComponent(typeof(Clickable))]
    public class KemetMap : MonoSingleton<KemetMap>
    {

        private static Dictionary<string, KemetMap> activeMaps = new Dictionary<string, KemetMap>();

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

        private bool _wasMovingLastFrame = false;
        private float _timeDown = 0f;

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
            GetComponent<Clickable>().OnLeftClick += delegate { if (!UnitSelectionInterface.IsNull) UnitSelectionInterface.I.Unit = null; };
        }

        private void Update()
        {
            if (!CreateCharacterInterface.IsNull)
                return;

            if (Input.GetMouseButtonUp(1))
            {
                _timeDown = 0;
            }
            if (Input.GetMouseButton(1))
            {
                if (_timeDown > 0.25f)
                {
                    if (PlayerUnit.MyPlayerUnit != null)
                    {
                        RaycastHit hit;
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        float distance = 100;
                        const int layerMask = 1 << 8;
                        if (Physics.Raycast(ray, out hit, distance, layerMask))
                        {
                            SendPacket(hit, true);
                            _wasMovingLastFrame = true;
                        }
                    }
                }
                else
                {
                    _timeDown += Time.deltaTime;
                }
            }
            else
            {
                //If iam holding my spell keys i shall rotate towards my mouse point anyway
                if (ClientCommunicator.Instance.WorldServerConnection != null && KeyboardInput.Instance.FullListener == null)
                    if (Input.GetKey(ClientControlSettings.Spell0) || Input.GetKey(ClientControlSettings.Spell1) ||
                        Input.GetKey(ClientControlSettings.Spell2) || Input.GetKey(ClientControlSettings.Spell3))
                    {
                        RaycastHit hit;
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        float distance = 100;
                        const int layerMask = 1 << 8;
                        if (Physics.Raycast(ray, out hit, distance, layerMask))
                        {
                            SendPacket(hit, false);
                            _wasMovingLastFrame = true;
                        }
                    }
            }
        }

        #region DISABLED

        private void MoveMyPlayerFromMouse()
        {
            if (PlayerUnit.MyPlayerUnit != null)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                float distance = 100;
                int layerMask = 1 << 7;
                layerMask = ~layerMask;
                if (Physics.Raycast(ray, out hit, distance, layerMask))
                {
                    SendPacket(hit, true, true);
                }
            }
        }

        private void MoveMyPlayerToClick()
        {
            if (PlayerUnit.MyPlayerUnit != null)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                float distance = 100;
                int layerMask = 1 << 7;
                layerMask = ~layerMask;
                if (Physics.Raycast(ray, out hit, distance, layerMask))
                {
                    SendPacket(hit, false);
                }
            }
        }

        #endregion


        private void SendPacket(RaycastHit hit)
        {
            SendPacket(hit, false, false);
        }

        private void SendPacket(RaycastHit hit, bool Walk)
        {
            SendPacket(hit, Walk, false);
        }

        private void SendPacket(RaycastHit hit, bool Walk, bool invert)
        {

            Vector3 myPos = PlayerUnit.MyPlayerUnit.MovementTargetPosition;
            var update = new WalkRequestPacket();

            update.Mask = new BitArray(new[] { Walk });

            if (Walk)
            {
                if (invert)
                {
                    //used for going away from the hitpoint
                    Vector3 inverted = (hit.point - myPos).normalized;
                    inverted.x *= -1;
                    inverted.z *= -1;
                    update.DirecionVector = inverted;
                }
                else
                {
                    Vector3 difference = (hit.point - myPos);
                        update.DirecionVector = difference;
                }
            }
            else
            {
                update.DirecionVector = hit.point - myPos;
            }

            ClientCommunicator.Instance.WorldServerConnection.SendPacket(update);
        }


    }
}
