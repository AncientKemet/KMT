using System;
using Client.Units;
using Code.Core.Client.Settings;
using Code.Libaries.Generic;
using UnityEngine;

namespace Code.Core.Client.Units.Managed
{
    public class UnitManager : MonoSingleton<UnitManager>
    {

        private PlayerUnit[] _playerUnits;

        void Awake()
        {
            _playerUnits = new PlayerUnit[GlobalConstants.Instance.MAX_UNIT_AMOUNT];
        }

        public void RegisterUnit(PlayerUnit PlayerUnit)
        {
            int id = FreeId;
            PlayerUnit.Id = id;
            _playerUnits [id] = PlayerUnit;
        }
    
        public void DeRegisterUnit(PlayerUnit playerUnit)
        {
            if (_playerUnits [playerUnit.Id] == playerUnit)
            {
                _playerUnits [playerUnit.Id] = null;
            } else
            {
                Debug.LogError("Broken PlayerUnit array.");
            }
        }

        public bool WasUnitRegistered(PlayerUnit playerUnit)
        {
            return playerUnit.Id != -1;
        }

        public PlayerUnit GetUnit(int id)
        {
            return _playerUnits [id];
        }

        private int FreeId
        {
            get
            {
                for (int i = 0; i < _playerUnits.Length; i++)
                {
                    if (_playerUnits [i] == null)
                    {
                        return i;
                    }
                }
                return -1;
            }
        }

        public PlayerUnit this[int key]
        {
            get
            {
                if (_playerUnits.Length > key)
                {
                    if (_playerUnits[key] == null)
                    {
                        _playerUnits[key] = UnitFactory.Instance.CreateNewUnit(key);
                    }
                    return _playerUnits[key];
                }
                throw new Exception("Bad index");
            }
        }

        public bool HasUnit(int unitId)
        {
            return _playerUnits[unitId] != null;
        }
    }
}
