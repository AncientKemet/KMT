using System;
using System.Collections;
using System.Collections.Generic;
using Code.Libaries.Generic;
using UnityEngine;

namespace Code.Libaries.UnityExtensions.Independent
{
    public class CorotineManager : MonoSingleton<CorotineManager>
    {

        public void RunCoroutine(IEnumerator enumrator)
        {
            StartCoroutine(enumrator);
        }

        public void RunCoroutine(Action enumrator, float f)
        {
            RunCoroutine(delayedAction(enumrator, f));
        }

        IEnumerator delayedAction(Action a, float time)
        {
            yield return new WaitForSeconds(time);
            try
            {
                a();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }

}
