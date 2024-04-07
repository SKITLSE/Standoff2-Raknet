using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using RiseRakNet.RakNet;
using UnityEngine;

using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RiseRakNet
{
    public class PlayerAttr : MessageBase
    {
        public ulong Guid { get; set; }
        public string PlayerName { get; set; }
        

        public override void Serialize(BitStream writer)
        {
            writer.Write(Guid);
            writer.Write(PlayerName);
        }

        public override void Deserialize(BitStream reader)
        {
            Guid = reader.ReadULong();
            PlayerName = reader.ReadString();
        }
    }
    public class ResourceNotFoundException : Exception
    {
        // Token: 0x06002ED4 RID: 11988 RVA: 0x00016918 File Offset: 0x00014B18
        public ResourceNotFoundException(string path) : base(string.Format("Resource {0} not found", path))
        {
        }
    }
    public class ResourcesUtility
    {
        // Token: 0x06002ED5 RID: 11989 RVA: 0x000B4270 File Offset: 0x000B2470
        public static T Load<T>([NotNull] string path) where T : UnityEngine.Object
        {
            bool flag = path == null;
            if (flag)
            {
                throw new ArgumentNullException("path");
            }
            bool debugEnabled = ResourcesUtility.Log.DebugEnabled;
            if (debugEnabled)
            {
                ResourcesUtility.Log.Debug(string.Format("Loading resource {0}", path));
            }
            T t = Resources.Load<T>(path);
            bool flag2 = t == null;
            if (flag2)
            {
                throw new ResourceNotFoundException(path);
            }
            bool debugEnabled2 = ResourcesUtility.Log.DebugEnabled;
            if (debugEnabled2)
            {
                ResourcesUtility.Log.Debug(string.Format("Resource {0} successfully loaded", path));
            }
            return t;
        }

        // Token: 0x06002ED6 RID: 11990 RVA: 0x000B4304 File Offset: 0x000B2504
        public static T LoadNullable<T>([NotNull] string path) where T : UnityEngine.Object
        {
            bool flag = path == null;
            if (flag)
            {
                throw new ArgumentNullException("path");
            }
            bool debugEnabled = ResourcesUtility.Log.DebugEnabled;
            if (debugEnabled)
            {
                ResourcesUtility.Log.Debug(string.Format("Loading resource {0}", path));
            }
            T result = Resources.Load<T>(path);
            bool debugEnabled2 = ResourcesUtility.Log.DebugEnabled;
            if (debugEnabled2)
            {
                ResourcesUtility.Log.Debug(string.Format("Resource {0} successfully loaded", path));
            }
            return result;
        }

        // Token: 0x06002ED7 RID: 11991 RVA: 0x000B437C File Offset: 0x000B257C
        public static T LoadSettings<T>() where T : UnityEngine.Object
        {
            T[] array = ResourcesUtility.LoadAll<T>("Settings");
            bool flag = array.Length != 1;
            if (flag)
            {
                throw new ResourceNotFoundException(string.Format("{0} with type {1}", "Settings", typeof(T)));
            }
            return array[0];
        }

        // Token: 0x06002ED8 RID: 11992 RVA: 0x000B43D0 File Offset: 0x000B25D0
        public static T[] LoadAll<T>(string path) where T : UnityEngine.Object
        {
            bool flag = path == null;
            if (flag)
            {
                throw new ArgumentNullException("path");
            }
            bool debugEnabled = ResourcesUtility.Log.DebugEnabled;
            if (debugEnabled)
            {
                ResourcesUtility.Log.Debug(string.Format("Loading resources {0}", path));
            }
            T[] result = Resources.LoadAll<T>(path);
            bool debugEnabled2 = ResourcesUtility.Log.DebugEnabled;
            if (debugEnabled2)
            {
                ResourcesUtility.Log.Debug(string.Format("Resources {0} successfully loaded", path));
            }
            return result;
        }

        // Token: 0x06002ED9 RID: 11993 RVA: 0x000B4448 File Offset: 0x000B2648
        public static void Unload([NotNull] UnityEngine.Object o)
        {
            bool flag = o == null;
            if (flag)
            {
                throw new ArgumentNullException("o");
            }
            bool debugEnabled = ResourcesUtility.Log.DebugEnabled;
            if (debugEnabled)
            {
                ResourcesUtility.Log.Debug(string.Format("Unloading resource {0}", o));
            }
            Resources.UnloadAsset(o);
            bool debugEnabled2 = ResourcesUtility.Log.DebugEnabled;
            if (debugEnabled2)
            {
                ResourcesUtility.Log.Debug("Unloaded successfully...");
            }
        }

        // Token: 0x06002EDA RID: 11994 RVA: 0x000B44B8 File Offset: 0x000B26B8
        public static void UnloadNullable(UnityEngine.Object o)
        {
            bool flag = o == null;
            if (!flag)
            {
                ResourcesUtility.Unload(o);
            }
        }

        // Token: 0x06002EDB RID: 11995 RVA: 0x000B44DC File Offset: 0x000B26DC
        public static ResourceRequest LoadAsync<T>(string path) where T : UnityEngine.Object
        {
            bool debugEnabled = ResourcesUtility.Log.DebugEnabled;
            if (debugEnabled)
            {
                ResourcesUtility.Log.Debug(string.Format("Loading async resource {0}", path));
            }
            return Resources.LoadAsync<T>(path);
        }

        // Token: 0x040026FD RID: 9981
        private static readonly Log Log = Log.Create(typeof(ResourcesUtility));
    }
    public class Log
    {
        // Token: 0x06002CBD RID: 11453 RVA: 0x000AD400 File Offset: 0x000AB600
        public static Log Create(Type type)
        {
            return new Log(type.Name);
        }

        // Token: 0x06002CBE RID: 11454 RVA: 0x000AD420 File Offset: 0x000AB620
        public static Log Create(string tag)
        {
            return new Log(tag);
        }

        // Token: 0x06002CBF RID: 11455 RVA: 0x00015C2A File Offset: 0x00013E2A
        public Log(string tag)
        {
            _tag = tag;
            _logger = UnityEngine.Debug.unityLogger;
            _logger.logEnabled = false;
        }

        // Token: 0x170006F2 RID: 1778
        // (get) Token: 0x06002CC0 RID: 11456 RVA: 0x00002247 File Offset: 0x00000447
        public bool DebugEnabled
        {
            get
            {
                return false;
            }
        }

        // Token: 0x06002CC1 RID: 11457 RVA: 0x000AD438 File Offset: 0x000AB638
        public void Debug(object message)
        {
            bool debugEnabled = DebugEnabled;
            if (debugEnabled)
            {
                _logger.Log(_tag, message);
            }
        }

        // Token: 0x06002CC2 RID: 11458 RVA: 0x000AD464 File Offset: 0x000AB664
        public void Warning(string message, params object[] args)
        {
            bool debugEnabled = DebugEnabled;
            if (debugEnabled)
            {
                _logger.LogWarning(_tag, Log.FormatMessage(message, args));
            }
        }

        // Token: 0x06002CC3 RID: 11459 RVA: 0x000AD498 File Offset: 0x000AB698
        public void Error(object message, UnityEngine.Object context)
        {
            bool debugEnabled = DebugEnabled;
            if (debugEnabled)
            {
                _logger.LogError(_tag, message, context);
            }
        }

        // Token: 0x06002CC4 RID: 11460 RVA: 0x000AD4C4 File Offset: 0x000AB6C4
        public void Error(object message)
        {
            bool debugEnabled = DebugEnabled;
            if (debugEnabled)
            {
                _logger.LogError(_tag, message);
            }
        }

        // Token: 0x06002CC5 RID: 11461 RVA: 0x000AD4F0 File Offset: 0x000AB6F0
        public void Debug(object message, UnityEngine.Object context)
        {
            bool debugEnabled = DebugEnabled;
            if (debugEnabled)
            {
                _logger.Log(_tag, message, context);
            }
        }

        // Token: 0x06002CC6 RID: 11462 RVA: 0x000AD51C File Offset: 0x000AB71C
        public void Warning(string message, UnityEngine.Object context, params object[] args)
        {
            bool debugEnabled = DebugEnabled;
            if (debugEnabled)
            {
                _logger.LogWarning(_tag, Log.FormatMessage(message, args), context);
            }
        }

        // Token: 0x06002CC7 RID: 11463 RVA: 0x000AD550 File Offset: 0x000AB750
        private static string FormatMessage(string message, params object[] args)
        {
            bool flag = args.Length != 0;
            if (flag)
            {
                message = string.Format(message, args);
            }
            return message;
        }

        // Token: 0x04002565 RID: 9573
        private readonly string _tag;

        // Token: 0x04002566 RID: 9574
        private readonly ILogger _logger;
    }
    public static class MonoBehaviourExtension
    {
        // Token: 0x060004AC RID: 1196 RVA: 0x0002FE98 File Offset: 0x0002E098
        public static T GetRequireComponent<T>(this Component component)
        {
            T component2 = component.GetComponent<T>();
            bool flag = component2 == null;
            if (flag)
            {
                throw new MonoBehaviourExtension.RequireComponentMissingException(component, typeof(T));
            }
            return component2;
        }

        // Token: 0x060004AD RID: 1197 RVA: 0x0002FED4 File Offset: 0x0002E0D4
        public static T GetRequireComponent<T>(this GameObject gameObject)
        {
            T component = gameObject.GetComponent<T>();
            bool flag = component == null;
            if (flag)
            {
                throw new MonoBehaviourExtension.RequireComponentMissingException(gameObject, typeof(T));
            }
            return component;
        }

        // Token: 0x060004AE RID: 1198 RVA: 0x0002FF10 File Offset: 0x0002E110
        public static T GetRequireComponentInChildren<T>(this Component component)
        {
            T componentInChildren = component.GetComponentInChildren<T>();
            bool flag = componentInChildren == null;
            if (flag)
            {
                throw new MonoBehaviourExtension.RequireComponentMissingException(component, typeof(T));
            }
            return componentInChildren;
        }

        // Token: 0x060004AF RID: 1199 RVA: 0x0002FF4C File Offset: 0x0002E14C
        public static T GetRequireComponentInChildren<T>(this GameObject gameObject)
        {
            T componentInChildren = gameObject.GetComponentInChildren<T>();
            bool flag = componentInChildren == null;
            if (flag)
            {
                throw new MonoBehaviourExtension.RequireComponentMissingException(gameObject, typeof(T));
            }
            return componentInChildren;
        }

        // Token: 0x020000B9 RID: 185
        public class RequireComponentMissingException : InvalidOperationException
        {
            // Token: 0x060004B0 RID: 1200 RVA: 0x00003B89 File Offset: 0x00001D89
            public RequireComponentMissingException(UnityEngine.Object component, Type componentType) : base(component + " not define require component " + componentType)
            {
            }
        }
    }
    public class Game : MonoBehaviour
    {
        [SerializeField] private Image _progressImage;
        [SerializeField] private GameController _gameController;
        [SerializeField] private GameObject _loadPanel;

        private static string _mapName;
        private static PlayerAttr _playerAttr;
        private AsyncOperation _levelLoadingAsync;
        public Camera FpsCamera;
        public Camera MainCamera;
        public static void Init(string mapName, PlayerAttr player)
        {
            _mapName = mapName;
            _playerAttr = player;
            SceneManager.LoadScene("Game", LoadSceneMode.Single);
        }
        private void LoadLevel()
        {
            base.StartCoroutine(this.LoadLevelAsync());
        }

        private IEnumerator LoadLevelAsync()
        {
            _levelLoadingAsync = SceneManager.LoadSceneAsync(_mapName, LoadSceneMode.Additive);
            while (!_levelLoadingAsync.isDone)
            {
                Debug.Log(_levelLoadingAsync.progress);
                _progressImage.fillAmount = _levelLoadingAsync.progress;
                yield return null;
            }
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(_mapName));
            yield return new WaitForSeconds(0.1f);
            yield return _gameController.Init();
        }

        // Start is called before the first frame update
        private IEnumerator Start()
        {
            LoadLevel();
            MainCamera = Camera.main;
            yield break;
        }
    }
}
