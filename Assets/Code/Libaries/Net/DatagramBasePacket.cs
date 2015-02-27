using Code.Code.Libaries.Net;

namespace Libaries.Net
{
    /// <summary>
    /// DatagramPacket
    /// 
    /// A class ment to save as much bandwith as possible, it has no OPCODE, hence it has to be sent to an special
    /// socket.
    /// </summary>
    public abstract class DatagramPacket  {

        public abstract int Port { get; }

        public abstract int Size { get; }
        
        public abstract void Deserialize(ByteStream b);
        public abstract void Execute();
        public abstract void Serialize(ByteStream b);
    }
}
