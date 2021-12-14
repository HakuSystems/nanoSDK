using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using UnityEngine.Serialization;

namespace nanoSDK
{
    [InitializeOnLoad]
    public class NanoSDK_Settings : EditorWindow
    {

        public static string projectConfigPath = "Assets/VRCSDK/nanoSDK/Configs/";
        private readonly string backgroundConfig = "BackgroundVideo.txt";
        private static readonly string projectDownloadPath = "Assets/VRCSDK/nanoSDK/Assets/";
        private static GUIStyle vrcSdkHeader;
        public static bool UITextRainbow  { get; set; }

        [MenuItem("nanoSDK/nanoSDK Settings", false, 501)]
        public static void OpenSplashScreen()
        {
            GetWindow<NanoSDK_Settings>(true);
            if (NanoApiManager.IsLoggedInAndVerified()) return;
            NanoApiManager.OpenLoginWindow();

        }

        public static string GetAssetPath()
        {
            if (EditorPrefs.GetBool("nanoSDK_onlyProject", false))
            {
                return projectDownloadPath;
            }

            var assetPath = EditorPrefs.GetString("nanoSDK_customAssetPath", "%appdata%/nanoSDK/")
                .Replace("%appdata%", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData))
                .Replace("/", "\\");

            if (!assetPath.EndsWith("\\"))
            {
                assetPath += "\\";
            }

            Directory.CreateDirectory(assetPath);
            return assetPath;
        }

        public static void EnableCloseMessage()
        {
            if (EditorPrefs.GetBool("nanoSDK_discordRPC"))
            {
                //Debug.Log("ON");
                if (EditorUtility.DisplayDialog("Discord RPC Restart", "To change Discord RPC you must restart unity  WARNING! Make sure you saved everything.", "Close Unity", "Cancel"))
                {
                    //Debug.Log("set to on");
                    EditorPrefs.SetBool("nanoSDK_discordRPC", true);
                    RealCloseProgram();
                }
                else
                {
                    //Debug.Log("set to off");
                    EditorPrefs.SetBool("nanoSDK_discordRPC", false);
                }
            }
            else
            {
                //Debug.Log("OFF");
                if (EditorUtility.DisplayDialog("Discord RPC Restart", "To change Discord RPC you must restart unity  WARNING! Make sure you saved everything.", "Close Unity", "Cancel"))
                {
                    //Debug.Log("set to off");
                    EditorPrefs.SetBool("nanoSDK_discordRPC", false);
                    RealCloseProgram();
                }
                else
                {
                   //Debug.Log("set to on");
                    EditorPrefs.SetBool("nanoSDK_discordRPC", true);
                }
            }
        }

        private static void RealCloseProgram()
        {
            Debug.Log("Closing Unity");
            EditorApplication.Exit(0);
        }

        public void OnEnable()
        {

            titleContent = new GUIContent("nanoSDK Settings");
            
            maxSize = new Vector2(400, 520);
            minSize = maxSize;
            
            vrcSdkHeader = new GUIStyle
            {
                normal =
                {
                    background = Resources.Load("nanoSDKSexyPanel") as Texture2D,
                    textColor = Color.white
                },
                fixedHeight = 250
            };
            
            if (!EditorPrefs.HasKey("nanoSDK_discordRPC"))
            {
                EditorPrefs.SetBool("nanoSDK_discordRPC", true);
            }

            if (!File.Exists(projectConfigPath + backgroundConfig) || !EditorPrefs.HasKey("nanoSDK_background"))
            {
                EditorPrefs.SetBool("nanoSDK_background", false);
                File.WriteAllText(projectConfigPath + backgroundConfig, "False");
            }
        }

        public async void OnGUI()
        {
            GUILayout.Box("", vrcSdkHeader);
            GUILayout.Space(4);
            GUI.backgroundColor = Color.gray;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Check for Updates"))
            {
                NanoApiManager.CheckServerVersion();
            }
            if (GUILayout.Button("Reinstall SDK"))
            {
                await NanoSDK_AutomaticUpdateAndInstall.DeleteAndDownloadAsync();
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("nanoSDK Discord"))
            {
                Application.OpenURL("https://nanosdk.net/discord");
            }

            if (GUILayout.Button("nanoSDK Website"))
            {
                Application.OpenURL("https://nanoSDK.net/");
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(4);
            GUILayout.Label("Overall:");
            GUILayout.BeginHorizontal();
            var isDiscordEnabled = EditorPrefs.GetBool("nanoSDK_discordRPC", true);
            var enableDiscord = EditorGUILayout.ToggleLeft("Discord RPC", isDiscordEnabled);
            if (enableDiscord != isDiscordEnabled)
            {
                EditorPrefs.SetBool("nanoSDK_discordRPC", enableDiscord);
                EnableCloseMessage();
            }
            //Hide Console logs
            GUILayout.EndHorizontal();
            GUILayout.Space(4);
            GUILayout.BeginHorizontal();
            var isHiddenConsole = EditorPrefs.GetBool("nanoSDK_HideConsole");
            var enableConsoleHide = EditorGUILayout.ToggleLeft("Hide Console Errors", isHiddenConsole);
            if (enableConsoleHide == true)
            {
                EditorPrefs.SetBool("nanoSDK_HideConsole", true);
                Debug.ClearDeveloperConsole();
                Debug.unityLogger.logEnabled = false;
            }
            else if (enableConsoleHide == false)
            {
                EditorPrefs.SetBool("nanoSDK_HideConsole", false);
                Debug.ClearDeveloperConsole();
                Debug.unityLogger.logEnabled = true;
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(4);
            GUILayout.Label("Upload panel:");
            GUILayout.BeginHorizontal();
            var isBackgroundEnabled = EditorPrefs.GetBool("nanoSDK_background", false);
            var enableBackground = EditorGUILayout.ToggleLeft("Custom background", isBackgroundEnabled);
            if (enableBackground != isBackgroundEnabled)
            {
                EditorPrefs.SetBool("nanoSDK_background", enableBackground);
                File.WriteAllText(projectConfigPath + backgroundConfig, enableBackground.ToString());
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(4);
            GUILayout.Label("Import panel:");
            GUILayout.BeginHorizontal();
            var isOnlyProjectEnabled = EditorPrefs.GetBool("nanoSDK_onlyProject", false);
            var enableOnlyProject = EditorGUILayout.ToggleLeft("Save files only in project", isOnlyProjectEnabled);
            if (enableOnlyProject != isOnlyProjectEnabled)
            {
                EditorPrefs.SetBool("nanoSDK_onlyProject", enableOnlyProject);
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(4);
            GUILayout.Label("Asset path:");
            GUILayout.BeginHorizontal();
            var customAssetPath = EditorGUILayout.TextField("",
                EditorPrefs.GetString("nanoSDK_customAssetPath", "%appdata%/nanoSDK/"));
            if (GUILayout.Button("Choose", GUILayout.Width(60)))
            {
                var path = EditorUtility.OpenFolderPanel("Asset download folder",
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "nanoSDK");
                if (path != "")
                {
                    Debug.Log(path);
                    customAssetPath = path;
                }
            }

            if (GUILayout.Button("Reset", GUILayout.Width(50)))
            {
                customAssetPath = "%appdata%/nanoSDK/";
            }

            if (EditorPrefs.GetString("nanoSDK_customAssetPath", "%appdata%/nanoSDK/") != customAssetPath)
            {
                EditorPrefs.SetString("nanoSDK_customAssetPath", customAssetPath);
            }

            GUILayout.EndHorizontal();
        }
    }
}