using System;
using System.Collections.Generic;
using Client.UI.Interfaces;
using Client.Units;
using Code.Core.Client.UI.Interfaces;
using Code.Core.Client.Units;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Code.Core.Client.UI.Controls
{
    public class Clickable : MonoBehaviour
    {
        private static RightClickAction CANCEL = new RightClickAction("Cancel");

        public Action OnLeftClick;

        public Action OnRightClick;
        public Action OnWheelClick;
        public Action OnHover;
        public Action OnMouseIn;
        public Action OnMouseOff;
        public Action OnLeftMouseHold;
        public Action OnRightMouseHold;

        public Action OnLeftDown;
        public Action OnLeftUp;

        private bool _wasLeftDown = false;

        [SerializeField]
        private bool HasRightClickMenu = false;

        public virtual List<RightClickAction> Actions
        {
            get
            {
                List<RightClickAction> list = new List<RightClickAction>();

                if (_actions.Count > 0)
                    list.AddRange(_actions);

                list.Add(CANCEL);

                return list;
            }
        }

        public bool MenuOn
        {
            get { return HasRightClickMenu; }
            set
            {
                if (value != HasRightClickMenu)
                {
                    if(value)
                        OnRightClick += OpenRightClickMenu;
                    else if(OnRightClick != null)
                        OnRightClick -= OpenRightClickMenu;
                }
                HasRightClickMenu = value; 
            }
        }

        protected virtual void Start()
        {
            //Left click is the first action
            if(!(this is PlayerUnit))
                OnLeftClick += delegate() { if (_actions.Count > 0 && _actions[0] != null && _actions[0].Action != null) _actions[0].Action(); };

            if(HasRightClickMenu)
                OnRightClick += OpenRightClickMenu;
        }
        
        protected void OpenRightClickMenu()
        {
            RightClickMenu.Open(this);
        }

        private void OnMouseOver()
        {
            if (OnHover != null)
                OnHover();

            if (Input.GetMouseButton(0))
                if (OnLeftMouseHold != null)
                    OnLeftMouseHold();

            if (Input.GetMouseButton(1))
                if (OnRightMouseHold != null)
                    OnRightMouseHold();

            if (Input.GetMouseButtonUp(0))
            {
                if (_wasLeftDown)
                {
                    _wasLeftDown = false;
                    if (OnLeftClick != null)
                        OnLeftClick();
                }
                if (OnLeftUp != null)
                    OnLeftUp();
            }

            if (Input.GetMouseButtonUp(1))
                if (OnRightClick != null)
                    OnRightClick();

            if (Input.GetMouseButtonUp(2))
                if (OnWheelClick != null)
                    OnWheelClick();

            if (Input.GetMouseButtonDown(0))
            {
                if (OnLeftDown != null)
                    OnLeftDown();
                _wasLeftDown = true;
            }
        }
    
        private void OnMouseExit()
        {
            if (OnMouseOff != null)
                OnMouseOff();
        }

        private void OnMouseEnter()
        {
            if (OnMouseIn != null)
                OnMouseIn();
        }
        
        [SerializeField]
        private List<RightClickAction> _actions = new List<RightClickAction>();

        public virtual void AddAction(RightClickAction action)
        {
            _actions.Insert(0, action);
        }

        public virtual void ClearAllActions(params string[] exceptions)
        {
            for (int j = 0; j < _actions.Count; j++)
            {
                bool match = false;
                for (int i = 0; i < exceptions.Length; i++)
                {
                    if (_actions[j].Name == exceptions[i])
                    {
                        match = true;
                        break;
                    }
                }
                if (!match)
                {
                    _actions[j] = null;
                }
            }
            _actions.RemoveAll(item => item == null);
        }

        public void RegisterChildClickable(Clickable child)
        {
            child.OnLeftClick += () => OnLeftClick();
            child.OnRightClick += () => OnRightClick();
            child.OnMouseIn += () =>
            {
                if (OnMouseIn != null)
                {
                    OnMouseIn();
                }
            };
            child.OnMouseOff += () =>
            {
                if (OnMouseOff != null)
                {
                    OnMouseOff();
                }
            };
        }
    }

    [Serializable]
    public class RightClickAction
    {
        public string Name;
        public Action Action;

        public RightClickAction()
        {}

        public RightClickAction(string name)
        {
            Name = name;
        }

        public RightClickAction(string name, Action action)
        {
            Name = name;
            Action = action;
        }
#if UNITY_EDITOR
        public void OnGUI()
        {
            Name = EditorGUILayout.TextField("Name", Name);
        }
#endif
    }
}
