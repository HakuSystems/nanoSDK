﻿using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using nanoSDK;
using Newtonsoft.Json.Linq;

namespace nanoSDK
{
    [InitializeOnLoad]
    public class nanoSDK_ImportPanel : EditorWindow
    {
        private static GUIStyle _nanoHeader;
        private static Dictionary<string, string> assets = new Dictionary<string, string>();
        private static int _sizeX = 400;
        private static int _sizeY = 700;
        private static Vector2 _changeLogScroll;
        

        [MenuItem("nanoSDK/Import panel", false, 501)]
        public static void OpenImportPanel()
        {
            GetWindow<nanoSDK_ImportPanel>(true);
        }

        public void OnEnable()
        {
            titleContent = new GUIContent("nanoSDK Import panel");
            
            nanoSDK_ImportManager.checkForConfigUpdate();
            LoadJson();

            maxSize = new Vector2(_sizeX, _sizeY);
            minSize = maxSize;

            _nanoHeader = new GUIStyle
            {
                normal =
                {
                    background = Resources.Load("nanoSdkHeader") as Texture2D,
                    textColor = Color.white
                },
                fixedHeight = 200
            };
        }

        public static void LoadJson()
        {
            assets.Clear();
            
            dynamic configJson =
                JObject.Parse(File.ReadAllText(nanoSDK_Settings.projectConfigPath + nanoSDK_ImportManager.configName));

            Debug.Log("Server Asset Url is: " + configJson["config"]["serverUrl"]);
            nanoSDK_ImportManager.serverUrl = configJson["config"]["serverUrl"].ToString();
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

        public void OnGUI()
        {
            GUILayout.Box("", _nanoHeader);
            GUILayout.Space(4);
            GUI.backgroundColor = Color.gray;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Check for Updates"))
            {

                nanoSDK_AutomaticUpdateAndInstall.AutomaticSDKInstaller();
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
                nanoSDK_ImportManager.updateConfig();
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
                        (File.Exists(nanoSDK_Settings.getAssetPath() + asset.Value) ? "Import" : "Download") +
                        " " + asset.Key))
                    {
                        nanoSDK_ImportManager.downloadAndImportAssetFromServer(asset.Value);
                    }

                    if (GUILayout.Button("Del", GUILayout.Width(40)))
                    {
                        nanoSDK_ImportManager.deleteAsset(asset.Value);
                    }
                }
                GUILayout.EndHorizontal();
            }
            
            GUILayout.EndScrollView();
        }
    }
}