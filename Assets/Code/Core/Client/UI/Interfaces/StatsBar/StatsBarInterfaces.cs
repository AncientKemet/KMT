
using System;
using System.Collections.Generic;
using Client.UI.Interfaces.StatsBar;
using Client.UI.Scripts;
using Client.Units;
using Code.Core.Client.UI.Controls;
using Code.Core.Client.Units;
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
                HPLabel.text = PlayerUnit.MyPlayerUnit.Health + " / " + PlayerUnit.MyPlayerUnit.MaxHealth;
                ENLabel.text = PlayerUnit.MyPlayerUnit.Energy + " / " + PlayerUnit.MyPlayerUnit.MaxEnergy;
                HPRegen.text = (PlayerUnit.MyPlayerUnit.HealthRegen > 0 ? "+" : "") + "" +
                               (float) ((int) (PlayerUnit.MyPlayerUnit.HealthRegen*10f))/10f;
                ENRegen.text = (PlayerUnit.MyPlayerUnit.EnergyRegen > 0 ? "+" : "") + "" +
                               (float) ((int) (PlayerUnit.MyPlayerUnit.EnergyRegen*10f))/10f;

                if (!_hasInjectedMyPlayer)
                {
                    _hasInjectedMyPlayer = true;
                    PlayerUnit.MyPlayerUnit.OnBuffWasAdded += instance =>
                    {
                        Debug.Log("add buff : "+instance.Index);
                        var n = ((GameObject)Instantiate(BuffInstance.gameObject)).GetComponent<BuffControl>();
                        n.Buff = instance;
                        n.gameObject.SetActive(true);
                        BuffControls.Add(n);
                        n.transform.parent = BuffInstance.transform.parent;
                        BuffControl.RealignBuffs(BuffInstance, BuffControls);
                    };
                    PlayerUnit.MyPlayerUnit.OnBuffWasRemoved += instance =>
                    {
                        Debug.Log("remove buff : " + instance.Index);
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
