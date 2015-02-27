using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Code.Code.Libaries.Net;
using UnityEngine;

namespace Libaries.Net.Packets.Data
{
    public class DataServerConnection : ConnectionHandler
    {
        /// <summary>
        /// Certificates
        /// </summary>
#if SERVER
        public static readonly string WRITE = "$@#^&%$";
#endif
        public static readonly string READ = "47a77";


        // Use this for initialization
        public DataServerConnection(Socket socket, DataPacketExecutor packetExecutor)
            : base(socket, packetExecutor)
        {
            packetExecutor.Certificate = READ;
#if SERVER
            packetExecutor.Certificate += WRITE;
#endif
            _dataRequests = new Dictionary<int, Action<bool, string>>();

            // pass the reference
            packetExecutor._datarequests = _dataRequests;
        }

        private Dictionary<int, Action<bool, string>> _dataRequests;

        /// <summary>
        /// Sends an request to the data server, if the data server respones, it will run the datarequest action on the values returned.
        /// </summary>
        /// <param name="dataRequest"></param>
        public void RequestData(DataRequest dataRequest)
        {
            if (_dataRequests.ContainsKey(dataRequest.RequestId))
            {
                throw new Exception("Duplicate data requests.");
            }
            _dataRequests.Add(dataRequest.RequestId, dataRequest.OnRecieve);

            var packet = new DataRequestPacket { Certificate = READ, DataPath = dataRequest.DataPath, ID = dataRequest.RequestId};
            SendPacket(packet);
            FlushOutPackets();
        }

        public void RequestData(string dataPath, Action<bool, string> action )
        {
            DataRequest request = new DataRequest(dataPath, action);
            RequestData(request);
        }

#if SERVER
        public void AppendData(string dataPath, string data)
        {
            DataAppendPacket packet = new DataAppendPacket();

            packet.Certificate = WRITE;
            packet.DataPath = dataPath;
            packet.Data = data;

            SendPacket(packet);

            FlushOutPackets();
        }

        public void SetData(string dataPath, string data)
        {
            DataSetPacket packet = new DataSetPacket();

            packet.Certificate = WRITE;
            packet.DataPath = dataPath;
            packet.Data = data;

            SendPacket(packet);

            FlushOutPackets();
        }

        public void ReplaceData(string dataPath, string old, string _new)
        {
            var packet = new DataReplacePacket();

            packet.Certificate = WRITE;
            packet.DataPath = dataPath;
            packet.OldValue = old;
            packet.NewValue = _new;

            SendPacket(packet);

            FlushOutPackets();
        }
#endif

        /// <summary>
        /// Class for local packetExectution
        /// </summary>
        public class DataPacketExecutor : PacketExecutor
        {
            public Dictionary<int, Action<bool, string>> _datarequests;

            protected override void aExecutePacket(BasePacket packet)
            {
                if (packet is DataPacket)
                {
                    if (packet is DataResponsePacket)
                    {
                        DataResponsePacket r = packet as DataResponsePacket;

                        Action<bool, string> action = _datarequests[r.ID];
                        if (action != null)
                        {
                            action(r.Success, r.Value);
                            _datarequests.Remove(r.ID);
                        }
                        else
                        {
                            Debug.LogError("Recieved data response, even if there was no request. data path: " + r.DataPath);
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

            private static int  REQUEST_COUNTER = 0;
            public Action<bool, string> OnRecieve;
            public string DataPath;

            public int RequestId;

            public DataRequest(string dataPath, Action<bool, string> onRecieve)
            {
                RequestId = REQUEST_COUNTER++;
                DataPath = dataPath;
                OnRecieve = onRecieve;
            }
        }

    }
}
