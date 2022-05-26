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


            
            if (GUILayout.Button("HASH"))
            {
                CreateMd5ForFolder($"Assets{Path.DirectorySeparatorChar}VRCSDK{Path.DirectorySeparatorChar}nanoSDK");
                
            }
        }

        public static string CreateMd5ForFolder(string path)
        {
            
            var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                                 .OrderBy(p => p).ToList();
            /*
            foreach (var file in files)
            {
                if (!file.EndsWith(".cs"))
                {
                    continue;
                }
            }
            */
            
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(path));
            var hashString = BitConverter.ToString(hash).Replace("-", "").ToLower();
            File.WriteAllText($"{path}{Path.DirectorySeparatorChar}hash.txt", hashString);
            /*
            var hashFile = File.ReadAllText($"{path}{Path.DirectorySeparatorChar}hash.txt");
            if (hashString == hashFile)
            {
                Debug.Log("Hashes are the same");
            }
            else
            {
                Debug.Log("Hashes are different");
            }
            */
            return hashString;

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
