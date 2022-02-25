using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

namespace nanoSDK
{
    public class nanoSDK_FindMissingScripts : EditorWindow
    {
        static int go_count = 0, components_count = 0, missing_count = 0;
        private static int _sizeX = 310;
        private static int _sizeY = 75;

        [MenuItem("nanoSDK/Help/Utilities/FindAndDelteMissingScripts")]
        public static void OpenMissingScriptWindow()
        {
            GetWindow<nanoSDK_FindMissingScripts>(true);
        }
        private void OnEnable()
        {
            titleContent = new GUIContent("FindAndDelteMissingScripts");
            maxSize = new Vector2(_sizeX, _sizeY);
            minSize = maxSize;
        }
        public void OnGUI()
        {
            GUILayout.Label("Logs will be displayed in Console.");
            GUILayout.Space(4);
            if (GUILayout.Button("Find Missing Scripts in selected GameObjects"))
            {
                FindInSelectedGameObjects();
            }
            GUILayout.Space(4);
            if (GUILayout.Button("Delete All Missing Scripts in selected GameObjects"))
            {
                FindAndRemoveMissingInSelected();
            }
        }
        private static void FindInSelectedGameObjects()
        {
            GameObject[] go = Selection.gameObjects;
            go_count = 0;
            components_count = 0;
            missing_count = 0;
            foreach (GameObject g in go)
            {
                FindInGO(g);
            }

            Debug.Log(string.Format("nanoSDK FindMissingScripts:  Searched {0} GameObjects, {1} components, found {2} missing", go_count, components_count, missing_count));
        }
        private static void FindInGO(GameObject g)
        {
            go_count++;
            Component[] components = g.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                components_count++;
                if (components[i] == null)
                {
                    missing_count++;
                    string s = g.name;
                    Transform t = g.transform;
                    while (t.parent != null)
                    {
                        s = t.parent.name + "/" + s;
                        t = t.parent;
                    }
                    Debug.Log("nanoSDK FindMissingScripts: " + s + " has an empty script attached in position: " + i, g);
                }
            }

            foreach (Transform childT in g.transform)
            {
                //Debug.Log("Searching " + childT.name  + " " );
                FindInGO(childT.gameObject);
            }
        }

        private static void FindAndRemoveMissingInSelected()
        {
            var deepSelection = EditorUtility.CollectDeepHierarchy(Selection.gameObjects);
            int compCount = 0;
            int goCount = 0;
            foreach (var o in deepSelection)
            {
                if (o is GameObject go)
                {
                    int count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
                    if (count > 0)
                    {
                        Undo.RegisterCompleteObjectUndo(go, "Remove missing scripts");
                        GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
                        compCount += count;
                        goCount++;
                    }
                }
            }
            Debug.Log($"nanoSDK FindMissingScripts: Found and removed {compCount} missing scripts from {goCount} GameObjects");
        }

    }
}
