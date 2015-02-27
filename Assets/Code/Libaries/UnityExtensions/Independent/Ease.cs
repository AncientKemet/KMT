using System.Collections;
using UnityEngine;

namespace Code.Libaries.UnityExtensions.Independent
{
    public static class Ease 
    {
        public enum EaseType
        {
            InOut
        }

        public static IEnumerator Vector(Vector3 start, Vector3 end, System.Action<Vector3> onUpdate, System.Action onFinish, float time)
        {
            float startTime = Time.realtimeSinceStartup;

            float t = 0.001f;

            while (Time.realtimeSinceStartup - startTime < time)
            {
                yield return new WaitForEndOfFrame();

                t += Time.deltaTime;

                if (Time.realtimeSinceStartup - startTime < time)
                    onUpdate(start * (1f - Mathf.Max(t / time,0f)) + end * Mathf.Min((t / time),1f));
            }

            onUpdate(end);
            if(onFinish != null)
            onFinish();
        }

        public static IEnumerator Color(Color start, Color end, System.Action<Color> onUpdate, System.Action onFinish, float time)
        {
            float startTime = Time.realtimeSinceStartup;

            float t = 0.001f;

            while (Time.realtimeSinceStartup - startTime < time)
            {
                yield return new WaitForEndOfFrame();

                t += Time.deltaTime;

                if (Time.realtimeSinceStartup - startTime < time)
                    onUpdate(start * (1f - (Mathf.Clamp(t, 0, time) / time)) + end * (Mathf.Clamp(t, 0, time) / time));
            }

            onUpdate(end);
            if(onFinish != null)
            onFinish();
        }

        private static Vector3 Bezier3(Vector3 s, Vector3 st, Vector3 et, Vector3 e, float t)
        {
            return (((-s + 3*(st-et) + e)* t + (3*(s+et) - 6*st))* t + 3*(st-s))* t + s;
        }

    }
}
