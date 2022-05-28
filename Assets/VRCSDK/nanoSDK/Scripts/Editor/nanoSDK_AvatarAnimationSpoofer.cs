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

            if (GUILayout.Button("GENERATE HASH"))
            {
                GenerateHashes($"Assets{Path.DirectorySeparatorChar}VRCSDK{Path.DirectorySeparatorChar}nanoSDK"); 

            }
        }

        private void nanoSDKCheckHashes()
        {
            string path = $"Assets{Path.DirectorySeparatorChar}VRCSDK{Path.DirectorySeparatorChar}nanoSDK";
            Uri serverUrl = new Uri("https://www.nanosdk.net/download/Hash/hashes.txt");

            var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                                 .OrderBy(p => p).ToList();

            var hashFile = File.ReadAllText($"{path}{Path.DirectorySeparatorChar}hashes.txt");


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
    }
}
