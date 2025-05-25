using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace WC.Shortcuts 
{
    public enum ShortcutTriggerType { NONE, COLDSTART, BACKGROUND };

    /// <summary>
    /// API for Unity to communicate with Android to manage shortcuts.
    /// <para>Only 1 instance of this manager is expected</para>
    /// </summary>
    [DisallowMultipleComponent]
    public class ShortcutManager : MonoBehaviour
    {
        /// <summary>Ensures that a single instance is available</summary>
        public static ShortcutManager instance { get; private set; } = null;

        /// <summary>Unity player's activity context</summary>
        public static AndroidJavaObject androidContext { get; private set; } = null;
        /// <summary>API's shortcut class in android plugin</summary>
        public static AndroidJavaClass androidShortcutManagerClass { get; private set; } = null;
        /// <summary>Current detected main activity</summary>
        public static string mainActivity { get; private set; } = "";

        private static bool _isAppFirstLaunch = true;

        [Space, Header("Debug")]
        [SerializeField] private bool _debugAndroidJNIHelper = false;
        private static readonly string _debugPrefix = "[ShortcutManager]";

        public static UnityAction<string, ShortcutTriggerType> OnShortcutTriggered;

        private void Awake()
        {
            if(instance == null) instance = this;
            else { Debug.LogError($"{_debugPrefix} Another ShortcutManager instance was detected!"); return; }
            DontDestroyOnLoad(gameObject);

#if UNITY_EDITOR
            Debug.Log($"{_debugPrefix} Cannot initialize ShortcutManager in Editor. Please run the app on a device!");
#elif UNITY_ANDROID
            androidContext = GetContext();
            androidShortcutManagerClass = new("com.wintercrestal.shortcut.WCShortcutManager");
            mainActivity = GetMainActivity(androidContext);

            AndroidJNIHelper.debug = _debugAndroidJNIHelper;
#else
            Debug.Log($"{_debugPrefix} Unsupported platform!");
#endif
        }

        private void Start()
        {
#if UNITY_EDITOR
            Debug.Log($"{_debugPrefix} Shortcuts are not supported in Editor. Please run the app on a device!");

#elif UNITY_ANDROID
            MonitorIntent(ShortcutTriggerType.COLDSTART);
#endif
            _isAppFirstLaunch = false;
        }

        private void OnApplicationPause(bool pause)
        {
#if UNITY_EDITOR

#elif UNITY_ANDROID              
            if (!pause)
            {
                if(!_isAppFirstLaunch)
                    MonitorIntent(ShortcutTriggerType.BACKGROUND);
            }
#endif
        }

        private void OnDestroy()
        {
            if (instance == this) instance = null;
            else Debug.LogError($"{_debugPrefix} Another ShortcutManager instance was detected!");
        }

        #region PUBLIC_API

        /// <summary>Create a shortcut with the given struct info</summary>
        /// <param name="shortcutData">Data struct to create dynamic shortcuts</param>
        public static void CreateShortcut(ShortcutData shortcutData)
        {
#if UNITY_EDITOR
            if (shortcutData.icon != null)
            {
                var iconTexture = shortcutData.icon.texture;
                if (!iconTexture.isReadable) Debug.LogWarning($"{_debugPrefix} Icon is not readable. Please change it in import settings");
                if (!IsTextureUncompressedFormat(iconTexture)) Debug.LogWarning($"{_debugPrefix} Icon is compressed. Make sure to change it in import settings.");
            }

            Debug.Log($"{_debugPrefix} Cannot create shortcuts in the editor!");
#elif UNITY_ANDROID
            CreateShortcutPrivate(shortcutData);
#else
            Debug.Log($"{_debugPrefix} Unsupported platform!");
#endif
        }

        /// <summary>Removes an existing shortcut</summary>
        /// <param name="shortcutID">ID used for creating dynamic shortcut</param>
        public static void RemoveShortcut(string shortcutID)
        {
#if UNITY_EDITOR
            Debug.Log($"{_debugPrefix} Cannot remove shortcuts in the editor!");
#elif UNITY_ANDROID
            RemoveShortcutPrivate(shortcutID);
#else
            Debug.Log($"{_debugPrefix} Unsupported platform!");
#endif
        }

        /// <summary>Checks if the given shortcut ID exists</summary>
        /// <param name="shortcutID">The shortcut's ID to check if it exists</param>
        public static bool HasShortcut(string shortcutID)
        {
#if UNITY_EDITOR
            Debug.Log($"{_debugPrefix} Shortcuts are not supported in Editor!");
            return false;
#elif UNITY_ANDROID
            return HasDynamicShortcutIDPrivate(shortcutID);
#else
            Debug.Log($"{_debugPrefix} Unsupported platform!");
            return false;
#endif
        }

        public static int ShortcutCount
        {
            get
            {
#if UNITY_EDITOR
                Debug.Log($"{_debugPrefix} Shortcuts are not supported in Editor!");
                return 0;
#elif UNITY_ANDROID
                return GetDynamicShortcutCountPrivate();
#else
                Debug.Log($"{_debugPrefix} Unsupported platform!");
                return 0;
#endif
            }
        }

        public static string[] ShortcutIDs
        {
            get
            {
#if UNITY_EDITOR
                Debug.Log($"{_debugPrefix} Shortcuts are not supported in Editor!");
                return null;
#elif UNITY_ANDROID
                return GetDynamicShortcutIDsPrivate();
#else
                Debug.Log($"{_debugPrefix} Unsupported platform!");
                return null;
#endif
            }
        }

        #endregion

        #region PRIVATE_API

#if UNITY_ANDROID

        private static void CreateShortcutPrivate(ShortcutData shortcutData)
        {
            if(shortcutData.systemIcon != ShortcutSystemIcons.NONE)
            {
                androidShortcutManagerClass.CallStatic("CreateDynamicShortcut", androidContext, mainActivity, shortcutData.id, shortcutData.shortLabel, shortcutData.longLabel, shortcutData.systemIcon.ToString());
            }
            else if (shortcutData.icon != null)
            {
                var iconTexture = shortcutData.icon.texture;
                if(iconTexture.isReadable && IsTextureUncompressedFormat(iconTexture))
                {
                    var byteBuffer = GenerateTextureByteBuffer(shortcutData.icon.texture);
                    androidShortcutManagerClass.CallStatic("CreateDynamicShortcut", androidContext, mainActivity, shortcutData.id, shortcutData.shortLabel, shortcutData.longLabel, byteBuffer);
                }
                else
                {
                    Debug.Log($"{_debugPrefix} Icon's texture is not readable or it is compressed. Creating shortcut without icon!");
                    androidShortcutManagerClass.CallStatic("CreateDynamicShortcut", androidContext, mainActivity, shortcutData.id, shortcutData.shortLabel, shortcutData.longLabel, "");
                }  
            }
            else
            {
                androidShortcutManagerClass.CallStatic("CreateDynamicShortcut", androidContext, mainActivity, shortcutData.id, shortcutData.shortLabel, shortcutData.longLabel, "");
            }
        }

        private static void RemoveShortcutPrivate(string shortcutID)
        {
            androidShortcutManagerClass.CallStatic("RemoveDynamicShortcut", androidContext, shortcutID);
        }

        private static int GetDynamicShortcutCountPrivate()
        {
            return androidShortcutManagerClass.CallStatic<int>("GetDynamicShortcutCount", androidContext);
        }

        private static string[] GetDynamicShortcutIDsPrivate()
        {
            return androidShortcutManagerClass.CallStatic<string[]>("GetDynamicShortcutIDs", androidContext);
        }

        private static bool HasDynamicShortcutIDPrivate(string shortcutID)
        {
            return androidShortcutManagerClass.CallStatic<bool>("HasDynamicShortcutID", androidContext, shortcutID);
        }

        private void MonitorIntent(ShortcutTriggerType triggerType)
        {
            AndroidJavaObject intent = androidContext.Call<AndroidJavaObject>("getIntent");
            if (intent.Call<bool>("hasExtra", "shortcut_ID"))
            {
                string action = intent.Call<string>("getStringExtra", "shortcut_ID");
                intent.Call("removeExtra", "shortcut_ID");
                OnShortcutTriggered?.Invoke(action, triggerType);
            }
        }

        private static AndroidJavaObject GetContext()
        {
            AndroidJavaObject unityActivity = new("com.unity3d.player.UnityPlayer");
            return unityActivity.GetStatic<AndroidJavaObject>("currentActivity");
        }

        private static string GetMainActivity(AndroidJavaObject context)
        {
            AndroidJavaClass packageManagerClass = new("android.content.pm.PackageManager");
            AndroidJavaObject packageManager = context.Call<AndroidJavaObject>("getPackageManager");
            string packageName = context.Call<string>("getPackageName");

            AndroidJavaObject launchIntent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", packageName);
            AndroidJavaObject resolveInfo = packageManager.Call<AndroidJavaObject>("resolveActivity", launchIntent, packageManagerClass.GetStatic<int>("MATCH_DEFAULT_ONLY"));
            AndroidJavaObject activityInfo = resolveInfo.Get<AndroidJavaObject>("activityInfo");
            return activityInfo.Get<string>("name");
        }

        private static AndroidJavaObject GenerateTextureByteBuffer(Texture2D texture2D)
        {
            var iconBytes = GetTextureBytes(texture2D);
            AndroidJavaObject byteBuffer = new AndroidJavaClass("java.nio.ByteBuffer").CallStatic<AndroidJavaObject>("wrap", iconBytes);
            return byteBuffer;
        }

        private static sbyte[] GetTextureBytes(Texture2D texture2D)
        {
            var byteData = texture2D.EncodeToPNG();
            sbyte[] sbyteData = new sbyte[byteData.Length];
            Parallel.For(0, byteData.Length, i =>
            {
                sbyteData[i] = (sbyte)byteData[i];
            });
            return sbyteData;
        }

        private static bool IsTextureUncompressedFormat(Texture2D texture)
        {
            return texture.format switch
            {
                TextureFormat.Alpha8 => true,
                TextureFormat.R8 => true,
                TextureFormat.R16 => true,
                TextureFormat.RFloat => true,
                TextureFormat.RG16 => true,
                TextureFormat.RGFloat => true,
                TextureFormat.RGB24 => true,
                TextureFormat.RGBA32 => true,
                TextureFormat.RGBAFloat => true,
                TextureFormat.RGB48 => true,
                TextureFormat.RGBA64 => true,
                _ => false
            };
        }

#endif

        #endregion
    }

}