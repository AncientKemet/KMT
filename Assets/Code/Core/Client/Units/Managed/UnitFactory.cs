using Client.Enviroment;
using Client.Units;
using Client.Units.UnitControllers;
using Code.Libaries.Generic;
using UnityEngine;

namespace Code.Core.Client.Units.Managed
{
    public class UnitFactory : MonoSingleton<UnitFactory> {

        [SerializeField]
        private PlayerUnit _playerUnitPrefab;

        [SerializeField] private Face _facePrefab;

        [SerializeField] private Projector _projector;

        /// <summary>
        /// Creates an player in the scene.
        /// </summary>
        /// <returns>The player.</returns>
        /// <param name="id">Identifier.</param>
        public PlayerUnit CreateNewUnit(int id){
            PlayerUnit playerUnit = ((GameObject)Instantiate (_playerUnitPrefab.gameObject)).GetComponent<PlayerUnit>();
            playerUnit.Id = id;
            playerUnit.transform.parent = KemetMap.Instance.transform;
            return playerUnit;
        }

        public Projector CreateProjector(PlayerUnit playerUnit)
        {
            Projector p = ((GameObject) Instantiate(_projector.gameObject)).GetComponent<Projector>();
            p.transform.parent = playerUnit.transform;
            p.transform.localPosition = Vector3.zero + new Vector3(0, 3f);
            p.transform.localScale = Vector3.one;
            p.gameObject.SetActive(false);
            return p;
        }

        public Face CreateFace(UnitDisplay display)
        {
            Face p = ((GameObject)Instantiate(_facePrefab.gameObject)).GetComponent<Face>();
            p.transform.parent = display.NeckBone;
            p.transform.localPosition = new Vector3(0.3421002f, 7.594749e-05f, -0.015f);
            p.transform.localEulerAngles = new Vector3(3.201651e-06f, 356.2337f, 90.00001f);
            p.transform.localScale = Vector3.one;
            return p;
        }
    }
}
