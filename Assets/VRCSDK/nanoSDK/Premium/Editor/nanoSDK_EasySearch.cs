using Assets.VRCSDK.nanoSDK.Premium.Editor;
using nanoSDK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

public class nanoSDK_EasySearch : EditorWindow
{
    private static GUIStyle vrcSdkHeader;
    [MenuItem("nanoSDK/EasySearch", false, 501)]
    public static void OpenSplashScreen()
    {
        GetWindow<nanoSDK_EasySearch>(true);
        /*
        if (NanoApiManager.IsLoggedInAndVerified()) return;
        NanoApiManager.OpenLoginWindow();
        */

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
        GUILayout.Label("indevelopment");
        GUILayout.BeginScrollView(maxSize);
        GUILayout.Label("Search For Unitypackage:");
        if (GUILayout.Button("Unitypackage"))
        {

            SearchUnitypackage();
        }
        GUILayout.EndScrollView();
    }

    private void SearchUnitypackage()
    {
        List<string> list = new List<string>();
        int MAX_RESULTS = 20;
        var results = Everything.Search(".unitypackage");

        var resultCount = 0;
        foreach (var result in results)
        {
            resultCount++;
            if (resultCount > MAX_RESULTS)
                break;
            list.Add(result.ResultString());
            Debug.Log($"{resultCount} - {result.ResultString()}");
        }
        EditorUtility.DisplayDialog("nanoSDK", "test", "Okay"); //remove later
    }

    public void OnEnable()
    {
        titleContent = new GUIContent("EasySearch");

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
    }
}