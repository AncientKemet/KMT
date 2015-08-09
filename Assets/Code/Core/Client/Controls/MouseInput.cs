using System.Collections.Generic;
using Code.Libaries.Generic;
using UnityEngine;

namespace Client.Controls
{
    public class MouseInput : MonoSingleton<MouseInput>
    {

        public Dictionary<string, Texture2D> Cursors = new Dictionary<string, Texture2D>();

        private float _frame = 0;
        private string _cursorName = "default";

        private void Start()
        {
            var a = Resources.LoadAll<Texture2D>("Cursors");
            foreach (var t in a)
            {
                Cursors.Add(t.name, t);
            }
            SetCursorInactive();
        }

        private void Update()
        {
            _frame += 0.25f;
            if (Cursors.ContainsKey("Cursor-" + _cursorName + "-" + (int)_frame))
                Cursor.SetCursor(Cursors["Cursor-" + _cursorName + "-" + (int)_frame], Vector2.zero, CursorMode.Auto);
            else _frame = 0;
        }

        public void SetCursorActive(string cursorName)
        {
            _cursorName = cursorName;
            _frame = 0;
            if (Cursors.ContainsKey("Cursor-" + cursorName + "-0"))
                Cursor.SetCursor(Cursors["Cursor-" + cursorName + "-0"], Vector2.zero, CursorMode.Auto);
            else
                Cursor.SetCursor(Cursors["Cursor-default-0-d"], Vector2.zero, CursorMode.Auto);
        }

        public void SetCursorInactive()
        {
            _cursorName = "default";
            _frame = 0;
            Cursor.SetCursor(Cursors["Cursor-default-0"], Vector2.zero, CursorMode.Auto);
        }
    }
}