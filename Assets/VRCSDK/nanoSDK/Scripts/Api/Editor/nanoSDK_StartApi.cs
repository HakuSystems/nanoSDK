﻿using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace nanoSDK
{
    [InitializeOnLoad]
    public class nanoSDK_StartApi
    {
        static nanoSDK_StartApi()
        {
            StartProgram();
        }
        [InitializeOnLoadMethod]
        public static void StartProgram()
        {
            //api name, ownerid, apikey(Hello fellow code looker u cant do shit with this key dont even try!), apiVersion
            OnProgramStart.Initialize("nanoSDK Api", "859404", "ZrGWTpiQV8WGqC6zIPozy1zyC0LVyOlryUx", "1.0");
            StartLoginWindow();
        }

        private static void StartLoginWindow()
        {
            const string k_ProjectOpened = "ProjectOpened";
            if (!SessionState.GetBool(k_ProjectOpened, false) && EditorApplication.isPlayingOrWillChangePlaymode == false)
            {
                SessionState.SetBool(k_ProjectOpened, true);
                EditorApplication.delayCall += () =>
                {
                    EditorWindow.GetWindow(typeof(nanoSDK_Login), true, "nanoSDK Api Login");
                };
            }
        }
    }

    public class nanoSDK_Login : EditorWindow
    {
        public static string assetName = "nanoSDKAPI.unitypackage";
        public static string unitypackageUrl = "https://nanosdk.net/download/newest/api/";
        //gets this window
        public static nanoSDK_Login Instance { get; private set; }
        //public bool if window is open
        public static bool IsOpen
        {
            get { return Instance != null; }
        }

        private static int _sizeX = 500; //500
        private static int _sizeY = 250; //250
        //login
        private string userinputText;
        private string passinputText;
        //register
        private string regUserinputText;
        private string regPassinputText;
        private string regEmailInputText;
        private string regLicenseInputText;

        public void OnEnable()
        {
            Instance = this;
            if (wantsMouseEnterLeaveWindow)
            {
                Close();
            }
            maxSize = new Vector2(_sizeX, _sizeY);
            minSize = maxSize;
        }

        private void CheckFileExists()
        {
            string apiStartFileLocation = "Assets\\VRCSDK\\nanoSDK\\Scripts\\Api\\Editor\\nanoSDK_StartApi.cs";
            string apiFileLocation = "Assets\\VRCSDK\\nanoSDK\\Scripts\\Api\\Editor\\API.cs";
            if (!File.Exists(apiStartFileLocation))
            {
                DownloadApiFiles();
            }
            else if (!File.Exists(apiFileLocation))
            {
                DownloadApiFiles();
            }

        }

        private void DownloadApiFiles()
        {
            //Creates WebClient to Download latest .unitypackage
            WebClient w = new WebClient();
            w.Headers.Set(HttpRequestHeader.UserAgent, "Webkit Gecko wHTTPS (Keep Alive 55)");
            w.DownloadFileCompleted += new AsyncCompletedEventHandler(fileDownloadComplete);
            w.DownloadProgressChanged += fileDownloadProgress;
            string url = unitypackageUrl;
            w.DownloadFileAsync(new Uri(url), assetName);
        }

        private void fileDownloadProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            //Creates A ProgressBar
            var progress = e.ProgressPercentage;
            if (progress < 0) return;
            if (progress >= 100)
            {
                EditorUtility.ClearProgressBar();
            }
            else
            {
                EditorUtility.DisplayProgressBar("Download of " + assetName,
                    "Downloading " + assetName + " " + progress + "%",
                    (progress / 100F));
            }
        }

        private void fileDownloadComplete(object sender, AsyncCompletedEventArgs e)
        {
            //Checks if Download is complete
            if (e.Error == null)
            {
                nanoLog("Download completed!");
                //Opens .unitypackage
                Process.Start(assetName);
            }
            else
            {
                //Asks to open Download Page Manually
                nanoLog("Download failed!");
                if (EditorUtility.DisplayDialog("nanoSDK Api", "nanoSDK Failed to Download to API", "Cancel"))
                {
                    Process.GetCurrentProcess().Kill();
                }
            }
        }

        private void OnDisable()
        {
            CheckFileExists();
            noVaildCredentials();
        }
        private void OnLostFocus()
        {
            CheckFileExists();
            noVaildCredentials();
        }
        private void OnDestroy()
        {
            CheckFileExists();
            noVaildCredentials();
        }

        void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();
            EditorGUILayout.TextArea("Login");
            userinputText = EditorGUILayout.TextField("Username", userinputText);
            passinputText = EditorGUILayout.PasswordField("Password", passinputText);


            if (GUILayout.Button("Login"))
            {
                if (API.Login(userinputText, passinputText))
                { 
                    EditorUtility.DisplayDialog("nanoSDK Api", "Thanks for using nanoSDK!", "Okay");
                    Close();
                }
            }
            EditorGUILayout.TextArea("Register an Account");

            regUserinputText = EditorGUILayout.TextField("Username", regUserinputText);
            regPassinputText = EditorGUILayout.PasswordField("Password", regPassinputText);
            regEmailInputText = EditorGUILayout.TextField("Email", regEmailInputText);
            regLicenseInputText = EditorGUILayout.PasswordField("License", regLicenseInputText);


            if (GUILayout.Button("Register"))
            {
                if (API.Register(regUserinputText, regPassinputText, regEmailInputText, regLicenseInputText))
                {
                    EditorUtility.DisplayDialog("nanoSDK Api", "You can now Login", "Okay");
                    userinputText = regUserinputText.ToString();
                    passinputText = regPassinputText.ToString();
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void noVaildCredentials()
        {
            if (!API.Login(userinputText, passinputText))
            {
                const string k_ProjectOpened = "ProjectOpened";
                if (!SessionState.GetBool(k_ProjectOpened, false) && EditorApplication.isPlayingOrWillChangePlaymode == false)
                {
                    SessionState.SetBool(k_ProjectOpened, true);
                    EditorApplication.delayCall += () =>
                    {
                        EditorWindow.GetWindow(typeof(nanoSDK_Login), true, "nanoSDK Api Login");
                    };
                }
            }
        }

        private void nanoLog(string message)
        {
            Debug.Log("[nanoSDK API] - " + message);
        }
    }
}
