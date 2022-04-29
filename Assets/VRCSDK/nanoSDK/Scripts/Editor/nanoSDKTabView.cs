using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
//using VRC.Core;
//using VRC.SDKBase.Editor;

namespace nanoSDK
{
    public class NanoSDKTabView : EditorWindow
    {
        public static NanoSDKTabView window;
        [MenuItem("nanoSDK/Manage", false, 100)]
        public static void ShowWindow()
        {
            window = (NanoSDKTabView)GetWindow(typeof(NanoSDKTabView));
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
                window = (NanoSDKTabView)GetWindow(typeof(NanoSDKTabView));
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
            _ = GetWindow<NanoSDK_Info>("Changelog", typeof(NanoSDKTabView));
            _ = GetWindow<NanoSDK_ImportPanel>("Importables", typeof(NanoSDKTabView));
            _ = GetWindow<NanoSDK_Settings>("Settings", typeof(NanoSDKTabView));
            loginWindow.Show();
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            
        }

    }
}
