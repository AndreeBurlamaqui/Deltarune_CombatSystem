using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace TinyCacto.Utils
{
    public static class TinyTasks
    {
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
