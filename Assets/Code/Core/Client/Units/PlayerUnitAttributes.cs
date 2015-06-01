using System;
using System.Collections.Generic;
using Shared.Content.Types;

namespace Client.Units
{
    public class PlayerUnitAttributes
    {
        public float GetAttribute(UnitAttributeProperty t)
        {
            return Attributes[t];
        }

        public void SetAttribute(UnitAttributeProperty t, float value)
        {
            Attributes[t] = value;
        }

        public PlayerUnitAttributes()
        {
            foreach (var v in Enum.GetValues(typeof (UnitAttributeProperty)))
            {
                Attributes.Add((UnitAttributeProperty) v,0);
            }
        }

        private Dictionary<UnitAttributeProperty, float> Attributes = new Dictionary<UnitAttributeProperty, float>();

        public float CurrentHealth { get; set; }
        public float CurrentEnergy { get; set; }
    }
}