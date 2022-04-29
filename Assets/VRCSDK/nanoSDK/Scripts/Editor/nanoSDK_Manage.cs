using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

        private static GUIStyle nanoSdkHeader;
        private static readonly int _sizeX = 1200;
        private static readonly int _sizeY = 800;
        public string currentVersion = "";


        int toolbarInt = 0;
        string[] toolbarStrings = { "Login", "Register" };
        public nanoSDK_Manage(){}

        [MenuItem("nanoSDK/MANAGE SDK", false, 500)]
        public static void OpenManageWindow()
        {
            EditorUtility.DisplayDialog("nanoSDK", "This is Still indevelopment we will announce it when its finished.", "okay");
            //GetWindow<nanoSDK_Manage>(true);
        }

        public void OnEnable()
        {
            titleContent = new GUIContent("Manage");

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


        public void OnGUI()
        {
            var version = File.ReadAllText("Assets/VRCSDK/version.txt");
            if (File.Exists(version))
                currentVersion = version;

            GUILayout.BeginHorizontal();
            GUILayout.Space(380); //Space bc idk how to allin it to the middle lol moshiro move
            GUILayout.Box("",nanoSdkHeader, GUILayout.Width(500), GUILayout.Height(0));
            GUILayout.EndHorizontal();

            toolbarInt = GUI.Toolbar(new Rect(515, 675, 250, 30), toolbarInt, toolbarStrings);

            switch (toolbarInt)
            {
                case 0:
                    ShowLogin();
                    break;

                case 1:
                    ShowRegister();
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

                if (GUI.Button(new Rect(1000, 775, 100, 20), "Changelogs"))
                {
                    NanoLog("Pressed");
                }
                GUI.Label(new Rect(1110, 775, 100, 20), "nanoSDK.net");
                GUI.contentColor = Color.green;
                GUI.Label(new Rect(530, 775, 200, 20), "Thanks for Choosing nanoSDK!");
            }
            catch (NullReferenceException)
            {
                currentVersion = "Waiting";
            }
            GUILayout.EndHorizontal();
            #endregion
        }

        private void ShowRegister()
        {
            #region register
            EditorGUILayout.BeginVertical();
            GUI.Label(new Rect(455, 275, 150, 20), "Register an Account");
            regUserInputText = EditorGUI.TextField(new Rect(455, 300, 350, 20),
            "Username",
            regUserInputText);

            regPassInputText = EditorGUI.TextField(new Rect(455, 325, 350, 20),
            "Password",
            regPassInputText);

            regEmailInputText = EditorGUI.TextField(new Rect(455, 350, 350, 20),
            "Email",
            regEmailInputText);

            if (GUI.Button(new Rect(705, 380, 100, 20), "Register"))
            {
                if (string.IsNullOrEmpty(regUserInputText) || string.IsNullOrEmpty(regPassInputText) ||
                    string.IsNullOrEmpty(regEmailInputText))
                    EditorUtility.DisplayDialog("nanoSDK Api", "Credentials Cant be Empty!", "Okay");
                else NanoApiManager.Register(regUserInputText, regPassInputText, regEmailInputText);
                
            }
            EditorGUILayout.EndVertical();
            #endregion
        }

        private void ShowLogin()
        {
            #region Login
            EditorGUILayout.BeginVertical();
            if (NanoApiManager.IsUserLoggedIn())
            {
                InitializeData();

                if (!NanoApiManager.IsLoggedInAndVerified())
                {
                    EditorGUILayout.LabelField("License Key");
                    redeemCode = EditorGUILayout.TextField("Key", redeemCode);
                    if (GUILayout.Button("Redeem"))
                    {
                        if (string.IsNullOrWhiteSpace(redeemCode))
                            EditorUtility.DisplayDialog("nanoSDK Api", "LicenseKey Cant be Empty!", "Okay");
                        else NanoApiManager.RedeemLicense(redeemCode);
                    }
                }

                if (GUILayout.Button("Logout")) NanoApiManager.Logout();

                EditorGUILayout.EndVertical();
                return;
            }

            GUI.Label(new Rect(455, 275, 150, 20), "Login");

            userInputText = EditorGUI.TextField(new Rect(455, 300, 350, 20),
            "Username",
            userInputText);

            passInputText = EditorGUI.TextField(new Rect(455, 325, 350, 20),
            "Password",
            passInputText);

            if (GUI.Button(new Rect(455, 380, 100, 20), "Login"))
            {
                if (string.IsNullOrWhiteSpace(userInputText) || string.IsNullOrWhiteSpace(passInputText))
                    EditorUtility.DisplayDialog("nanoSDK Api", "Credentials Cant be Empty!", "Okay");
                else NanoApiManager.Login(userInputText, passInputText);
                
            }
            EditorGUILayout.EndVertical();
            #endregion
        }

        private static void NanoLog(string message)
        {
            //Our Logger
            Debug.Log("[nanoSDK] Manage: " + message);
        }
        private void InitializeData()
        {
            EditorGUILayout.LabelField($"ID:  {NanoApiManager.User.ID}");
            EditorGUILayout.LabelField($"Logged in as:  {NanoApiManager.User.Username}");
            EditorGUILayout.LabelField($"Email:  {NanoApiManager.User.Email}");
            EditorGUILayout.LabelField($"Permission: {NanoApiManager.User.Permission}");
            EditorGUILayout.LabelField($"Verified:  {NanoApiManager.User.IsVerified}");
            EditorGUILayout.LabelField($"Premium:  {NanoApiManager.User.IsPremium}");
        }
    }
}
