using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using VRC.Core;
using VRC.SDKBase.Editor;

namespace nanoSDK
{
    public class nanoSDKTabView : EditorWindow
    {
        public static nanoSDKTabView window;
        [MenuItem("nanoSDK/Manage", false, 100)]
        public static void ShowWindow()
        {
            window = (nanoSDKTabView)GetWindow(typeof(nanoSDKTabView));
            InitializeWindow();
        }
        private void OnEnable()
        {
            PutIconOnWindow();
            
        }

        private void PutIconOnWindow()
        {
            Texture2D icon = Resources.Load("icon") as Texture2D;
            titleContent.text = " nanoSDK";
            titleContent.image = icon;

        }

        private static void InitializeWindow()
        {
            if (window == null)
            {
                window = (nanoSDKTabView)GetWindow(typeof(nanoSDKTabView));
            }
            
        }

        private void OnGUI()
        {
            autoRepaintOnSceneChange = true;
            Repaint();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();
            //tab things
                EditorWindow loginWindow = GetWindow<NanoSDK_Login>("Account");
                EditorWindow changelogWindow = GetWindow<NanoSDK_Info>("Changelog", typeof(nanoSDKTabView));
                EditorWindow importPanelWindow = GetWindow<NanoSDK_ImportPanel>("Importables", typeof(nanoSDKTabView));
                EditorWindow settingsWindow = GetWindow<NanoSDK_Settings>("Settings", typeof(nanoSDKTabView));
                loginWindow.Show();
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            
        }

    }
}
