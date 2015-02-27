using UnityEngine;

namespace Code.Libaries.Generic
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {

        private static T _instance;
        private static GameObject _singletons;

        public static GameObject singletons
        {
            get
            {
                if (_singletons == null)
                {
                    _singletons = GameObject.Find("Singletons");
                }
                return _singletons;
            }
        }

        private static object _lock = new object();

        private void Awake()
        {
            _instance = this as T;
            OnAwake();
        }

        protected virtual void OnAwake()
        {
            
        }

        public static bool IsNull
        {
            get { return _instance == null; }
        }

        public static T Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = (T)FindObjectOfType(typeof(T));
          
                        if (FindObjectsOfType(typeof(T)).Length > 1)
                        {
                            Debug.LogError("[Singleton] Something went really wrong " +
                                           " - there should never be more than 1 singleton!" +
                                           " Reopenning the scene might fix it.");
                            return _instance;
                        }

                        GameObject prefab = ((GameObject)Resources.Load("(singleton) "+typeof(T).Name));
                        if (prefab != null && prefab is GameObject)
                        {
                            if (prefab.GetComponent<T>() != null)
                            {
                                _instance = prefab.GetComponent<T>();
                                return _instance;
                            }
                        }
          
                        if (_instance == null)
                        {
                            GameObject singleton = new GameObject();
                            singleton.transform.parent = singletons.transform;

                            _instance = singleton.AddComponent<T>();
                            singleton.name = "(singleton) " + typeof(T).ToString();
            
                            DontDestroyOnLoad(singleton);
            
                            /*Debug.Log("[Singleton] An instance of " + typeof(T) + 
                                      " is needed in the scene, so '" + singleton +
                                      "' was created with DontDestroyOnLoad.");*/
                        } 
                    }
        
                    return _instance;
                }
            }
        }
    }
}
