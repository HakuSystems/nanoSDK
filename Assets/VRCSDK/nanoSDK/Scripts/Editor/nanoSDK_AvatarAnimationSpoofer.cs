using nanoSDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using VRC.SDKBase.Editor;
using Newtonsoft.Json;
using System.Net;

namespace nanoSDK
{
    public class nanoSDK_AvatarAnimationSpoofer : EditorWindow
    {
        [MenuItem("nanoSDK/AvatarAnimationSpoofer", false, 800)]
        private static void OpenWindow()
        {
            nanoSDK_AvatarAnimationSpoofer window = (nanoSDK_AvatarAnimationSpoofer)EditorWindow.GetWindow(typeof(nanoSDK_AvatarAnimationSpoofer));
            window.Show();

            window.titleContent = new GUIContent("RippingSaving");

        }
        private void OnGUI()
        {

            EditorGUILayout.LabelField("Avatar Animation Spoofer", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Amature will be ignored!!");
            if (GUILayout.Button("Copy Gameobject and rename it"))
            {
                AvatarSelectSave();
            }

            if (GUILayout.Button("GENERATE HASH"))
            {
                GenerateHashes($"Assets{Path.DirectorySeparatorChar}VRCSDK{Path.DirectorySeparatorChar}nanoSDK"); 

            }

            if (GUILayout.Button("CHECK HASH"))
            {
                CheckHashes($"Assets{Path.DirectorySeparatorChar}VRCSDK{Path.DirectorySeparatorChar}nanoSDK");
            }
        }

        private void CheckHashes(string path)
        {
            Uri serverUrl = new Uri("https://www.nanosdk.net/download/Hash/hashes.txt");

            var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                                 .OrderBy(p => p).ToList();

            var hashFile = File.ReadAllText($"{path}{Path.DirectorySeparatorChar}hashes.txt");


            //check if code of files are the same as the server has
            foreach (var file in files)
            {
                if (!file.EndsWith(".cs"))
                {
                    continue;
                }
                var md5 = MD5.Create();
                var hash = md5.ComputeHash(File.ReadAllBytes(file));
                var result = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                //Debug.Log(file + " : " + result);
                if (!hashFile.Contains(result))
                {
                    Debug.LogError("File is not the same: " + file + " : " + result);
                    if (EditorUtility.DisplayDialog("Error", "Manipulation Detected, Please dont Manipulate nanoSDK Code!", "OK"))
                    {
                        //Exit Unity
                    }
                }

            }


        }

        public static string GenerateHashes(string path)
        {
            var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                                 .OrderBy(p => p).ToList();
            File.WriteAllText($"{path}{Path.DirectorySeparatorChar}hashes.txt", "");
            foreach (var file in files)
            {
                if (!file.EndsWith(".cs"))
                {
                    continue;
                }
                var md5 = MD5.Create();
                var hash = md5.ComputeHash(File.ReadAllBytes(file));
                var result = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                Debug.Log(file + " : " + result);
                
                File.AppendAllText($"{path}{Path.DirectorySeparatorChar}hashes.txt", $"{result}\n");
            }
            return "";

        }


        private void AvatarSelectSave()
        {
            GameObject selected = Selection.activeGameObject;
            if (selected == null)
            {
                Debug.Log("No GameObject selected");
                return;
            }
            GameObject copy = GameObject.Instantiate(selected);
            copy.name = "nanoSDK Avatar Animation Spoofer";
            RenameGameObjects(copy);

            EditorUtility.DisplayDialog("nanoSDK", "you can upload the spoofed version now.", "OK");
            //NanoSDK_MissingScripts.GetAndDelScripts();
            //VRC_SdkBuilder.shouldBuildUnityPackage = false;
            //VRC_SdkBuilder.ExportAndUploadAvatarBlueprint(copy);


        }
        private void RenameGameObjects(GameObject copy)
        {
            foreach (Transform child in copy.transform)
            {
                if (child.gameObject.name.Contains("Armature"))
                {
                    continue;
                }
                child.name = "nanoSDK Avatar Animation Spoofer";
                RenameGameObjects(child.gameObject);
            }
        }
    }
}
