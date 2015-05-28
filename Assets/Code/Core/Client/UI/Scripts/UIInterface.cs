using System;
using System.Collections.Generic;
using Code.Core.Client.UI;
using Code.Core.Client.UI.Controls;
using Libaries.UnityExtensions.Independent;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Client.UI.Scripts
{
    public static class InterfaceManager
    {
        private static Dictionary<InterfaceType, UIInterface> ActiveInterfaces = new Dictionary<InterfaceType, UIInterface>();

        private static Dictionary<tk2dBaseSprite.Anchor, tk2dCameraAnchor> cameraAnchors;

        private static Object[] InResourcesObjects;

        public static UIInterface GetInterface(InterfaceType type)
        {
            if (!ActiveInterfaces.ContainsKey(type))
            {
                if (InResourcesObjects == null)
                {
                    InResourcesObjects = Resources.LoadAll("Interfaces");
                }
                foreach (var o in InResourcesObjects)
                {
                    if (o is GameObject)
                    {
                        GameObject go = o as GameObject;
                        if (go != null)
                        {
                            UIInterface iUiInterface = go.GetComponent<UIInterface>();
                            if (iUiInterface != null)
                            {
                                if (iUiInterface.Type == type)
                                {
                                    ActiveInterfaces[type] = ((GameObject)Object.Instantiate(go)).GetComponent<UIInterface>();
                                }
                            }
                        }
                    }
                }
            }
            return ActiveInterfaces[type];
        }

        public static T GetInterface<T>() where T : UIInterface
        {
            foreach (var uiInterface in ActiveInterfaces)
            {
                if (uiInterface.Value is T)
                {
                    return uiInterface.Value as T;
                }
            }

            // The interface doesnt exist
            if (InResourcesObjects == null)
            {
                InResourcesObjects = Resources.LoadAll("Interfaces");
            }

            foreach (var o in InResourcesObjects)
            {
                if (o is GameObject)
                {
                    GameObject go = o as GameObject;
                    UIInterface iUiInterface = go.GetComponent<UIInterface>();
                    if (iUiInterface != null)
                    {
                        if (iUiInterface is T)
                        {
                            T instance = ((GameObject)Object.Instantiate(go)).GetComponent<T>();

                            try
                            {
                                ActiveInterfaces.Add(instance.Type, instance);
                            }
                            catch (ArgumentException e)
                            {
                                Debug.Log("Broken type: " + instance.Type + " value is: " + ActiveInterfaces[instance.Type]);
                                Debug.LogException(e);
                            }
                            return instance;
                        }
                    }
                }
            }
            throw new Exception("Couldnt find interface of type: " + typeof(T));
        }


        public static Dictionary<tk2dBaseSprite.Anchor, tk2dCameraAnchor> CameraAnchors
        {
            get
            {
                if (cameraAnchors == null)
                {
                    cameraAnchors = new Dictionary<tk2dBaseSprite.Anchor, tk2dCameraAnchor>();
                    foreach (var anchor in tk2dUIManager.Instance.UICamera.GetComponentsInChildren<tk2dCameraAnchor>())
                    {
                        cameraAnchors.Add(anchor.AnchorPoint, anchor);
                    }
                }
                return cameraAnchors;
            }
        }

        /// <summary>
        /// Base class
        /// </summary>
        public abstract class UIInterface : MonoBehaviour
        {

            [SerializeField]
            public InterfaceType Type;

            public bool IsAnchored = false;
            public tk2dBaseSprite.Anchor anchor = tk2dBaseSprite.Anchor.MiddleCenter;

            public List<UIControl> Controls;

            protected virtual void Awake()
            {
                if (IsAnchored)
                {
                    transform.parent = InterfaceManager.CameraAnchors[anchor].transform;
                    transform.localPosition = Vector3.zero;
                }

                if (ActiveInterfaces.ContainsKey(Type))
                {
                    Debug.LogError("Duplicate of interfacetype: " + gameObject + " " + ActiveInterfaces[Type].gameObject);
                    return;
                }

                int counter = 0;

                Controls.Clear();

                List<UIControl> list = new List<UIControl>(new UIControl[1024]);
                foreach (var button in GetComponentsInChildren<UIControl>(true))
                {
                    list[counter] = button;

                    button.InterfaceId = Type;

                    if (button.Index == -1)
                        button.Index = counter;

                    counter++;
                }

                Controls.AddRange(list);

                /*foreach (var interfaceButton in list)
                {
                    if (Controls.Count > interfaceButton.Index)
                    {
                        Controls[interfaceButton.Index] = interfaceButton;
                    }
                    else
                    {
                        for (int i = 0; i < interfaceButton.Index+1 - Controls.Count; i++)
                        {
                            Controls.Add(null);
                        }
                        Controls[interfaceButton.Index] = interfaceButton;
                    }
                }*/
            }

            public abstract void Hide();
            public abstract void Show();

            public UIControl this[int index]
            {
                get { return Controls.Find(control => control != null && control.Index == index); }
            }
        }

        
    }



    /// <summary>
    /// Singleton like InGameInterface generic class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UIInterface<T> : InterfaceManager.UIInterface where T : InterfaceManager.UIInterface
    {

        private static T _instance;
        private bool _visible;

        public static bool IsNull
        {
            get { return _instance == null; }
        }

        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (_visible != value)
                {
                    _visible = value;
                    OnVisibiltyChanged();
                }
            }
        }

        protected virtual float AnimSpeed { get { return 0.33f; } }

        private static bool _triedToLoadFromResources;

        public static T I
        {
            get
            {
                if (_instance == null && !_triedToLoadFromResources)
                {
                    _triedToLoadFromResources = true;
                    _instance = InterfaceManager.GetInterface<T>();
                }
                return _instance;

            }
        }

        protected override void Awake()
        {
            base.Awake();

            OnStart();
        }

        public override void Hide()
        {
            Visible = false;
            StopAllCoroutines();
            StartCoroutine(
                Ease.Vector(
                    transform.localScale,
                    Vector3.zero,
                    delegate(Vector3 vector3)
                    {
                        if (this != null)
                            if (!Visible)
                                transform.localScale = vector3;
                    },
                    delegate
                    {
                        if (this != null)
                            if (!Visible)
                                gameObject.SetActive(false);
                    },
                    AnimSpeed
                    )
                );

        }

        public override void Show()
        {
            Visible = true;
            gameObject.SetActive(true);
            StopAllCoroutines();

            StartCoroutine(
                Ease.Vector(
                    transform.localScale,
                    Vector3.one,
                    delegate(Vector3 vector3)
                    {
                        if (Visible)
                            transform.localScale = vector3;
                    },
                    delegate
                    {
                    },
                    AnimSpeed
                    )
                );
        }

        private void Update()
        {
            OnUpdate();
        }

        private void FixedUpdate()
        {
            OnFixedUpdate();
        }

        private void LateUpdate()
        {
            OnLateUpdate();
        }

        protected virtual void OnStart()
        {
        }
        protected virtual void OnUpdate()
        {
        }
        protected virtual void OnFixedUpdate()
        {
        }
        protected virtual void OnLateUpdate()
        {
        }
        protected virtual void OnVisibiltyChanged()
        {
        }

        protected virtual void OnEnable()
        {
            _visible = true;
        }

        protected virtual void OnDisable()
        {
            _visible = false;
        }
    }
}
