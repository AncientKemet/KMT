using System.Collections.Generic;
using System.Linq;
using Client.UI.Controls;
using Client.UI.Scripts;
using Code.Core.Client.UI.Controls;
using UnityEngine;

namespace Client.UI.Interfaces
{
    public class RightClickMenu : UIInterface<RightClickMenu>
    {
        /// <summary>
        /// So the menu doesnt apper exatcly at the mouse point. But a bit off, so it doesnt instanly close.
        /// </summary>
        private static Vector3 OFFSET = new Vector3(-0.5f, 0.5f);

        public static RightClickMenu Open(Clickable clickable)
        {
            RightClickMenu menu = I;
            menu.Setup(clickable);
            return menu;
        }

        [SerializeField]
        private TextButton Button;

        [SerializeField]
        private tk2dSlicedSprite _backGround;

        [SerializeField]
        private float _buttonHeightRatio = 1f;
        [SerializeField]
        private float _buttonWidthRatio = 1f;

        private bool _opened = false;
        private Vector3 buttonOffset;

        private List<TextButton> buttons = new List<TextButton>(); 

        private void Setup(Clickable clickable)
        {
            if(_opened)
                Close();

            Vector3 wp = tk2dUIManager.Instance.camera.ScreenToWorldPoint(Input.mousePosition);

            Vector3 pos = new Vector3(wp.x, wp.y, clickable.transform.position.z - 50) + OFFSET;

            
            
            int buttonIndex = 0;
            foreach (RightClickAction action in clickable.Actions)
            {
                AddButton(action, buttonIndex);
                buttonIndex++;
            }

            //find the widest button
            float maxWidth = buttons.Select(button => button.Width).Concat(new float[] {0}).Max();

            foreach (var button in buttons)
            {
                button.Width = maxWidth;
                button.GetComponent<Clickable>().OnLeftClick += Close;
            }

            //fit the background size
            _backGround.renderer.enabled = true;
            _backGround.dimensions = new Vector2(20 + maxWidth * _buttonWidthRatio, 16 + _buttonHeightRatio * buttons.Count) * 2;
            _backGround.ForceBuild();

            pos.x = Mathf.Clamp(pos.x,
                tk2dUIManager.Instance.camera.ViewportToWorldPoint(new Vector2(0, 0)).x,
                tk2dUIManager.Instance.camera.ViewportToWorldPoint(new Vector2(1, 0)).x - _backGround.renderer.bounds.size.x);
            pos.y = Mathf.Clamp(pos.y,
                tk2dUIManager.Instance.camera.ViewportToWorldPoint(new Vector2(0, 0)).y + _backGround.renderer.bounds.size.y,
                tk2dUIManager.Instance.camera.ViewportToWorldPoint(new Vector2(1, 1)).y );

            transform.position = pos;

            _opened = true;
        }

        private void Close()
        {
            if (_opened)
            {
                foreach (var button in buttons)
                {
                    Destroy(button.gameObject);
                }

                buttons.Clear();
                _backGround.renderer.enabled = false;

                _opened = false;
            }
        }

        private void AddButton(RightClickAction action, int buttonIndex)
        {
            TextButton button = ((GameObject) Instantiate(Button.gameObject)).GetComponent<TextButton>();

            button.gameObject.SetActive(true);

            button.Text = action.Name;
            button.GetComponent<Clickable>().OnLeftClick += action.Action;
            button.transform.parent = transform;
            button.transform.localPosition = new Vector3(0, -buttonIndex, -1) + buttonOffset;

            /*if (action.Action != null)
                button.GetComponent<Clickable>().OnLeftClick += action.Action;*/

            buttons.Add(button);
        }

        protected override void OnStart()
        {
            base.OnStart();
            Button.gameObject.SetActive(false);
            buttonOffset = Button.transform.localPosition;
        }

        protected virtual void LateUpdate()
        {
            if(_opened)
            if(!_backGround.renderer.bounds.IntersectRay(tk2dUIManager.Instance.UICamera.ScreenPointToRay(Input.mousePosition)))
                Close();
        }
    }
}
