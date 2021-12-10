using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VRC.Core;
using VRC.SDKBase.Editor;

[ExecuteInEditMode]
public class nanoSDK_FastUploader
{
    [MenuItem("nanoSDK/FAST UPLOADER", false, 100)]
    static void Init() => RunFastUpload();

    public static void RunFastUpload()
    {
        if (Application.isPlaying)
        {
            EditorUtility.DisplayDialog("nanoSDK FAST UPLOADER", "Cant Run While in Playmode", "Okay");
            return;
        }
        try
        {
            List<VRC.SDKBase.VRC_AvatarDescriptor> allavatars = VRC.Tools.FindSceneObjectsOfTypeAll<VRC.SDKBase.VRC_AvatarDescriptor>().ToList();

            var avatars = allavatars.Where(av => av.gameObject.activeInHierarchy).ToArray();
            var avatar = allavatars[0];

            if (avatars.Length > 0)
            {
                if (APIUser.CurrentUser == null)
                {
                    EditorUtility.DisplayDialog("nanoSDK FAST UPLOADER", "Login first (vrchat not nanosdk account)!", "Okay");
                    return;
                }

                if (APIUser.CurrentUser.canPublishAvatars)
                {
                    if (EditorUtility.DisplayDialog("nanoSDK FAST UPLOADER", "Avatar: " + "[" + avatar.gameObject.name + "]" + " Will be Uploaded now.", "Nice!", "not the Right one"))
                    {
                        VRC_SdkBuilder.shouldBuildUnityPackage = false;
                        VRC_SdkBuilder.ExportAndUploadAvatarBlueprint(avatar.gameObject);
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("nanoSDK FAST UPLOADER", "This Method gets Index 0 Of all Gameobjects in Hierarchy To fix your issue Please Delete All --not needed-- AvatarDescriptor Scripts. OR Use normal VRChat Uploader", "Okay");
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog("nanoSDK FAST UPLOADER", "Cant Upload Avatar Please Check Normal Upload Method (VRChat One) for Errors", "Okay");
                }

            }
            else
            {
                EditorUtility.DisplayDialog("nanoSDK FAST UPLOADER", "add a avatar descriptor", "Okay");
            }

        }
        catch (ArgumentOutOfRangeException)
        {
            EditorUtility.DisplayDialog("nanoSDK FAST UPLOADER", "Please First Add A Avatar Descriptor To Your Avatar.", "Okay");

        }
        
    }
}
