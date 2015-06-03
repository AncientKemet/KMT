using System;
using System.Collections.Generic;
using Code.Code.Libaries.Net;
using UnityEngine;

namespace Libaries.Net
{
    public abstract class PacketExecutor
    {

        public Dictionary<Type, int> ExtecutedPackets = new Dictionary<Type, int>(); 

        protected abstract void aExecutePacket(BasePacket packet);
        public BasePacket LastExecutedPacket;

        public Action<BasePacket> OnPacket;
 
        public void ExecutePacket(BasePacket packet)
        {
            try
            {
                if (ExtecutedPackets.ContainsKey(packet.GetType()))
                {
                    ExtecutedPackets[packet.GetType()]++;
                }
                else
                {
                    ExtecutedPackets.Add(packet.GetType(),1);
                }
                aExecutePacket(packet);
                
                if (OnPacket != null)
                    OnPacket(packet);
                
                LastExecutedPacket = packet;
            }
            catch(Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}
