using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtMethods;

namespace MH
{
    /// <summary>
    /// helpers to install coroutine on other GO
    /// </summary>
    public class CoroutineBehaviour : MonoBehaviour
    {
        public static Coroutine StartCoroutine(GameObject go, IEnumerator coFunc)
        {
            var co = go.ForceGetComponent<CoroutineBehaviour>();
            return co.StartCoroutine(coFunc);
        }

        public static Coroutine StartCoroutineDelay(GameObject go, float delay, System.Action normalFunc)
        {
            var co = go.ForceGetComponent<CoroutineBehaviour>();
            return co.StartCoroutine(_DelayWrapper(delay, normalFunc));
        }

        public static Coroutine StartCoroutineDelay(GameObject go, float delay, System.Action<GameObject> normalFunc)
        {
            var co = go.ForceGetComponent<CoroutineBehaviour>();
            return co.StartCoroutine(_DelayWrapper(delay, normalFunc, go));
        }

        public static Coroutine StartCoroutineDelay(GameObject go, float delay, IEnumerator coFunc)
        {
            var co = go.ForceGetComponent<CoroutineBehaviour>();
            return co.StartCoroutine(_DelayWrapper(delay, coFunc));
        }

        public static void StopAllCoroutines(GameObject go)
        {
            var cp = go.GetComponent<CoroutineBehaviour>();
            cp.StopAllCoroutines();
        }

        public static void StopCoroutine(GameObject go, Coroutine co)
        {
            var cp = go.GetComponent<CoroutineBehaviour>();
            if (cp != null)
            {
                cp.StopCoroutine(co);
            }
        }


        private static IEnumerator _DelayWrapper(float delay, System.Action func)
        {
            yield return new WaitForSeconds(delay);
            func();
        }

        private static IEnumerator _DelayWrapper(float delay, System.Action<GameObject> func, GameObject go)
        {
            yield return new WaitForSeconds(delay);
            func(go);
        }

        private static IEnumerator _DelayWrapper(float delay, IEnumerator func)
        {
            yield return new WaitForSeconds(delay);
            yield return func;
        }
    }
}
