using Assets.VRCSDK.nanoSDK.Premium.Editor;
using nanoSDK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class nanoSDK_EasySearch : EditorWindow
{
    private static GUIStyle vrcSdkHeader;
    private static bool UnitypackageToogle = false;
    private static Vector2 changeLogScroll;
    private static string searchString = "";

    private static int sliderLeftValue = 1;

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

        
        GUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"));
        GUILayout.FlexibleSpace();
        searchString = GUILayout.TextField(searchString, GUI.skin.FindStyle("ToolbarSeachTextField"), GUILayout.Width(780));
        if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton")))
        {
            searchString = string.Empty;
            GUI.FocusControl(null);
        }
        GUILayout.EndHorizontal();



        GUILayout.Space(4);

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"Asset Count: {Everything.Everything_GetNumResults()}", EditorStyles.boldLabel);

        
        //sliderLeftValue = EditorGUILayout.IntSlider(sliderLeftValue, 1, 10);


        EditorGUILayout.EndHorizontal();
        GUILayout.Label(

    @"This feature searches your whole Computer to list your .unitypackges inside of the unity editor.
Side note: This feature is currently quite laggy!

We have implemented this feature to make your life easier so you don't have to search as hard to
find where a specific .unitypackage is on your computer.

Who knows, you might even find some of your old missing assets again.

Recommend settings:
1: Type what you are looking for in the search-bar.

2: Set the asset count to how many results you want shown. (You can use the slider bar or enter a number)

3: Click the small arrow that is located next to the word Unitypackage at the end of the instructions,
this will drop down a list of all found .unitypackages matching your search.

4: The window will begin to lag as it searches your whole computer just a few seconds.

5: You can now import the .unitypackage that you want by clicking the >Import< button to the right of the .unitypackage.", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Unitypackage", EditorStyles.boldLabel, GUILayout.ExpandWidth(false), GUILayout.Width(90));
        UnitypackageToogle = EditorGUILayout.Foldout(UnitypackageToogle, "< Click");
        
        EditorGUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (UnitypackageToogle)
        {
            if (SearchEverythingProgramOpen())
            {
                SearchEverythingTopic(".unitypackage");
            }
            else
            {
                if (EditorUtility.DisplayDialog("nanoSDK", "Search Everything isnt Running please make sure to run it.", "Okay", "Install"))
                {
                    Close();
                }
                else
                {
                    RunInstallAction();
                }
            }
        }

        EditorGUILayout.EndHorizontal();

    }

    private void RunInstallAction()
    {
        if (EditorUtility.DisplayDialog("nanoSDK", "Download of Search Everything has to be made manually since its not a nanoSDK product.", "Okay", "Open website"))
        {

            Close();
        }
        else
        {
            Process.Start("https://www.voidtools.com/downloads/");
        }
    }

    private void SearchEverythingTopic(string topic)
    {
        changeLogScroll = GUILayout.BeginScrollView(changeLogScroll, GUILayout.Width(800));
        RunActuallProcess(topic);
        GUILayout.EndScrollView();
    }
    private void RunActuallProcess(string topic)
    {
        List<string> list = new List<string>();
        var results = Everything.Search(searchString+topic);


        //var resultCount = 0;
        foreach (var result in results)
        {
            //resultCount++;
            /*
            if (resultCount > sliderLeftValue)
                break;
            */
            
            list.Add(result.ResultString());
            
            /*
            if (!result.Filename.Contains(searchString))
                continue;
            */
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            if (!result.Folder)
            {
                if (result.Filename.EndsWith(".unitypackage"))
                {
                    GUILayout.Label(list.Count.ToString(), GUILayout.Width(50));
                    GUILayout.Label(result.Filename);
                    if (GUILayout.Button("Import", GUILayout.Width(50)))
                    {
                        AssetDatabase.ImportPackage(result.Path, true);
                    }
                }
            }
            else
            {
                GUILayout.Label(list.Count.ToString(), GUILayout.Width(50));
                GUILayout.Label(result.Filename + " (Folder)");
            }
            GUILayout.EndHorizontal();


            GUILayout.Space(3);

        }
    }
    private bool SearchEverythingProgramOpen()
    {
        Process[] proc = Process.GetProcessesByName("Everything");
        if (proc.Length == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }


    public void OnEnable()
    {
        titleContent = new GUIContent("EasySearch");

        maxSize = new Vector2(800, 820);
        minSize = maxSize;

        vrcSdkHeader = new GUIStyle
        {
            normal =
                {
                    background = Resources.Load("") as Texture2D,
                    textColor = Color.white
                },
            fixedHeight = 100
        };
    }
}