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
            window.titleContent.text = "Account";
            window.Show();
        }
        private void OnGUI()
        {

            if (window==null)
            {
                window= (nanoSDKTabView)GetWindow(typeof(nanoSDKTabView));
            }
            Repaint();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();
            //tab things
                EditorWindow loginWindow = GetWindow<NanoSDK_Login>("Account");
                EditorWindow changelogWindow = GetWindow<NanoSDK_Info>("Changelog", typeof(nanoSDKTabView));
                EditorWindow importPanelWindow = GetWindow<NanoSDK_ImportPanel>("ImportPanel", typeof(nanoSDKTabView));
                EditorWindow settingsWindow = GetWindow<NanoSDK_Settings>("Settings", typeof(nanoSDKTabView));
                loginWindow.Show();

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

        }

    }
}
