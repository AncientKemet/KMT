using System;
using System.Collections.Generic;
using System.Reflection;
using Code.Code.Libaries.Net;
using UnityEngine;

namespace Libaries.Net
{
    public static class PacketManager
    {
        public static Dictionary<int, Type> TCPpacketTypes = new Dictionary<int, Type>();
        public static Dictionary<int, Type> UDPpacketTypes = new Dictionary<int, Type>();

        private static Type _lastPackeType;

        private static bool _packetsWereLoaded = false;

        /// <summary>
        /// This method will return you a type of packet by OPCODE.
        /// It will also traverse assembly to find all extensions of BasePacket, then storing it into dictionary<opcode, type>.
        /// </summary>
        /// <param name="opcode"> OPCODE of packet </param>
        /// <returns>Type of packet with the specified OPCODE.</returns>
        public static BasePacket PacketForOpcode(int opcode)
        {
            if (!_packetsWereLoaded)
            {
                LoadPackets();
            }
            BasePacket packetInstance;
            try
            {
                packetInstance = (BasePacket) Activator.CreateInstance(TCPpacketTypes[opcode]);
            }
            catch (KeyNotFoundException e)
            {
                Debug.LogError("Corrupted packet: "+_lastPackeType + " cause we've recievied opcode: "+opcode);
                return null;
            }
            _lastPackeType = packetInstance.GetType();
            return packetInstance;

        }

        private static void LoadPackets()
        {
            _packetsWereLoaded = true;

            Type basePacketType = typeof(BasePacket);
            foreach (var type in Assembly.GetAssembly(typeof(BasePacket)).GetTypes())
            {
                int _opcode;
                if (basePacketType.IsAssignableFrom(type) && type != basePacketType && !type.IsAbstract)
                {
                    object instance = Activator.CreateInstance(type);
                    var methodInfo = type.GetMethod("OPCODE");
                    _opcode = (int)(
                        methodInfo.
                            Invoke(
                                instance,
                                new object[0]));
                    TCPpacketTypes[_opcode] = type;
                }
            }

            Type udpPacketType = typeof(DatagramPacket);
            foreach (var type in Assembly.GetAssembly(typeof(DatagramPacket)).GetTypes())
            {
                int _port;
                if (udpPacketType.IsAssignableFrom(type) && type != udpPacketType && !type.IsAbstract)
                {
                    object instance = Activator.CreateInstance(type);
                    var methodInfo = type.GetMethod("get_Port");
                    _port = (int)(
                        methodInfo.
                            Invoke(
                                instance,
                                new object[0]));
                    UDPpacketTypes[_port] = type;
                }
            }
        }

        public static DatagramPacket UDPPacketForPort(int port)
        {
            if (!_packetsWereLoaded)
            {
                LoadPackets();
            }
            DatagramPacket packetInstance;
            try
            {
                packetInstance = (DatagramPacket)Activator.CreateInstance(UDPpacketTypes[port]);
            }
            catch (KeyNotFoundException e)
            {
                Debug.LogError("Corrupted packet: " + _lastPackeType);
                return null;
            }
            return packetInstance;
        }
    }
}