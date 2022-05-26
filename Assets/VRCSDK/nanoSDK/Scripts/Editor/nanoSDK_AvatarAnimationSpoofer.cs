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
            // assuming you want to include nested folders
            var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                                 .OrderBy(p => p).ToList();
            //foreach if meta file is found
            for (int i = 0; i < files.Count; i++)
            {
                if (!files[i].EndsWith(".cs"))
                {
                    files.RemoveAt(i);
                }
            }
            foreach (string s in files)
            {
                Debug.Log(s);
            }



            MD5 md5 = MD5.Create();

            for (int i = 0; i < files.Count; i++)
            {
                string file = files[i];

                // hash path
                string relativePath = file.Substring(path.Length + 1);
                byte[] pathBytes = Encoding.UTF8.GetBytes(relativePath.ToLower());
                md5.TransformBlock(pathBytes, 0, pathBytes.Length, pathBytes, 0);

                // hash contents
                byte[] contentBytes = File.ReadAllBytes(file);
                if (i == files.Count - 1)
                    md5.TransformFinalBlock(contentBytes, 0, contentBytes.Length);
                else
                    md5.TransformBlock(contentBytes, 0, contentBytes.Length, contentBytes, 0);
            }
            
            //now hash the result
            byte[] hash = md5.Hash;
            string result = BitConverter.ToString(hash).Replace("-", "").ToLower();
            Debug.Log("MD5: " + result);
            return result;
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
