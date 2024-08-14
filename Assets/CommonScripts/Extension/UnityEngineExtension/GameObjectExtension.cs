using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Cosmos
{
    public static class GameObjectExtension
    {
        /// <summary>
        /// 获取一个GameObject的完整路径
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetHierarchyPath(this GameObject obj)
        {
            if (obj == null)
                return "";

            string path = obj.name;

            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                path = string.Concat(obj.name, "\\", path);
            }

            return path;
        }
        static Stack<Transform> m_tempChildren = new Stack<Transform>();
        /// <summary>
        /// 获取所有子节点
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public static List<Transform> GetAllChildren(this Transform root, bool self = true)
        {
            m_tempChildren.Clear();
            List<Transform> result = new List<Transform>();

            if (root.childCount == 0)
            {
                if (self) result.Add(root);
            }
            else
            {
                if (self) m_tempChildren.Push(root);
                while (!m_tempChildren.IsNullorEmpty())
                {
                    var current = m_tempChildren.Pop();
                    result.Add(current);

                    for (int i = 0; i < current.childCount; i++)
                        m_tempChildren.Push(current.GetChild(i));
                }
            }
            return result;
        }
        /// <summary>
        /// 获取所有子节点上的组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="root"></param>
        /// <returns></returns>
        public static List<T> GetAllChildren<T>(this Transform root) where T : Component
        {
            m_tempChildren.Clear();
            List<T> result = new List<T>();

            if (root.childCount == 0)
            {
                var com = root.Get<T>();
                if (com != null) result.Add(com);
            }
            else
            {
                m_tempChildren.Push(root);
                while (!m_tempChildren.IsNullorEmpty())
                {
                    var current = m_tempChildren.Pop();
                    var com = current.Get<T>();
                    if (com != null) result.Add(com);

                    for (int i = 0; i < current.childCount; i++)
                        m_tempChildren.Push(current.GetChild(i));
                }
            }
            return result;
        }
        public static GameObject Get(this Transform trans, in string path)
        {
            var child = trans.Find(path);
            Assert.IsNotNull(child, path);
            return child.gameObject;
        }
        public static GameObject Get(this GameObject obj, in string path)
        {
            var child = obj.transform.Find(path);
            Assert.IsNotNull(child, path);
            return child.gameObject;
        }
        public static bool TryGet(this GameObject obj, in string path, out GameObject childObject)
        {
            var child = obj.transform.Find(path);
            Assert.IsNotNull(child, path);
            return childObject = child.gameObject;
        }
        public static T Get<T>(this Transform trans, in string path = null) where T : Component
        {
            if (path == null)
            {
                return trans.GetComponent<T>();
            }
            else
            {
                var child = trans.Get(path);
                Assert.IsNotNull(child, path);
                return trans.Find(path).GetComponent<T>();
            }
        }
        public static T Get<T>(this GameObject obj, in string path = null) where T : Component
        {
            if (path == null)
            {
                return obj.GetComponent<T>();
            }
            else
            {
                var child = obj.transform.Get(path);
                Assert.IsNotNull(child, path);
                return child.GetComponent<T>();
            }
        }
        public static void DestoryChildren(this Transform trans)
        {
            for (int i = trans.childCount - 1; i >= 0; --i)
            {
                var gameObject = trans.GetChild(i).gameObject;
                gameObject.SetActiveOptimize(false);
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    GameObject.DestroyImmediate(gameObject);
                    continue;
                }
#endif
                GameObject.Destroy(gameObject);
            }
        }
        public static void SetActiveOptimize(this GameObject go, bool isActive)
        {
            if (go.activeSelf != isActive)
            {
                go.SetActive(isActive);
            }
        }
        public static void SetActiveOptimize<TComp>(this TComp comp, bool isActive)
            where TComp : Component
            => SetActiveOptimize(comp?.gameObject, isActive);
    }
}
