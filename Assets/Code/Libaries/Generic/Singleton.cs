namespace Code.Libaries.Generic
{
    public class Singleton<T> : LilSingleton where T : LilSingleton, new()
    {

        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new T();
                }
                return _instance;
            }
        }
    }

    public class LilSingleton
    {
        public LilSingleton ()
        { OnInstanceWasCreated(); }

        protected virtual void OnInstanceWasCreated()
        {
        }
    }

}
