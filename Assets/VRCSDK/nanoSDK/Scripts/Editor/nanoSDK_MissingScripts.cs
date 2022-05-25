using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

namespace nanoSDK
{ 
    public class NanoSDK_MissingScripts
    {
        [MenuItem("nanoSDK/DelteMissingScripts", false, 200)]
        public static async void GetAndDelScripts()
        {
            var gmjs = GameObject.FindObjectsOfType<GameObject>().Where(gmj => GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(gmj) != 0).ToArray();
            for (int i = 0; i < gmjs.Length; i++)
            {
                GameObjectUtility.RemoveMonoBehavioursWithMissingScript(gmjs[i]);
            }
            await Task.Run(()=>
            {
                string message = gmjs.Length == 0 ? "No missing scripts found" : $"{gmjs.Length} missing scripts deleted";
                NanoLog(message);
            });


            /*
            var deepSelection = EditorUtility.CollectDeepHierarchy(Selection.gameObjects);
            int compCount = 0;
            int goCount = 0;
            try
            {
                foreach (var o in deepSelection)
                {
                    if (o is GameObject go)
                    {
                        int count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
                        if (count > 0)
                        {
                            Undo.RegisterCompleteObjectUndo(go, "Removed Scripts.");
                            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
                            compCount += count;
                            goCount++;
                        }
                    }
                }
                await Task.Run(() =>
                {
                    NanoLog($"Found {compCount} missing Scripts from {goCount} Gameobjects - All of them got Deleted.");
                });
            }
            catch (Exception ex)
            {
                await Task.Run(() =>
                {
                    NanoErrLog(ex.Message);
                });
            }
            */
        }

        private static async void NanoLog(string message)
        {
            await Task.Run(() =>
            {
                Debug.Log("[nanoSDK_MissingScripts]: " + message);
            });
        }
    }
}
