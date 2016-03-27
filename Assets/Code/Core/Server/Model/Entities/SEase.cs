using System;
using System.Collections;
using UnityEngine;

namespace Server.Model.Entities
{
    public static class SEase {

        public static IEnumerator Action(Action _action, int repeat = -1, float delay = -1f)
        {
            if (repeat == -1)
            {
                if (delay > 0f)
                    yield return new WaitForSeconds(delay);
                else
                    yield return new WaitForFixedUpdate();
                _action();
            }
            else
            {
                for (int i = 0; i < repeat; i++)
                {
                    if (delay > 0f)
                        yield return new WaitForSeconds(delay);
                    else
                        yield return new WaitForFixedUpdate();
                    _action();
                }
            }
        }

    }
}
