
using System;
using System.Collections.Generic;
using Client.UI.Interfaces.StatsBar;
using Client.UI.Scripts;
using Client.Units;
using Code.Core.Client.UI.Controls;
using Shared.Content.Types;
using UnityEngine;

namespace Code.Core.Client.UI.Interfaces
{
    public class StatsBarInterfaces : UIInterface<StatsBarInterfaces>
    {

        public ChannelBar HPBar, ENBar;
        public tk2dTextMesh HPLabel, ENLabel, HPRegen, ENRegen;

        public BuffControl BuffInstance;

        private List<BuffControl> BuffControls = new List<BuffControl>();

        private bool _hasInjectedMyPlayer;

        protected override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            if (PlayerUnit.MyPlayerUnit != null)
            {
                HPLabel.text = PlayerUnit.MyPlayerUnit.PlayerUnitAttributes.CurrentHealth + " / " + PlayerUnit.MyPlayerUnit.PlayerUnitAttributes.GetAttribute(UnitAttributeProperty.Health);
                ENLabel.text = PlayerUnit.MyPlayerUnit.PlayerUnitAttributes.CurrentEnergy + " / " + PlayerUnit.MyPlayerUnit.PlayerUnitAttributes.GetAttribute(UnitAttributeProperty.Energy);
                HPRegen.text = (PlayerUnit.MyPlayerUnit.PlayerUnitAttributes.GetAttribute(UnitAttributeProperty.HealthRegen) > 0 ? "+" : "") + "" +
                               (float) ((int) (PlayerUnit.MyPlayerUnit.PlayerUnitAttributes.GetAttribute(UnitAttributeProperty.HealthRegen)*10f))/10f;
                ENRegen.text = (PlayerUnit.MyPlayerUnit.PlayerUnitAttributes.GetAttribute(UnitAttributeProperty.EnergyRegen) > 0 ? "+" : "") + "" +
                               (float)((int)(PlayerUnit.MyPlayerUnit.PlayerUnitAttributes.GetAttribute(UnitAttributeProperty.EnergyRegen) * 10f)) / 10f;

                HPBar.Progress = PlayerUnit.MyPlayerUnit.PlayerUnitAttributes.CurrentHealth /
                                 PlayerUnit.MyPlayerUnit.PlayerUnitAttributes.GetAttribute(UnitAttributeProperty.Health);
                ENBar.Progress = PlayerUnit.MyPlayerUnit.PlayerUnitAttributes.CurrentEnergy /
                                 PlayerUnit.MyPlayerUnit.PlayerUnitAttributes.GetAttribute(UnitAttributeProperty.Energy);

                if (!_hasInjectedMyPlayer)
                {
                    _hasInjectedMyPlayer = true;
                    PlayerUnit.MyPlayerUnit.OnBuffWasAdded += instance =>
                    {
                        var n = ((GameObject)Instantiate(BuffInstance.gameObject)).GetComponent<BuffControl>();
                        n.Buff = instance;
                        n.gameObject.SetActive(true);
                        BuffControls.Add(n);
                        n.transform.parent = BuffInstance.transform.parent;
                        BuffControl.RealignBuffs(BuffInstance, BuffControls);
                    };
                    PlayerUnit.MyPlayerUnit.OnBuffWasRemoved += instance =>
                    {
                        try
                        {
                            var c = BuffControls.Find(control => control.Buff.Index == instance.Index);
                            BuffControls.Remove(c);
                            Destroy(c.gameObject);
                            BuffControl.RealignBuffs(BuffInstance, BuffControls);
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                        }
                    };

                }
            }
            else
            {
                _hasInjectedMyPlayer = false;
            }
        }
    }
}

