using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace nanoSDK
{
    class nanoSDK_Manage : EditorWindow
    {
        // Login
        private string userInputText;
        private string passInputText;

        // Register
        private string regUserInputText;
        private string regPassInputText;
        private string regEmailInputText;

        // License
        private string redeemCode;

        //Window stuff
        private static GUIStyle nanoSdkHeader;
        private static readonly int _sizeX = 1200;
        private static readonly int _sizeY = 800;
        public string currentVersion;
        public string _webData;
        private static Vector2 changeLogScroll;






        int toolbarInt = 1;
        string[] toolbarStrings = { "Changelogs", "Settings", "Importables" };
        public nanoSDK_Manage(){}

        [MenuItem("nanoSDK/MANAGE SDK", false, 500)]
        public static void OpenManageWindow()
        {
            GetWindow<nanoSDK_Manage>(true);
            if (NanoApiManager.IsLoggedInAndVerified()) return;
            NanoApiManager.OpenLoginWindow();
        }

        public void OnEnable()
        {
            titleContent.text = "nanoSDK";
            maxSize = new Vector2(_sizeX, _sizeY);
            minSize = maxSize;

            nanoSdkHeader = new GUIStyle
            {
                normal =
                    {
                    //Top
                       background = Resources.Load("nanoSDKLogo") as Texture2D,
                       textColor = Color.white
                    },
                fixedHeight = 180
            };
        }
        private void OnLostFocus()
        {
            if (NanoApiManager.IsLoggedInAndVerified()) return;
            NanoApiManager.OpenLoginWindow();
        }

        public void OnGUI()
        {
            autoRepaintOnSceneChange = true;
            GetVERSION();

            GUILayout.BeginHorizontal();
            GUILayout.Space(380); //Space bc idk how to allin it to the middle lol moshiro move
            GUILayout.Box("",nanoSdkHeader, GUILayout.Width(500), GUILayout.Height(0));
            GUILayout.EndHorizontal();

            toolbarInt = GUI.Toolbar(new Rect(500, 770, 250, 30), toolbarInt, toolbarStrings);

            switch (toolbarInt)
            {
                case 0:
                    ShowChangelogs();
                    break;

                case 1:
                    ShowSettings();
                    break;

                case 2:
                    ShowImportables();
                        break;
            }

            #region bottom Section
            GUILayout.BeginHorizontal();
            try
            {
                if (GUI.Button(new Rect(10, 755, 100, 20), "Switch Version"))
                {
                    if (NanoApiManager.IsLoggedInAndVerified())
                    {
                        NanoLog("Pressed");
                    }
                }
                GUI.Label(new Rect(10, 775, 150, 20), currentVersion);

                if (NanoApiManager.User.IsPremium)
                {
                    if (GUI.Button(new Rect(155, 775, 100, 20), "Manage Premium"))
                    {
                        NanoLog("Pressed");
                    }
                }
                else
                {
                    if (GUI.Button(new Rect(155, 775, 100, 20), "Premium"))
                    {
                        NanoLog("Pressed");
                    }
                    if (GUI.Button(new Rect(250, 775, 20, 20), "?"))
                    {
                        NanoLog("Pressed");
                    }
                }
                GUI.Label(new Rect(1110, 775, 100, 20), "nanoSDK.net");
                GUI.contentColor = Color.green;
                GUI.Label(new Rect(530, 750, 200, 20), "Thanks for Choosing nanoSDK!");
            }
            catch (NullReferenceException)
            {
                GetVERSION();
            }
            GUILayout.EndHorizontal();
            #endregion
        }

        private void ShowImportables()
        {
            if (NanoApiManager.IsLoggedInAndVerified())
            {
                InitializeData();
            }
        }

        private void ShowSettings()
        {
            if (NanoApiManager.IsLoggedInAndVerified())
            {
                InitializeData();
            }
        }

        private void ShowChangelogs()
        {
            if (NanoApiManager.IsLoggedInAndVerified())
            {
                InitializeData();
                string url = "https://nanosdk.net/download/changelogs/logs.txt";
                using (var client = new WebClient())
                {
                    var webData = client.DownloadString(url);
                    _webData = webData;
                }

                GUILayout.BeginHorizontal();

                GUILayout.Space(300); //Moshiro Move

                changeLogScroll = GUILayout.BeginScrollView(changeLogScroll,GUILayout.Height(500), GUILayout.Width(700));
                GUI.contentColor = Color.green;
                GUILayout.Label("The lagg is caused by unity, not our fault :)");
                GUI.contentColor = Color.white;
                GUILayout.Space(5);
                GUILayout.TextArea(_webData);

                GUILayout.EndScrollView();

                GUILayout.EndHorizontal();
            }
        }

        private void GetVERSION()
        {
            //todoo Json - Type + Version
            if (File.Exists("Assets/VRCSDK/version.txt"))
            {
                var version = File.ReadAllText("Assets/VRCSDK/version.txt");
                currentVersion = version;
            }
            Repaint();
        }

        private static void NanoLog(string message)
        {
            //Our Logger
            Debug.Log("[nanoSDK] Manage: " + message);
        }
        private void InitializeData()
        {
            //will show when user is logged in
            EditorGUILayout.LabelField($"ID:  {NanoApiManager.User.ID}");
            EditorGUILayout.LabelField($"Logged in as:  {NanoApiManager.User.Username}");
            EditorGUILayout.LabelField($"Email:  {NanoApiManager.User.Email}");
            EditorGUILayout.LabelField($"Permission: {NanoApiManager.User.Permission}");
            EditorGUILayout.LabelField($"Verified:  {NanoApiManager.User.IsVerified}");
            EditorGUILayout.LabelField($"Premium:  {NanoApiManager.User.IsPremium}");
            if (GUI.Button(new Rect(115, 75, 100, 20), "Copy"))
            {

                string copyContent = $@"
Username: {NanoApiManager.User.Username}
Email: {NanoApiManager.User.Email}
ID: {NanoApiManager.User.ID}
                    ";
                EditorGUIUtility.systemCopyBuffer = copyContent;
            }
            if (GUI.Button(new Rect(5, 120, 100, 20), "Logout")) NanoApiManager.Logout();
        }
    }
}
