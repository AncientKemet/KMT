using System;
using Client.Net;
using Code.Core.Client.Controls.Camera;
using Code.Core.Shared.NET;
using Code.Libaries.Generic;
using Code.Libaries.Net.Packets.ForServer;
using UnityEngine;

namespace Code.Core.Client.Controls
{
    public class KeyboardInput : MonoSingleton<KeyboardInput> {

        private KeyboardImputListener _fullListener;

        public KeyboardImputListener FullListener
        {
            get
            {
                return _fullListener;
            }
            set
            {
                if(_fullListener != null && _fullListener != value)
                {
                    _fullListener.ListenerWasDeclined();
                }
                _fullListener = value;
            }
        }

        void Update ()
        {
            try
            {
                if (_fullListener == null)
                {
                    bool rotateLeft = Input.GetKey(KeyCode.A);
                    bool rotateRight = Input.GetKey(KeyCode.S);
                    bool dontWalk = Input.GetKeyDown(KeyCode.LeftAlt);
                    bool canWak = Input.GetKeyUp(KeyCode.LeftAlt);
                    bool jump = Input.GetKey(KeyCode.Space);

                    if (rotateLeft)
                        CameraController.Instance.rotation += 1.5f*Time.deltaTime;

                    if (rotateRight)
                        CameraController.Instance.rotation -= 1.5f*Time.deltaTime;

                    if (dontWalk)
                        ClientCommunicator.Instance.SendToServer(new InputEventPacket(PacketEnums.INPUT_TYPES.StopWalk));

                    if (jump)
                        ClientCommunicator.Instance.SendToServer(new InputEventPacket(PacketEnums.INPUT_TYPES.Jump));
                    
                }
                else
                {
                    foreach (var c in Input.inputString.ToCharArray())
                    {
                        try
                        {
                            _fullListener.KeyWasPressed(c);
                        }
                        catch (MissingReferenceException e)
                        {
                            Debug.LogException(e);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public abstract class KeyboardImputListener
        {
            public void Attach()
            {
                Instance.FullListener = this;
            }
        
            public void Deattach()
            {
                if(!IsNull)
                if(Instance.FullListener == this)
                {
                    Instance.FullListener = null;
                }
            }
        
            public abstract void KeyWasPressed(char k);
            public abstract void ListenerWasDeclined();
        }
    }
}

