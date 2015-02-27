#if SERVER
using System.Collections.ObjectModel;

using Code.Code.Libaries.Net;
using System.Collections.Generic;
using Server.Model.Entities;
using Server.Model.Entities.Human;

namespace Server.Model.Extensions.UnitExts
{
    public class UnitFocus : EntityExtension
    {
        private List<WorldEntity> _listeners = new List<WorldEntity>(5);

        public ReadOnlyCollection<WorldEntity> Listeners
        {
            get
            {

                return _listeners.AsReadOnly();
            }
        }

        private ServerUnit _focusedUnit { get; set; }

        public ServerUnit FocusedUnit 
        { 
            get 
            {
                return _focusedUnit;
            }
            set 
            {
                //Uninject old unit
                if (_focusedUnit != null)
                {
                    if (_focusedUnit.Focus != null)
                    {
                        _focusedUnit.Focus.RemoveListener(entity);
                    }
                }

                _focusedUnit = value;

                if (value != null)
                {
                    //Injet new units
                    if (value.Focus != null)
                    {
                        value.Focus.AddListener(entity);
                    }

                    //Exception
                    if (entity is Human)
                    {
                        (entity as Human).Anim.LookingAt = value;
                    }
                }
            }
        }

        private void RemoveListener(WorldEntity worldEntity)
        {
            _listeners.Remove(worldEntity);
        }

        private void AddListener(WorldEntity worldEntity)
        {
            _listeners.Add(worldEntity);
        }

        public override void Progress()
        {
        }

        public override void Serialize(ByteStream bytestream) { }
        public override void Deserialize(ByteStream bytestream) { }
    }
}
#endif
