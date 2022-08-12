using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace TinyCacto.Utils
{
    public static class TinyTasks
    {
        /// <summary>
        /// Blocks while condition is true or timeout occurs.
        /// <para>To use along async method.</para>
        /// </summary>
        /// <param name="condition">The condition that will perpetuate the block.</param>
        /// <param name="frequency">The frequency at which the condition will be check, in milliseconds.</param>
        /// <exception cref="TimeoutException"></exception>
        /// <returns></returns>
        public static void WaitWhile(MonoBehaviour routineHolder, Func<bool> condition, int frequency, Action callback)
        {

            routineHolder.StartCoroutine(WaitWhileRoutine());

            IEnumerator WaitWhileRoutine()
            {
                while (!condition())
                {
                    yield return new WaitForSeconds(frequency);
                }

                callback?.Invoke();
            }
        }

        /// <summary>
        /// Wait a set seconds and then call an action.
        /// </summary>
        public static void WaitThenCall(MonoBehaviour routineHolder, int seconds, Action callback)
        {

            routineHolder.StartCoroutine(WaitThenCall());

            IEnumerator WaitThenCall()
            {

                yield return new WaitForSeconds(seconds);


                callback?.Invoke();
            }
        }
    }
}
