using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Unity.Plastic.Newtonsoft.Json.Linq;

namespace nanoSDK
{
    public class NanoSDK_ImportPanel : EditorWindow
    {
        private static GUIStyle _nanoHeader;
        private static readonly Dictionary<string, string> assets = new Dictionary<string, string>();
        private static int _sizeX = 400;
        private static int _sizeY = 700;
        private static Vector2 _changeLogScroll;
        

        [MenuItem("nanoSDK/Import panel", false, 501)]
        public static void OpenImportPanel()
        {
            //nanoSDK_AutomaticUpdateAndInstall.apiCheckFileExists();
            GetWindow<NanoSDK_ImportPanel>(false);
            if (NanoApiManager.IsLoggedInAndVerified()) return;
            NanoApiManager.OpenLoginWindow();
        }

        public void OnEnable()
        {

            titleContent = new GUIContent("nanoSDK Import panel");

            NanoSDK_ImportManager.CheckForConfigUpdate();
            LoadJson();

            maxSize = new Vector2(_sizeX, _sizeY);
            minSize = maxSize;
            
            _nanoHeader = new GUIStyle
            {
                normal =
                {
                    background = Resources.Load("nanosdkImportPanel") as Texture2D,
                    textColor = Color.white
                },
                fixedHeight = 250
            };
        }

        public static void LoadJson()
        {
            assets.Clear();
            
            dynamic configJson =
                JObject.Parse(File.ReadAllText(NanoSDK_Settings.projectConfigPath + NanoSDK_ImportManager.configName));

            Debug.Log("Server Asset Url is: " + configJson["config"]["serverUrl"]);
            NanoSDK_ImportManager.serverUrl = configJson["config"]["serverUrl"].ToString();
            _sizeX = (int)configJson["config"]["window"]["sizeX"];
            _sizeY = (int)configJson["config"]["window"]["sizeY"];

            foreach (JProperty x in configJson["assets"])
            {
                var value = x.Value;

                var buttonName = "";
                var file = "";
                
                foreach (var jToken in value)
                {
                    var y = (JProperty) jToken;
                    switch (y.Name)
                    {
                        case "name":
                            buttonName = y.Value.ToString();
                            break;
                        case "file":
                            file = y.Value.ToString();
                            break;
                    }
                }
                assets[buttonName] = file;
            }
        }

        public async void OnGUI()
        {
            GUILayout.Box("", _nanoHeader);
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

            //Update assets
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Update assets (config)"))
            {
                NanoSDK_ImportManager.UpdateConfig();
            }
            GUILayout.EndHorizontal();

            //Imports V!V

            _changeLogScroll = GUILayout.BeginScrollView(_changeLogScroll, GUILayout.Width(_sizeX));
            foreach (var asset in assets)
            {
                GUILayout.BeginHorizontal();
                if (asset.Value == "")
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(asset.Key);
                    GUILayout.FlexibleSpace();
                }
                else
                {
                    if (GUILayout.Button(
                        (File.Exists(NanoSDK_Settings.GetAssetPath() + asset.Value) ? "Import" : "Download") +
                        " " + asset.Key))
                    {
                        NanoSDK_ImportManager.DownloadAndImportAssetFromServer(asset.Value);
                    }

                    if (GUILayout.Button("Del", GUILayout.Width(40)))
                    {
                        NanoSDK_ImportManager.DeleteAsset(asset.Value);
                    }
                }
                GUILayout.EndHorizontal();
            }
            
            GUILayout.EndScrollView();
        }
    }
}