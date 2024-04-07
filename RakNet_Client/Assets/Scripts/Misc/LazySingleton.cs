using UnityEngine;

namespace RiseRakNet.Misc
{
    public abstract class LazySingleton<T> : MonoBehaviour where T : LazySingleton<T>
    {
        public static T Instance
        {
            get
            {
                if (_instance != null) return _instance;
                var gameObject = new GameObject(typeof(T).Name);
                return _instance = gameObject.AddComponent<T>();
            }
        }

        public static bool IsInitialized()
        {
            return _instance != null;
        }

        protected virtual void OnDestroy()
        {
            _instance = default;
        }

        private static T _instance;
    }
}
