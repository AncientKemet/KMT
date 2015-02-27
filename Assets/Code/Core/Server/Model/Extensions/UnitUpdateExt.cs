#if SERVER
using Code.Code.Libaries.Net;

namespace Server.Model.Extensions
{
    
    public abstract class UnitUpdateExt : EntityExtension
    {

        protected bool _wasUpdate;

        public bool WasUpdate()
        {
            return _wasUpdate;
        }

        public void ResetUpdate()
        {
            _wasUpdate = false;
        }

        /// <summary>
        /// Used when composing final PlayerUnit update.
        /// 0x01 - movement
        /// 0x02 - display
        /// 0x04 - combat
        /// 0x08 - animaion
        /// 0x0F - equipment
        /// ...
        /// </summary>
        /// <returns>Byte flag</returns>
        public abstract byte UpdateFlag();

        protected abstract void pSerializeState(ByteStream packet);
        protected abstract void pSerializeUpdate(ByteStream packet);

        public override void Progress()
        {
            _wasUpdate = false;
        }

        /// <summary>
        /// Serializes current state of this extension into packet.
        /// Use: Client has never seen an object, so we need to send him DirecionVector, even if it wasn't updated recently.
        /// </summary>
        /// <param name="packet"></param>
        public void SerializeState(ByteStream packet)
        {
            pSerializeState(packet);
        }

        /// <summary>
        /// Serializes this extension lastest update into packet.
        /// </summary>
        /// <param name="packet"></param>
        public void SerializeUpdate(ByteStream packet)
        {
            pSerializeUpdate(packet);
        }

    }
}
#endif
