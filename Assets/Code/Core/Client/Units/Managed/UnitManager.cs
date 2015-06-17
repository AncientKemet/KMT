using System;
using Client.Units;
using Code.Core.Client.Settings;
using Code.Libaries.Generic;

namespace Code.Core.Client.Units.Managed
{
    public class UnitManager : MonoSingleton<UnitManager>
    {

        private PlayerUnit[] _playerUnits;

        void Awake()
        {
            _playerUnits = new PlayerUnit[GlobalConstants.Instance.MAX_UNIT_AMOUNT];
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
