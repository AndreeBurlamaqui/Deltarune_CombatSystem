using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TinyCacto.Effects
{
    public static class TinyEffects
    {

        #region GHOST TRAIL EFFECT

        /// <summary>
        /// Spawn a ghost trail effect of only the sprite renderer
        /// </summary>
        /// <param name="routineHolder">Who's going to handle the routine.
        /// Don't destroy the holder script while the routine is happening.</param>
        /// <param name="interval">Interval in seconds for a new ghost to be spawned.</param>
        /// <param name="duration">How long will ghosts keep spawning.</param>
        /// <param name="fadeSpeed">How fast will the ghost disappear in a fade out effect.</param>
        public static Coroutine SpawnGhostTrail(this SpriteRenderer s, MonoBehaviour routineHolder, float interval, float duration, float fadeSpeed)
        {
            if (s == null)
                return null;

            if (s.gameObject == null)
                return null;

            if (routineHolder == null)
                return null;

            return routineHolder.StartCoroutine(RoutineTrail());

            IEnumerator RoutineTrail()
            {
                GameObject visualGO = s.gameObject;
                var time = 0f;
                var startTime = Time.time;
                while (Time.time - startTime < duration)
                {
                    if (time + interval < Time.time - startTime)
                    {
                        if (visualGO == null)
                        {
                            break;
                        }

                        GameObject go = Instantiate(visualGO, s.transform.position);
                        go.GetComponent<SpriteRenderer>().DOFade(0, fadeSpeed).From(1).OnComplete(() => Destroy(go));

                        if (go.TryGetComponent(out Animator anim))
                        {
                            // Remove the animator
                            Destroy(anim);
                        }

                        time = Time.time - startTime;
                    }

                    yield return new WaitForSeconds(Time.deltaTime);
                }
            }

        }

        /// <summary>
        /// Spawn a ghost trail effect of only the sprite renderer
        /// </summary>
        /// <param name="routineHolder">Who's going to handle the routine.
        /// Don't destroy the holder script while the routine is happening.</param>
        /// <param name="interval">Interval in seconds for a new ghost to be spawned.</param>
        /// <param name="duration">How long will ghosts keep spawning.</param>
        /// <param name="fadeSpeed">How fast will the ghost disappear in a fade out effect.</param>
        public static Coroutine SpawnGhostTrail(this Image i, MonoBehaviour routineHolder, float interval, float duration, float fadeSpeed)
        {
            if (i == null)
                return null;

            if (i.gameObject == null)
                return null;

            if (routineHolder == null)
                return null;

            return routineHolder.StartCoroutine(RoutineTrail());

            IEnumerator RoutineTrail()
            {
                GameObject visualGO = i.gameObject;
                var time = 0f;
                var startTime = Time.time;
                while (Time.time - startTime < duration)
                {
                    if (time + interval < Time.time - startTime)
                    {
                        if (visualGO == null)
                        {
                            break;
                        }

                        GameObject go = Instantiate(visualGO, i.transform.position, i.transform.parent);
                        go.GetComponent<Image>().DOFade(0, fadeSpeed).From(1).OnComplete(() => Destroy(go));


                        time = Time.time - startTime;
                    }

                    yield return new WaitForSeconds(Time.deltaTime);
                }
            }

        }

        public static void StopGhostTrail(this MonoBehaviour routineHolder, Coroutine c)
        {
            routineHolder.StopCoroutine(c);
        }

        #endregion

        #region HELPERS

        internal static void Destroy(Object o) => Object.Destroy(o);
        internal static GameObject Instantiate(GameObject o, Vector3 v, Transform p) => Object.Instantiate(o, v, Quaternion.identity, p);
        internal static GameObject Instantiate(GameObject o, Vector3 v) => Object.Instantiate(o, v, Quaternion.identity);

        #endregion

    }
}