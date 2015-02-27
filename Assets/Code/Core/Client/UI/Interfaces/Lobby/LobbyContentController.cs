using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client.UI.Interfaces.Lobby
{
    public class LobbyContentController : MonoBehaviour
    {
        [SerializeField]
        private List<LobbyPage> _lobbyPages = new List<LobbyPage>();

        public void LoadPage(int id)
        {
            StartCoroutine(DoLoadAction(() =>
            {
                for (int i = 0; i < _lobbyPages.Count; i++)
                {
                    _lobbyPages[i].gameObject.SetActive(i == id);
                }
            }));
        }

        private void Start()
        {
            _lobbyPages.AddRange(GetComponentsInChildren<LobbyPage>(true));
            for (int i = 0; i < _lobbyPages.Count; i++)
            {
                _lobbyPages[i].gameObject.SetActive(false);
            }
            LoadPage(0);
        }

        private IEnumerator DoLoadAction(Action action)
        {
            if (animation.isPlaying)
                yield return WaitForAnimation(animation);
            animation.Play();
            yield return new WaitForSeconds(0.4f);
            yield return new WaitForEndOfFrame();
            if(action != null)
                action();
        }

        private IEnumerator WaitForAnimation(Animation animation)
        {
            do
            {
                yield return null;
            } while (animation.isPlaying);
        }
    }
}
