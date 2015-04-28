using System.Collections.Generic;
using Server;
using UnityEditor;
using UnityEngine;
using System.Collections;

[InitializeOnLoad]
class DoInEditor
{

    private static Medium medium = new Medium();

    static DoInEditor ()
    {
        //Nezapinat domco to bude manulne ukladat
        //EditorApplication.update += Update;
    }

    static void Update ()
    {
        if (!Application.isPlaying)
        {
                medium.Update();
        }
    }

    public class Medium : AUpdate
    {
        private static GameObject _mapObjects;

        private static GameObject MapObjects
        {
            get { return _mapObjects ?? (_mapObjects = new GameObject("MapObjects")); }
        }

        protected override int Rate
        {
            get { return 100; }
        }

        protected override void OnUpdate()
        {
            foreach (var o in Object.FindObjectsOfType<GameObject>())
            {
                if(o.transform.parent == null)
                switch (o.name)
                {
                    case "Cameras":
                    case "Static":
                    case "MapObjects":
                        break;
                    default:
                            o.transform.parent = MapObjects.transform;
                        break;
                }
            }
        }
    }

    #region UPDATECLASS
    public abstract class AUpdate
    {
        private int _currentUpdate = 0;
        protected abstract int Rate { get; }

        public void Update()
        {
            _currentUpdate--;
            if (_currentUpdate <= 0)
            {
                _currentUpdate = Rate;
                OnUpdate();
            }
        }

        protected abstract void OnUpdate();
    }
    #endregion
}
