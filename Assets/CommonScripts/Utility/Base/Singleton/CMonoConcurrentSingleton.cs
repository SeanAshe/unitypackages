using UnityEngine;

namespace Cosmos
{
    public class CMonoConcurrentSingleton<TInstance> : MonoBehaviour where TInstance : CMonoConcurrentSingleton<TInstance>
    {
        private static TInstance s_Instance = null;
        private static readonly object s_syncObj = new();
        private static bool _applicationIsQuitting = false;

        // This defines a static instance property that attempts to find the manager object in the scene and
        // returns it to the caller.
        public static TInstance Instance
        {
            get
            {
                if (_applicationIsQuitting)
                {
                    return null;
                }

                if (s_Instance == null)
                {
                    // This is where the magic happens.
                    //  FindObjectOfType(...) returns the first AManager object in the scene.
                    var tempInstance = FindObjectOfType(typeof(TInstance)) as TInstance;

                    if (tempInstance != null)
                    {
                        lock (s_syncObj)
                        {
                            if (s_Instance == null)
                            {
                                s_Instance = tempInstance;
                            }
                        }
                    }
                }

                // If it is still null, create a new instance
                if (s_Instance == null)
                {
                    lock (s_syncObj)
                    {
                        if (s_Instance == null)
                        {
                            GameObject obj = new(typeof(TInstance).ToString());
                            s_Instance = obj.AddComponent(typeof(TInstance)) as TInstance;
                        }
                    }
                }
                return s_Instance;
            }
        }

        public static void Release()
        {
            if (s_Instance != null)
            {
                MonoBehaviour component = s_Instance as MonoBehaviour;
                Destroy(component.gameObject);
                s_Instance = null;
            }
        }

        // Ensure that the instance is destroyed when the game is stopped in the editor.
        virtual protected void OnApplicationQuit()
        {
            _applicationIsQuitting = true;
            s_Instance = null;
        }
    }
}