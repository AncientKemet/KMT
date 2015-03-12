using Client.Net;
using Code.Core.Client.Controls;
using Code.Core.Client.UI.Controls;
using Code.Libaries.Net.Packets.ForServer;
using Code.Libaries.UnityExtensions.Independent;
using Libaries.UnityExtensions.Independent;
using UnityEngine;

namespace Client.UI.Controls
{
    public class CircleButton : InterfaceButton
    {
        public KeyCode HotKey = KeyCode.Return;

        public tk2dSprite BackGround, Icon;

        private bool _isRotating = false;

        [SerializeField]
        private bool _canBeHeldDown = false;

        protected override void Start()
        {
            base.Start();

            if (_canBeHeldDown)
            {
                OnLeftDown += ScaleDown;
                OnLeftDown += SendButtonDown;
                OnLeftClick += ScaleBack;
                OnLeftClick += () => SendClickPacket("");
            }
            else
            {
                OnLeftClick += ScaleDownAndBack;
            }
            OnMouseIn += Highlight;
            OnMouseOff += Dehighlight;
        }

        private void SendButtonDown()
        {
            UIInterfaceEvent p = new UIInterfaceEvent();

            p.interfaceId = InterfaceId;
            p.controlID = Index;

            p._eventType = UIInterfaceEvent.EventType.Button_Down;

            ClientCommunicator.Instance.SendToServer(p);
        }

        protected virtual void Update()
        {
            if (Input.GetKeyDown(HotKey) && KeyboardInput.Instance.FullListener == null)
            {
                if (_canBeHeldDown)
                {
                    if (OnLeftDown != null)
                    {
                        OnLeftDown();
                    }
                }
                else if (OnLeftClick != null) OnLeftClick();
            }

            if (Input.GetKeyUp(HotKey) && KeyboardInput.Instance.FullListener == null)
            {
                if (_canBeHeldDown)
                {
                    if (OnLeftClick != null) OnLeftClick();
                }
            }
        }

        private void Dehighlight()
        {
        }

        private void Highlight()
        {
        }

        private void ScaleDownAndBack()
        {
            CorotineManager.Instance.StartCoroutine(
                Ease.Vector(
                    transform.localScale,
                    Vector3.one * 0.7f,
                    delegate(Vector3 vector3) { transform.localScale = vector3; },
                    delegate
                    {
                        CorotineManager.Instance.StartCoroutine(
                            Ease.Vector(
                                transform.localScale,
                                Vector3.one,
                                delegate(Vector3 vector3) { transform.localScale = vector3; },
                                delegate { },
                                0.15f
                                )
                            );
                    },
                    0.15f
                    )
                );
        }

        private void ScaleDown()
        {
            CorotineManager.Instance.StartCoroutine(
                Ease.Vector(
                    transform.localScale,
                    Vector3.one * 0.7f,
                    delegate(Vector3 vector3) { transform.localScale = vector3; },
                    delegate
                    {
                    },
                    0.15f
                    )
                );
        }

        private void ScaleBack()
        {
            CorotineManager.Instance.StartCoroutine(
                Ease.Vector(
                    transform.localScale,
                    Vector3.one,
                    delegate(Vector3 vector3) { transform.localScale = vector3; },
                    delegate
                    {
                    },
                    0.15f
                    )
                );
        }
    }
}
