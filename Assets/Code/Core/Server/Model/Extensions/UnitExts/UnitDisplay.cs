using Libaries.IO;
using Shared.Content.Types;
#if SERVER
using System.Collections.Generic;

using Code.Code.Libaries.Net;
using UnityEngine;
using Code.Core.Shared.Content.Types;
using Server.Model.Entities;

namespace Server.Model.Extensions.UnitExts
{
    
    public class UnitDisplay : UnitUpdateExt
    {
        private bool IsItem { get { return _item != null; } }

        private int _modelId = 1;
        private Item _item;
        private bool _destroy;
        private ServerUnit _pickupingUnit;
        private float _size;
        private bool _hasCharacterCustomalization;

        private List<int> _addedEffects = new List<int>(); 

        private ServerUnit Unit;

        public int ModelID
        {
            get { return _modelId; }
            set {
                _modelId = value;
                if (value == 0 || value == 1)
                    _hasCharacterCustomalization = true;
                _wasUpdate = true;
            }
        }

        public Item Item
        {
            get { return _item; }
            set
            {
                _item = value;
                ModelID = _item.InContentManagerIndex;
            }
        }
        
        public bool Destroy
        {
            get { return _destroy; }
            set
            {
                _destroy = value;
                _wasUpdate = true;
            }
        }

        public ServerUnit PickupingUnit
        {
            get { return _pickupingUnit; }
            set
            {
                _pickupingUnit = value;
                Destroy = true;
            }
        }

        public float Size
        {
            get { return _size; }
            set
            {
                if (Mathf.Abs(_size - value) > 0.01f)
                {
                    _wasUpdate = true;
                }
                _size = value;
            }
        }

        public void SendEffect(int id)
        {
            _addedEffects.Add(id);
            _wasUpdate = true;
        }

        private int[] _characterCustomalizations = new int[10];

        public int HairColor
        {
            get { return _characterCustomalizations[0]; }
            set
            {
                _characterCustomalizations[0] = value;
                _wasUpdate = true;
                _hasCharacterCustomalization = true;
            }
        }
        public int Hairtype
        {
            get { return _characterCustomalizations[1]; }
            set
            {
                _characterCustomalizations[1] = value;
                _wasUpdate = true;
                _hasCharacterCustomalization = true;
            }
        }
        public int FaceType
        {
            get { return _characterCustomalizations[2]; }
            set
            {
                _characterCustomalizations[2] = value;
                _wasUpdate = true;
                _hasCharacterCustomalization = true;
            }
        }
        public int FaceColor
        {
            get { return _characterCustomalizations[3]; }
            set
            {
                _characterCustomalizations[3] = value;
                _wasUpdate = true;
                _hasCharacterCustomalization = true;
            }
        }
        public int SkinColor
        {
            get { return _characterCustomalizations[4]; }
            set
            {
                _characterCustomalizations[4] = value;
                _wasUpdate = true;
                _hasCharacterCustomalization = true;
            }
        }
        public int UnderwearColor
        {
            get { return _characterCustomalizations[5]; }
            set
            {
                _characterCustomalizations[5] = value;
                _wasUpdate = true;
                _hasCharacterCustomalization = true;
            }
        }

        public override byte UpdateFlag()
        {
            return 0x02;
        }

        protected override void pSerializeState(ByteStream packet)
        {
            packet.AddFlag(IsItem, false, Unit.IsStatic(), false, false, _modelId == 0 || _modelId == 1);
            packet.AddByte(ModelID);
            packet.AddFloat4B(_size);
            if (_modelId == 0 || _modelId == 1)
                {
                    packet.AddByte(_characterCustomalizations.Length);
                    for (int i = 0; i < _characterCustomalizations.Length; i++)
                    {
                        packet.AddByte(_characterCustomalizations[i]);
                    }
                }
        }

        protected override void pSerializeUpdate(ByteStream packet)
        {
            packet.AddFlag(IsItem, Destroy, Unit.IsStatic(), _addedEffects.Count > 0, _hasCharacterCustomalization);
            packet.AddByte(ModelID);
            packet.AddFloat4B(_size);

            if (Destroy)
            {
                packet.AddFlag(PickupingUnit != null);

                if (PickupingUnit != null)
                {
                    packet.AddShort(PickupingUnit.ID);
                }

                Destroy = false;
                Object.Destroy(entity.gameObject);
            }

            if (_addedEffects.Count > 0)
            {
                packet.AddByte(_addedEffects.Count);
                foreach (int id in _addedEffects)
                {
                    packet.AddByte(id);
                }
                _addedEffects.Clear();
            }
            if (_hasCharacterCustomalization)
            {
                _hasCharacterCustomalization = false;
                packet.AddByte(_characterCustomalizations.Length);
                for (int i = 0; i < _characterCustomalizations.Length; i++)
                {
                    packet.AddByte(_characterCustomalizations[i]);
                }
            }
        }

        protected override void OnExtensionWasAdded()
        {
            base.OnExtensionWasAdded();
            Size = 1f;
            Unit = entity as ServerUnit;
        }

        public override void Serialize(JSONObject j)
        {
            JSONObject display = new JSONObject();

            display.AddField("HairType", "" + Hairtype);
            display.AddField("HairColor", "" + HairColor);
            display.AddField("FaceType", "" + FaceType);
            display.AddField("FaceColor", "" + FaceColor);
            display.AddField("SkinColor", "" + SkinColor);
            display.AddField("UnderwearColor", "" + UnderwearColor);

            j.AddField("Display", display);
         }

        public override void Deserialize(JSONObject j)
        {
            JSONObject display = j.GetField("Display");

            Hairtype = int.Parse(display.GetField("HairType").str);
            HairColor = int.Parse(display.GetField("HairColor").str);
            FaceType = int.Parse(display.GetField("FaceType").str);
            FaceColor = int.Parse(display.GetField("FaceColor").str);
            SkinColor = int.Parse(display.GetField("SkinColor").str);
            UnderwearColor = int.Parse(display.GetField("UnderwearColor").str);
        }
    }
}
#endif
