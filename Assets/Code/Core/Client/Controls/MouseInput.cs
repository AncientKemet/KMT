using System.Collections.Generic;
using Code.Libaries.Generic;
using UnityEngine;

namespace Client.Controls
{
    public class MouseInput : MonoSingleton<MouseInput>
    {

        public Dictionary<string, Texture2D> Cursors = new Dictionary<string, Texture2D>();

        private void Start()
        {
            var a = Resources.LoadAll<Texture2D>("Cursors");
            foreach (var t in a)
            {
                Cursors.Add(t.name, t);
            }
            SetCursorInactive();
        }

        public void SetCursorActive(string cursorName)
        {
            if (Cursors.ContainsKey("Cursor-" + cursorName + "-0"))
                Cursor.SetCursor(Cursors["Cursor-" + cursorName + "-0"], Vector2.zero, CursorMode.Auto);
            else
                Cursor.SetCursor(Cursors["Cursor-default-0-d"], Vector2.zero, CursorMode.Auto);
        }

        public void SetCursorInactive()
        {
            Cursor.SetCursor(Cursors["Cursor-default-0"], Vector2.zero, CursorMode.Auto);
        }
    }
}