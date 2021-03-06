using Libaries.IO;
using Server.Model.Content;
#if SERVER

namespace Server.Model
{
    public abstract class EntityExtension : ServerMonoBehaviour
    {

        private WorldEntity _entity;

        public WorldEntity entity 
        {
            get
            {
                return _entity;
            }
        }

        private void Awake()
        {
            _entity = GetComponent<WorldEntity>();
            OnExtensionWasAdded();   
        }

        protected virtual void OnExtensionWasAdded()
        {

        }
        
        public abstract void Progress(float time);

        public virtual void Serialize(JSONObject j)
        {
        }

        public virtual void Deserialize(JSONObject j)
        {
        }

        public virtual int Priority { get { return 1; } }
    }
}
#endif
