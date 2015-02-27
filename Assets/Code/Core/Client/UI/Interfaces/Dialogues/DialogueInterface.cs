using System;
using System.Collections.Generic;
using Code.Libaries.UnityExtensions.Independent;
using UnityEngine;

namespace Client.UI.Interfaces.Dialogues
{
    public abstract class DialogueInterface : MonoBehaviour
    {
        private static DialogueInterface _lastDialogueInterface;

        private static bool CanCreateNewDialogue()
        {
            return _lastDialogueInterface == null;
        }

        public static T Create<T>() where T : DialogueInterface
        {
            if (CanCreateNewDialogue())
            {
                var t = ((GameObject)Instantiate(Manager.GetDialoguePrefab<T>().gameObject)).GetComponent<T>();
                t.transform.localScale = Vector3.zero;
                _lastDialogueInterface = t;
                t.StartCoroutine(Ease.Vector(Vector3.zero, Vector3.one, vector3 => t.transform.localScale = vector3, null, 0.5f));
                return t;
            }
            throw new Exception("An dialogue already exists.");
        }

        public virtual void Continue()
        {
            Close();
        }

        public void Close()
        {
            _lastDialogueInterface = null;
            StartCoroutine(Ease.Vector(transform.localScale, Vector3.zero, vector3 => transform.localScale = vector3, () => Destroy(gameObject), 0.5f));
        }

        public class Manager
        {
            public static Dictionary<Type, DialogueInterface> PrefabDictionary = new Dictionary<Type, DialogueInterface>();
            
            public static T GetDialoguePrefab<T>() where T : DialogueInterface
            {
                // The interface doesnt exist
                if (PrefabDictionary.Count == 0)
                {
                    foreach (var o in Resources.LoadAll("Dialogues"))
                    {
                        var go = (GameObject) o;
                        if (go.GetComponent<DialogueInterface>() != null)
                        {
                            PrefabDictionary.Add(go.GetComponent<DialogueInterface>().GetType(), go.GetComponent<T>());
                        }
                    }
                }
                return PrefabDictionary[typeof (T)] as T;
            }
        }
    }
}