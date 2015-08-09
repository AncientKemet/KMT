using System;
using System.Collections.Generic;
using Shared.Content;
using Shared.Content.Types;
using UnityEditor;
using UnityEngine;
using System.Collections;

public class KemetContentItemPopup : EditorWindow
{
    private event Action<ContentItem> Callback;

    private string Filter = "";
    private Vector2 _mainScroll = Vector2.zero;

    protected virtual void OnCallback(ContentItem obj)
    {
        var handler = Callback;
        if (handler != null) handler(obj);
    }

    private void OnGUI()
    {
        int x = 0;
        GUILayout.BeginVertical();
        Filter = GUILayout.TextField(Filter);
        if(Content == null)
            GUILayout.Label("Null content.");
        if (Content != null)
        {
            _mainScroll = EditorGUILayout.BeginScrollView(_mainScroll);
            foreach (var c in Content)
            {

                if (c is Item)
                {
                    if (!string.IsNullOrEmpty(Filter))
                    {
                        if (!(c as Item).name.Contains(Filter))
                            continue;
                    }

                    if (x == 0)
                        GUILayout.BeginHorizontal();


                    GUILayout.BeginVertical();
                    GUILayout.Label((c as Item).name);
                    if (GUILayout.Button((c as Item).Icon))
                    {
                        OnCallback((c as Item));
                        this.Close();
                    }
                    GUILayout.EndVertical();

                    if (x == 5)
                    {
                        GUILayout.EndHorizontal();
                        x = 0;
                    }
                    else
                        x++;
                }
            }
            EditorGUILayout.EndScrollView();
        }
        if (x != 0)
            GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }

    public static void DoPickItem(IEnumerable contentItems, Action<ContentItem> callback )
    {
        KemetContentItemPopup popup = EditorWindow.GetWindow(typeof(KemetContentItemPopup), true, "Select", true) as KemetContentItemPopup;
        popup.Content = contentItems;
        popup.Callback = callback;
    }

    public IEnumerable Content { get; set; }
}
