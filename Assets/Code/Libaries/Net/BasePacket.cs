namespace Code.Code.Libaries.Net
{
    public abstract class BasePacket
    {
        protected abstract int GetOpCode();

        protected abstract void enSerialize(ByteStream bytestream);
        protected abstract void deSerialize(ByteStream bytestream);

        public int Size { get; set; }

        public int OPCODE()
        {
            return GetOpCode();
        }

        public void Deserialize(ByteStream bytestream)
        {
            deSerialize(bytestream);
        }

        public void Serialize(ByteStream bytestream)
        {
            //add empty short for the size of packet
            bytestream.AddShort(0);

            //add opcode
            bytestream.AddByte(GetOpCode());

            //abstract serializing
            enSerialize(bytestream);

            //now we add the actual size of packet
            //set offset back
            bytestream.Offset = 0;

            //add the size, but with -2 cause its gonna be there always
            bytestream.AddShort(bytestream.Length - 2);

            //Done :)
        }
    }
}
