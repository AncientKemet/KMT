#if SERVER
using Libaries.Net.Packets.Data;

using Libaries.Net;

using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Code.Code.Libaries.Net;
using UnityEngine;

namespace Server.Net
{
    public class MasterServerConnection : ConnectionHandler
    {

        private readonly string _authCertificate = ServerSingleton.Instance.DataCertificate;

        // Use this for initialization
        public MasterServerConnection(Socket socket, MasterPacketExecutor packetExecutor)
            : base(socket, packetExecutor)
        {
            packetExecutor.Certificate = _authCertificate;
            _dataRequests = new Dictionary<string, Action<bool, string>>();

            // pass the reference
            packetExecutor._datarequests = _dataRequests;
        }

        private Dictionary<string, Action<bool, string>> _dataRequests;

        /// <summary>
        /// Sends an request to the data server, if the data server respones, it will run the datarequest action on the values returned.
        /// </summary>
        /// <param name="dataRequest"></param>
        public void RequestData(DataRequest dataRequest)
        {
            if (_dataRequests.ContainsKey(dataRequest.DataPath))
            {
                throw new Exception("Duplicate data path requests.");
            }
            _dataRequests.Add(dataRequest.DataPath, dataRequest.OnRecieve);

            var packet = new DataRequestPacket { Certificate = _authCertificate, DataPath = dataRequest.DataPath };
            SendPacket(packet);
            FlushOutPackets();
        }

        /// <summary>
        /// Class for local packetExectution
        /// </summary>
        public class MasterPacketExecutor : PacketExecutor
        {
            public Dictionary<string, Action<bool, string>> _datarequests;

            protected override void aExecutePacket(BasePacket packet)
            {
                if (packet is DataPacket)
                {
                    if (packet is DataResponsePacket)
                    {
                        DataResponsePacket r = packet as DataResponsePacket;

                        if (r.Certificate == Certificate)
                        {
                            Action<bool, string> action = _datarequests[r.DataPath];
                            if (action != null)
                            {
                                action(r.Success, r.Value);
                                _datarequests.Remove(r.DataPath);
                            }
                            else
                            {
                                Debug.LogError("Recieved data response, even if there was no request. data path: " + r.DataPath);
                            }
                        }
                        else
                        {
                            Debug.LogError("Invalid data certificate " + r.Certificate + " != " + Certificate);
                        }
                    }
                    else
                    {
                        Debug.LogError("Wrong packet[ " + packet.GetType() + " ] in data DataServerConnection.");
                    }
                }
                else
                {
                    Debug.LogError("Wrong packet[ " + packet.GetType() + " ] in data DataServerConnection.");
                }
            }

            public string Certificate { get; set; }
        }

        /// <summary>
        /// Class that creates an data requests.
        /// 
        /// Bool stands for success of authentication.
        /// </summary>
        public class DataRequest
        {
            public Action<bool, string> OnRecieve;
            public string DataPath;

            public DataRequest(string dataPath, Action<bool, string> onRecieve)
            {
                DataPath = dataPath;
                OnRecieve = onRecieve;
            }
        }
    }
}
#endif
