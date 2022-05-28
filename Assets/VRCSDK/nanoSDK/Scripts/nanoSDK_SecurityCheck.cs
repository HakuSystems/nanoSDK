using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace nanoSDK
{
    public class nanoSDK_SecurityCheck : MonoBehaviour
    {
        public static void CheckSecurity()
        {
            string path = $"Assets{Path.DirectorySeparatorChar}VRCSDK{Path.DirectorySeparatorChar}nanoSDK";
            string[] files = Directory.GetFiles(path, "nanoSDKHash.dll", SearchOption.AllDirectories);
            //check if dll named nanoSDKHash is in the project
            if (files.Length == 0)
            {
                
            }

        }
    }

}
