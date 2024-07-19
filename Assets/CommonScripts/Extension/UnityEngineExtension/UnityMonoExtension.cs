using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine;

namespace Custom
{
    public static class UnityMonoExtension
    {
        public class ExceptionContainer
        {
            public Exception exception { get; set; }
        }
        public class ObjectContainer<T>
        {
            public T content { get; set; }
        }
        public static void TryStartCoroutine(this MonoBehaviour monoBehaviour, ref Coroutine routine, IEnumerator enumerator)
        {
            if (routine != null)
                monoBehaviour.StopCoroutine(routine);
            routine = monoBehaviour.TryStartCoroutine(enumerator);
        }
        public static void TryStartCoroutine(this MonoBehaviour monoBehaviour, ref Coroutine routine, IEnumerator enumerator, Action callback)
        {
            if (routine != null)
                monoBehaviour.StopCoroutine(routine);
            routine = monoBehaviour.TryStartCoroutine(enumerator, callback);
        }
        /// <summary>
        /// Try start a coroutine. The inner logic would be protected by try-catch block.
        /// </summary>
        /// <param name="enumerator">Enumerator of a coroutine.</param>
        public static Coroutine TryStartCoroutine(this MonoBehaviour monoBehaviour, IEnumerator enumerator, bool silent = false)
        {
            return monoBehaviour.StartCoroutine(wrap(enumerator, silent));
        }
        /// <summary>
        /// Try start a coroutine. The inner logic would be protected by try-catch block.
        /// </summary>
        /// <param name="enumerator">Enumerator of a coroutine.</param>
        public static Coroutine TryStartCoroutine(this MonoBehaviour monoBehaviour, IEnumerator enumerator, Action callback)
        {
            return monoBehaviour.StartCoroutine(wrap(enumerator, callback));
        }
        /// <summary>
        /// Try start a coroutine. The inner logic would be protected by try-catch block.
        /// </summary>
        /// <param name="enumerator">Enumerator of a coroutine.</param>
        /// <param name="container">Capture the exception if exception occured.</param>
        public static Coroutine TryStartCoroutine(this MonoBehaviour monoBehaviour, IEnumerator enumerator, out ExceptionContainer container, bool silent = false)
        {
            container = new ExceptionContainer();
            return monoBehaviour.StartCoroutine(wrap(enumerator, container, silent));
        }
        /// <summary>
        /// Try start a coroutine. The inner logic would be protected by try-catch block.
        /// </summary>
        /// <param name="enumerator">Enumerator of a coroutine.</param>
        /// <param name="asyncCheck">Async check for every yield</param>
        /// <param name="container">Capture the exception if exception occured.</param>
        public static Coroutine TryStartCoroutine(this MonoBehaviour monoBehaviour, IEnumerator enumerator, Func<bool> asyncCheck, out ExceptionContainer container, bool silent = false)
        {
            container = new ExceptionContainer();
            return monoBehaviour.StartCoroutine(wrap(enumerator, asyncCheck, container, silent));
        }
        static IEnumerator wrap(IEnumerator enumerator, Action callback)
        {
            while (true)
            {
                try
                {
                    if (!enumerator.MoveNext())
                        break;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    break;
                }
                yield return enumerator.Current;
            }
            callback?.TryInvoke();
        }
        static IEnumerator wrap(IEnumerator enumerator, bool silent)
        {
            while (true)
            {
                try
                {
                    if (!enumerator.MoveNext())
                        break;
                }
                catch (Exception e)
                {
                    if (!silent)
                        Debug.LogException(e);
                    break;
                }
                yield return enumerator.Current;
            }
        }
        static IEnumerator wrap(IEnumerator enumerator, ExceptionContainer container, bool silent)
        {
            while (true)
            {
                try
                {
                    if (!enumerator.MoveNext())
                        break;
                }
                catch (Exception e)
                {
                    if (!silent)
                        Debug.LogException(e);
                    container.exception = e;
                    break;
                }
                yield return enumerator.Current;
            }
        }
        static IEnumerator wrap(IEnumerator enumerator, Func<bool> asyncCheck, ExceptionContainer container, bool silent)
        {
            while (asyncCheck())
            {
                try
                {
                    if (!enumerator.MoveNext())
                        break;
                }
                catch (Exception e)
                {
                    if (!silent)
                        Debug.LogException(e);
                    container.exception = e;
                    break;
                }
                yield return enumerator.Current;
            }
        }
        public static bool TryGetComponentInChildren<T>(this GameObject @this, out T component, bool includeInactive = false)
        {
            component = @this.GetComponentInChildren<T>(includeInactive);
            return component != null;
        }
        public static bool TryGetComponentInChildren<T>(this Component @this, out T component, bool includeInactive = false)
        {
            component = @this.GetComponentInChildren<T>(includeInactive);
            return component != null;
        }
        public static bool TryGetComponentInParent<T>(this GameObject @this, out T component, bool includeInactive = false)
        {
            return @this.transform.TryGetComponentInParent(out component, includeInactive);
        }
        public static bool TryGetComponentInParent<T>(this Component @this, out T component, bool includeInactive = false)
        {
            if (includeInactive)
            {
                var parent = @this.transform.parent;
                while (parent)
                {
                    if (parent.TryGetComponent(out component))
                        return true;
                    parent = parent.parent;
                }
                component = default;
                return false;
            }
            else
            {
                component = @this.GetComponentInParent<T>();
                return component != null;
            }
        }
        public static T GetOrAddComponent<T>(this GameObject @this) where T : Component
        {
            var component = @this.GetComponent<T>();
            if (!component) component = @this.AddComponent<T>();
            return component;
        }
    }
}
