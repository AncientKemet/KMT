using System.Collections;
using UnityEngine;

namespace Libaries.UnityExtensions.Independent
{
    public static class Ease
    {
        public enum EaseType
        {
            InOut
        }

        public static IEnumerator Vector(Vector3 start, Vector3 end, System.Action<Vector3> onUpdate, System.Action onFinish= null, float time= 0.3f)
        {
            float startTime = Time.realtimeSinceStartup;

            float t = 0.001f;

            while (t < time)
            {
                yield return new WaitForEndOfFrame();

                t += Time.deltaTime;

                if (Time.realtimeSinceStartup - startTime < time / Time.timeScale)
                    onUpdate(Vector3.Lerp(start, end, Mathf.Min((t / time), 1f)));
            }

            onUpdate(end);
            if (onFinish != null)
                onFinish();
        }

        public static IEnumerator Join(Transform start, Transform end, System.Action onFinish, float time)
        {
            float startTime = Time.realtimeSinceStartup;

            float t = 0.001f;

            while (t < time)
            {
                yield return new WaitForEndOfFrame();

                t += Time.deltaTime;

                if (Time.realtimeSinceStartup - startTime < time / Time.timeScale)
                {
                    start.position = Vector3.Lerp(start.position, end.position, Mathf.Min((t / time), 1f));
                    start.rotation = Quaternion.Lerp(start.rotation, end.rotation, Mathf.Min((t / time), 1f));
                }
            }
            
            start.position = end.position;
            start.rotation = end.rotation;

            if (onFinish != null)
                onFinish();
        }

        public static IEnumerator Color(Color start, Color end, System.Action<Color> onUpdate, System.Action onFinish, float time)
        {
            float startTime = Time.realtimeSinceStartup;

            float t = 0.001f;

            while (t < time)
            {
                yield return new WaitForEndOfFrame();

                t += Time.deltaTime * Time.timeScale;

                if (Time.realtimeSinceStartup - startTime < time / Time.timeScale)
                    onUpdate(start * (1f - (Mathf.Clamp(t, 0, time) / time)) + end * (Mathf.Clamp(t, 0, time) / time));
            }

            onUpdate(end);
            if (onFinish != null)
                onFinish();
        }

        private static Vector3 Bezier3(Vector3 s, Vector3 st, Vector3 et, Vector3 e, float t)
        {
            return (((-s + 3 * (st - et) + e) * t + (3 * (s + et) - 6 * st)) * t + 3 * (st - s)) * t + s;
        }

    }
}
