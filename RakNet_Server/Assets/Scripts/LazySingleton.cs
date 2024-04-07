using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RiseRakNet
{
    public abstract class LazySingleton<T> : MonoBehaviour where T : LazySingleton<T>
    {
        public static T Instance
        {
            get
            {
                bool flag = _instance != null;
                T instance;
                if (flag)
                {
                    instance = _instance;
                }
                else
                {
                    GameObject gameObject = new GameObject(typeof(T).Name);
                    _instance = gameObject.AddComponent<T>();
                    instance = _instance;
                }
                return instance;
            }
        }

        public static bool IsInitialized()
        {
            return _instance != null;
        }

        protected virtual void OnDestroy()
        {
            _instance = default(T);
        }

        private static T _instance;
    }
}
