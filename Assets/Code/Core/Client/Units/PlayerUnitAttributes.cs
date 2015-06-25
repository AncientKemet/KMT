using System;
using System.Collections;
using System.Collections.Generic;
using Shared.Content.Types;

namespace Client.Units
{
    public class PlayerUnitAttributes 
    {
        
        private float _currentHealth = -1;

        public Action<UnitAttributeProperty, float> OnChange;

        public float GetAttribute(UnitAttributeProperty t)
        {
            return Attributes[t];
        }

        public void SetAttribute(UnitAttributeProperty t, float value)
        {
            Attributes[t] = value;
            if(OnChange != null)
                OnChange(t, value);
        }

        public PlayerUnitAttributes()
        {
            foreach (var v in Enum.GetValues(typeof (UnitAttributeProperty)))
            {
                Attributes.Add((UnitAttributeProperty) v,0);
            }
        }

        private Dictionary<UnitAttributeProperty, float> Attributes = new Dictionary<UnitAttributeProperty, float>();
        

        public float CurrentHealth
        {
            get { return _currentHealth; }
            set { _currentHealth = value; }
        }

        public float CurrentEnergy { get; set; }
        public string Combatlevel { get; set; }

        public IEnumerator GetEnumerator()
        {
            return Attributes.GetEnumerator();
        }
    }
}