using nanoSDK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.VRCSDK.nanoSDK.Premium.Editor
{ //TODOO PREMIUM CHECK
    public class nanoLoader : EditorWindow
    {
        private static GUIStyle vrcSdkHeader;
        public static AssetBundle _bundle;
        public static GameObject _object;

        [MenuItem("nanoSDK/nanoLoader", false, 501)]
        public static void OpenSplashScreen()
        {
            GetWindow<nanoLoader>(true);
            if (NanoApiManager.IsLoggedInAndVerified()) return;
            NanoApiManager.OpenLoginWindow();
        }
        public async void OnGUI()
        {
            GUILayout.Box("", vrcSdkHeader);
            GUILayout.Space(4);
            GUI.backgroundColor = Color.gray;
            GUILayout.BeginHorizontal();
            if (!NanoApiManager.User.IsPremium)
            {
                Close();
                if (EditorUtility.DisplayDialog("nanoSDK Premium", "This Feature is only for Premium user", "Buy Premium"))
                {
                    Process.Start("https://www.patreon.com/nanoSDK");
                }
            }
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


            GUILayout.Label(

    @"This feature lets you see any avatar in unity with shaders and everything.
But you cannot get the assets. this tool is made for looking inside avatars
how they are made or maybe look at your own avatars to remember how you
made some things", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Drag and drop your avatar.vrca file here.");
            GUILayout.Space(60);
            GUILayout.Label("Note: this feature is only for avatars it isnt for Worlds!");
            if (Event.current.type == EventType.DragUpdated)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                Event.current.Use();
            }
            else if (Event.current.type == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();
                if (DragAndDrop.paths.Length > 0 && DragAndDrop.objectReferences.Length == 0)
                {
                    foreach (string path in DragAndDrop.paths)
                    {
                        UnityEngine.Debug.Log("- " + path);
                        if (path.EndsWith(".vrca"))
                        {
                            try
                            {
                                if (_bundle)
                                {
                                    _bundle.Unload(false);
                                    Destroy(_object);
                                }
                                _bundle = AssetBundle.LoadFromFile(path);
                                foreach (UnityEngine.Object obj in _bundle.LoadAllAssets())
                                {
                                    _object = (GameObject)Instantiate(obj);
                                }
                            }
                            catch (Exception ex)
                            {
                                EditorUtility.DisplayDialog("nanoLoader", ex.Message, "Okay");
                            }
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("nanoLoader", "Sorry but this is not a Avatar", "Okay");
                        }
                    }
                }
                EditorGUILayout.EndVertical();

            }

        }
        public void OnEnable()
        {
            titleContent = new GUIContent("nanoLoader");

            maxSize = new Vector2(500, 220);
            minSize = maxSize;

            vrcSdkHeader = new GUIStyle
            {
                normal =
                {
                    background = Resources.Load("") as Texture2D,
                    textColor = Color.white
                },
                fixedHeight = 1
            };
            
        }
    }
}
