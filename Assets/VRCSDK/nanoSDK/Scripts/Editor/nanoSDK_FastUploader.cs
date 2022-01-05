using nanoSDK;
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
    [MenuItem("nanoSDK/EasyUpload", false, 100)]

    public static void RunFastUpload()
    {
        bool checkedForIssues = false;
        if (!VRC.Core.ConfigManager.RemoteConfig.IsInitialized())
        {
            VRC.Core.API.SetOnlineMode(true, "vrchat");
            VRC.Core.ConfigManager.RemoteConfig.Init();
        }
        if (Application.isPlaying)
        {
            EditorUtility.DisplayDialog("nanoSDK EasyUpload", "Cant Run While in Playmode", "Okay");
            return;
        }
        try
        {
            if (!checkedForIssues)
                EnvConfig.ConfigurePlayerSettings();

            List<VRC.SDKBase.VRC_AvatarDescriptor> allavatars = VRC.Tools.FindSceneObjectsOfTypeAll<VRC.SDKBase.VRC_AvatarDescriptor>().ToList();

            var avatars = allavatars.Where(av => av.gameObject.activeInHierarchy).ToArray();
            var avatar = allavatars[0];

            if (avatars.Length > 0)
            {
                if (APIUser.CurrentUser == null)
                {
                    EditorUtility.DisplayDialog("nanoSDK EasyUpload", "Login first (vrchat not nanosdk account)!", "Okay");
                    return;
                }

                if (APIUser.CurrentUser.canPublishAvatars)
                {
                    if (EditorUtility.DisplayDialog("nanoSDK EasyUpload", "Avatar: " + "[" + avatar.gameObject.name + "]" + " Will be Uploaded now. (NOTE: THIS IS ONLY FOR WINDOWS NOT FOR QUEST)", "Nice!", "not the Right one"))
                    {
                        NanoSDK_MissingScripts.GetAndDelScripts();
                        VRC_SdkBuilder.shouldBuildUnityPackage = false;
                        VRC_SdkBuilder.ExportAndUploadAvatarBlueprint(avatar.gameObject);
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("nanoSDK EasyUpload", "This Method gets Index 0 Of all Gameobjects in Hierarchy To fix your issue Please Delete All --not needed-- AvatarDescriptor Scripts. OR Use normal VRChat Uploader", "Okay");
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog("nanoSDK EasyUpload", "Cant Upload Avatar Please Check Normal Upload Method (VRChat One) for Errors", "Okay");
                }

            }
            else
            {
                EditorUtility.DisplayDialog("nanoSDK EasyUpload", "Please First Add A Avatar Descriptor To Your Avatar.", "Okay");
            }

        }
        catch (ArgumentOutOfRangeException)
        {
            EditorUtility.DisplayDialog("nanoSDK EasyUpload", "Please First Add A Avatar Descriptor To Your Avatar.", "Okay");

        }

    }
}
