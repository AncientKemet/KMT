using System.Collections;
using System.Collections.Generic;
using Client.Net;
using Client.UI.Scripts;
using Client.Units;
using Code.Code.Libaries.Net.Packets;
using Code.Core.Client.UI.Controls;
using Code.Core.Client.Units;
using Code.Libaries.Net.Packets.InGame;
using Code.Libaries.UnityExtensions.Independent;
using Libaries.Net.Packets.ForClient;
using Shared.Content.Types;
using UnityEngine;

namespace Code.Code.Libaries.Net.Packets
{
}

namespace Code.Core.Client.UI.Interfaces.UpperLeft
{
    public class UnitSelectionInterface : UIInterface<UnitSelectionInterface>
    {
        public System.Action<PlayerUnit> OnUnitWasSelected;

        [SerializeField]
        private List<Animation> Animations = new List<Animation>();

        private PlayerUnit _unit;

        [SerializeField]
        private tk2dTextMesh UnitNameLabel, Armor, MagicResist, MovementSpeed, CooldownSpeed;

        [SerializeField]
        private ChannelBar _healthBar, _energyBar;

        [SerializeField]
        private GameObject HPWing, AttributesPanel, DescriptionWing;

        protected override float AnimSpeed
        {
            get { return 0.2f; }
        }

        public PlayerUnit Unit
        {
            get { return _unit; }
            set
            {
                if (_unit != null && _unit != value)
                {
                    _unit.Projector.gameObject.SetActive(false);
                }

                if (value != null)
                    value.Projector.gameObject.SetActive(true);

                if (OnUnitWasSelected == null)
                    OnUnitWasSelected = ShowUnit;

                OnUnitWasSelected(value);

                _unit = value;
            }
        }

        void Start()
        {
            Unit = null;
        }

        private void ShowUnit(PlayerUnit u)
        {
            if (Unit != u)
            {
                var packet = new TargetUpdatePacket();
                packet.UnitId = u == null ? -1 : u.Id;
                ClientCommunicator.Instance.SendToServer(packet);
            }
            if (u == null)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        protected override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

            if (_unit == null) Unit = _unit;

            if (Unit == null) return;
            _healthBar.Progress = Unit.Health / Unit.MaxHealth;
            _energyBar.Progress = Unit.Energy / Unit.MaxEnergy;
        }

        public void OnDataRecieved(UnitSelectionPacketData data)
        {
            AttributesPanel.SetActive(data.HasAttributes);
            HPWing.SetActive(data.HasCombat);

            SetText(UnitNameLabel, data.UnitName);
            if (data.Attributes != null)
            {
                try
                {
                    SetText(Armor, data.Attributes[UnitAttributeProperty.Armor]);
                }
                catch (KeyNotFoundException e)
                {
                }
                try
                {
                    SetText(MagicResist, data.Attributes[UnitAttributeProperty.MagicResist]);
                }
                catch (KeyNotFoundException e)
                {
                }

                try
                {
                    SetText(MovementSpeed, data.Attributes[UnitAttributeProperty.MovementSpeed]);
                }
                catch (KeyNotFoundException e)
                {
                }
                try
                {
                    SetText(CooldownSpeed, data.Attributes[UnitAttributeProperty.ChargeSpeed]);
                }
                catch (KeyNotFoundException e)
                {
                }
            }
            if (Unit != null)
                Unit.Name = data.UnitName;
        }

        private void SetText(tk2dTextMesh mesh, string s)
        {
            mesh.text = s;
            mesh.ForceBuild();
        }

        private void SetText(tk2dTextMesh mesh, float s)
        {
            mesh.text = "" + s;
            mesh.ForceBuild();
        }

        public override void Hide()
        {
            Visible = false;
            CorotineManager.Instance.StartCoroutine(
                Ease.Vector(
                    transform.localScale,
                    Vector3.zero,
                    delegate(Vector3 vector3)
                    {
                        if (!Visible)
                            transform.localScale = vector3;
                    },
                    delegate
                    {
                        if (!Visible)
                            gameObject.SetActive(false);
                        if (Unit != null)
                        {
                            Unit = null;
                        }

                    },
                    AnimSpeed
                    )
                );

        }

        public override void Show()
        {
            Visible = true;
            gameObject.SetActive(true);

            StopAllCoroutines();
            foreach (var a in Animations)
            {
                if (a.name == "Wing")
                {
                    a.gameObject.SetActive(false);
                    a.Stop();
                }
            }

            StartCoroutine(PlayAnimations());

            CorotineManager.Instance.StartCoroutine(
                Ease.Vector(
                    transform.localScale,
                    Vector3.one,
                    delegate(Vector3 vector3)
                    {
                        if (Visible)
                            transform.localScale = vector3;
                    },
                    delegate
                    {

                    },
                    AnimSpeed
                    )
                );
        }

        private IEnumerator PlayAnimations()
        {
            foreach (var a in Animations)
            {
                a.gameObject.SetActive(true);
                a.Play();
                yield return new WaitForSeconds(a.clip.length * 0.5f);
            }
        }
    }
}
