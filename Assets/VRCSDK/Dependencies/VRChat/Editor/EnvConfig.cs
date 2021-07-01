﻿#define ENV_SET_INCLUDED_SHADERS

using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.Rendering;
using VRC.SDKBase.Validation.Performance.Stats;

/// <summary>
/// Setup up SDK env on editor launch
/// </summary>
[InitializeOnLoad]
public class EnvConfig
{
    static BuildTarget[] relevantBuildTargets = new BuildTarget[] {
        BuildTarget.Android, BuildTarget.iOS,
        BuildTarget.StandaloneLinux64,
        BuildTarget.StandaloneWindows, BuildTarget.StandaloneWindows64,
        BuildTarget.StandaloneOSX
    };

#if !VRC_CLIENT
    static BuildTarget[] allowedBuildtargets = new BuildTarget[]
    {
        BuildTarget.StandaloneWindows64,
        BuildTarget.Android
    };
#endif

    static System.Collections.Generic.Dictionary<BuildTarget, UnityEngine.Rendering.GraphicsDeviceType[]> allowedGraphicsAPIs = new System.Collections.Generic.Dictionary<BuildTarget, UnityEngine.Rendering.GraphicsDeviceType[]>()
    {
        { BuildTarget.Android, new [] { GraphicsDeviceType.OpenGLES3, /* GraphicsDeviceType.Vulkan */ }},
        { BuildTarget.iOS, null },
        { BuildTarget.StandaloneLinux64, null },
        { BuildTarget.StandaloneWindows, new UnityEngine.Rendering.GraphicsDeviceType[] { UnityEngine.Rendering.GraphicsDeviceType.Direct3D11 } },
        { BuildTarget.StandaloneWindows64, new UnityEngine.Rendering.GraphicsDeviceType[] { UnityEngine.Rendering.GraphicsDeviceType.Direct3D11 } },
        { BuildTarget.StandaloneOSX, null }
    };

#if ENV_SET_INCLUDED_SHADERS && VRC_CLIENT
    static string[] ensureTheseShadersAreAvailable = new string[]
    {
        "Hidden/CubeBlend",
        "Hidden/CubeBlur",
        "Hidden/CubeCopy",
        "Hidden/VideoDecode",
        "Legacy Shaders/Bumped Diffuse",
        "Legacy Shaders/Bumped Specular",
        "Legacy Shaders/Decal",
        "Legacy Shaders/Diffuse Detail",
        "Legacy Shaders/Diffuse Fast",
        "Legacy Shaders/Diffuse",
        "Legacy Shaders/Diffuse",
        "Legacy Shaders/Lightmapped/Diffuse",
        "Legacy Shaders/Lightmapped/Specular",
        "Legacy Shaders/Lightmapped/VertexLit",
        "Legacy Shaders/Parallax Diffuse",
        "Legacy Shaders/Parallax Specular",
        "Legacy Shaders/Reflective/Bumped Diffuse",
        "Legacy Shaders/Reflective/Bumped Specular",
        "Legacy Shaders/Reflective/Bumped Unlit",
        "Legacy Shaders/Reflective/Bumped VertexLit",
        "Legacy Shaders/Reflective/Diffuse",
        "Legacy Shaders/Reflective/Parallax Diffuse",
        "Legacy Shaders/Reflective/Parallax Specular",
        "Legacy Shaders/Reflective/Specular",
        "Legacy Shaders/Reflective/VertexLit",
        "Legacy Shaders/Self-Illumin/Bumped Diffuse",
        "Legacy Shaders/Self-Illumin/Bumped Specular",
        "Legacy Shaders/Self-Illumin/Diffuse",
        "Legacy Shaders/Self-Illumin/Parallax Diffuse",
        "Legacy Shaders/Self-Illumin/Parallax Specular",
        "Legacy Shaders/Self-Illumin/Specular",
        "Legacy Shaders/Self-Illumin/VertexLit",
        "Legacy Shaders/Specular",
        "Legacy Shaders/Transparent/Bumped Diffuse",
        "Legacy Shaders/Transparent/Bumped Specular",
        "Legacy Shaders/Transparent/Cutout/Bumped Diffuse",
        "Legacy Shaders/Transparent/Cutout/Bumped Specular",
        "Legacy Shaders/Transparent/Cutout/Diffuse",
        "Legacy Shaders/Transparent/Cutout/Soft Edge Unlit",
        "Legacy Shaders/Transparent/Cutout/Specular",
        "Legacy Shaders/Transparent/Cutout/VertexLit",
        "Legacy Shaders/Transparent/Diffuse",
        "Legacy Shaders/Transparent/Parallax Diffuse",
        "Legacy Shaders/Transparent/Parallax Specular",
        "Legacy Shaders/Transparent/Specular",
        "Legacy Shaders/Transparent/VertexLit",
        "Legacy Shaders/VertexLit",
        "Mobile/Particles/Additive",
        "Mobile/Particles/Alpha Blended",
        "Mobile/Particles/Multiply",
        "Mobile/Particles/VertexLit Blended",
        "Mobile/Skybox",
        "Nature/Terrain/Diffuse",
        "Nature/Terrain/Specular",
        "Nature/Terrain/Standard",
        "Particles/Additive (Soft)",
        "Particles/Additive",
        "Particles/Alpha Blended Premultiply",
        "Particles/Alpha Blended",
        "Particles/Anim Alpha Blended",
        "Particles/Multiply (Double)",
        "Particles/Multiply",
        "Particles/VertexLit Blended",
        "Particles/~Additive-Multiply",
        "Skybox/Cubemap",
        "Skybox/Procedural",
        "Skybox/6 Sided",
        "Sprites/Default",
        "Sprites/Diffuse",
        "UI/Default",
        "VRChat/UI/Unlit/WebPanelTransparent",
        "Toon/Lit",
        "Toon/Lit (Double)",
        "Toon/Lit Cutout",
        "Toon/Lit Cutout (Double)",
        "Toon/Lit Outline",
        "VRChat/Mobile/Diffuse",
        "Video/RealtimeEmissiveGamma",
        "VRChat/Mobile/Bumped Uniform Diffuse",
        "VRChat/Mobile/Bumped Uniform Specular",
        "VRChat/Mobile/Toon Lit",
        "VRChat/Mobile/MatCap Lit",
        "VRChat/Mobile/Skybox",
        "VRChat/Mobile/Lightmapped",
        "VRChat/PC/Toon Lit",
        "VRChat/PC/Toon Lit (Double)",
        "VRChat/PC/Toon Lit Cutout",
        "VRChat/PC/Toon Lit Cutout (Double)",
        "Unlit/Color",
        "Unlit/Transparent",
        "Unlit/Transparent Cutout",
        "Unlit/Texture",
        "MatCap/Vertex/Textured Lit",
    };
#endif

    private static bool _requestConfigureSettings = true;

    static EnvConfig()
    {
        EditorApplication.update += EditorUpdate;
    }

    static void EditorUpdate()
    {
        if (_requestConfigureSettings)
        {
            if (ConfigureSettings())
            {
                _requestConfigureSettings = false;
            }
        }
    }

    public static void RequestConfigureSettings()
    {
        _requestConfigureSettings = true;
    }

    [UnityEditor.Callbacks.DidReloadScripts(int.MaxValue)]
    static void DidReloadScripts()
    {
        RequestConfigureSettings();
    }

    public static bool ConfigureSettings()
    {
        CheckForFirstInit();

        if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isUpdating)
            return false;

        //ConfigurePlayerSettings();

        if (!VRC.Core.ConfigManager.RemoteConfig.IsInitialized())
        {
            VRC.Core.API.SetOnlineMode(true, "vrchat");
            VRC.Core.ConfigManager.RemoteConfig.Init();
        }

        LoadEditorResources();

        return true;
    }

    static void SetDLLPlatforms(string dllName, bool active)
    {
        string[] assetGuids = AssetDatabase.FindAssets(dllName);

        foreach (string guid in assetGuids)
        {
            string dllPath = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(dllPath) || dllPath.ToLower().EndsWith(".dll") == false)
                return;

            PluginImporter importer = AssetImporter.GetAtPath(dllPath) as PluginImporter;

            bool allCorrect = true;
            if (importer.GetCompatibleWithAnyPlatform() != active)
            {
                allCorrect = false;
            }
            else
            {
                if (importer.GetCompatibleWithAnyPlatform())
                {
                    if (importer.GetExcludeEditorFromAnyPlatform() != !active ||
                        importer.GetExcludeFromAnyPlatform(BuildTarget.StandaloneWindows) != !active)
                    {
                        allCorrect = false;
                    }
                }
                else
                {
                    if (importer.GetCompatibleWithEditor() != active ||
                        importer.GetCompatibleWithPlatform(BuildTarget.StandaloneWindows) != active)
                    {
                        allCorrect = false;
                    }

                }
            }

            if (allCorrect == false)
            {
                if (active)
                {
                    importer.SetCompatibleWithAnyPlatform(true);
                    importer.SetExcludeEditorFromAnyPlatform(false);
                    importer.SetExcludeFromAnyPlatform(BuildTarget.Android, false);
                    importer.SetExcludeFromAnyPlatform(BuildTarget.StandaloneWindows, false);
                    importer.SetExcludeFromAnyPlatform(BuildTarget.StandaloneWindows64, false);
                    importer.SetExcludeFromAnyPlatform(BuildTarget.StandaloneLinux64, false);
                }
                else
                {
                    importer.SetCompatibleWithAnyPlatform(false);
                    importer.SetCompatibleWithEditor(false);
                    importer.SetCompatibleWithPlatform(BuildTarget.Android, false);
                    importer.SetCompatibleWithPlatform(BuildTarget.StandaloneWindows, false);
                    importer.SetCompatibleWithPlatform(BuildTarget.StandaloneWindows64, false);
                    importer.SetCompatibleWithPlatform(BuildTarget.StandaloneLinux64, false);
                }
                importer.SaveAndReimport();
            }
        }
    }
    /*
    [MenuItem("VRChat SDK/Utilities/Force Configure Player Settings")]
    public static void ConfigurePlayerSettings()
    {
        VRC.Core.Logger.Log("Setting required PlayerSettings...", VRC.Core.DebugLevel.All);

        SetBuildTarget();

        // Needed for Microsoft.CSharp namespace in DLLMaker
        // Doesn't seem to work though
        if (PlayerSettings.GetApiCompatibilityLevel(EditorUserBuildSettings.selectedBuildTargetGroup) != ApiCompatibilityLevel.NET_4_6)
            PlayerSettings.SetApiCompatibilityLevel(EditorUserBuildSettings.selectedBuildTargetGroup, ApiCompatibilityLevel.NET_4_6);

        if (!PlayerSettings.runInBackground)
            PlayerSettings.runInBackground = true;

#if !VRC_CLIENT
        SetDLLPlatforms("VRCCore-Standalone", false);
        SetDLLPlatforms("VRCCore-Editor", true);
#endif

        SetDefaultGraphicsAPIs();
        SetGraphicsSettings();
        SetAudioSettings();
        SetPlayerSettings();

#if VRC_CLIENT
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        PlatformSwitcher.RefreshRequiredPackages(EditorUserBuildSettings.selectedBuildTargetGroup);
#else
        // SDK

		// default to steam runtime in sdk (shouldn't matter)
		SetVRSDKs(EditorUserBuildSettings.selectedBuildTargetGroup, new string[] { "None", "OpenVR", "Oculus" });
        VRC.Core.AnalyticsSDK.Initialize(VRC.Core.SDKClientUtilities.GetSDKVersionDate());
#endif

        #if VRC_CLIENT
        // VRCLog should handle disk writing
        PlayerSettings.usePlayerLog = false;
        foreach (LogType logType in Enum.GetValues(typeof(LogType)).Cast<LogType>())
        {
            switch(logType)
            {
                case LogType.Assert:
                case LogType.Exception:
                {
                    PlayerSettings.SetStackTraceLogType(logType, StackTraceLogType.ScriptOnly);
                    break;
                }
                case LogType.Error:
                case LogType.Warning:
                case LogType.Log:
                {
                    PlayerSettings.SetStackTraceLogType(logType, StackTraceLogType.None);
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }
        #endif
    }
    */

    static void EnableBatching(bool enable)
    {
        PlayerSettings[] playerSettings = Resources.FindObjectsOfTypeAll<PlayerSettings>();
        if (playerSettings == null)
            return;

        SerializedObject playerSettingsSerializedObject = new SerializedObject(playerSettings);
        SerializedProperty batchingSettings = playerSettingsSerializedObject.FindProperty("m_BuildTargetBatching");
        if (batchingSettings == null)
            return;

        for (int i = 0; i < batchingSettings.arraySize; i++)
        {
            SerializedProperty batchingArrayValue = batchingSettings.GetArrayElementAtIndex(i);
            if (batchingArrayValue == null)
                continue;

            IEnumerator batchingEnumerator = batchingArrayValue.GetEnumerator();
            if (batchingEnumerator == null)
                continue;

            while (batchingEnumerator.MoveNext())
            {
                SerializedProperty property = (SerializedProperty)batchingEnumerator.Current;

                if (property != null && property.name == "m_BuildTarget")
                {
                    // only change setting on "Standalone" entry
                    if (property.stringValue != "Standalone")
                        break;
                }


                if (property != null && property.name == "m_StaticBatching")
                {
                    property.boolValue = enable;
                }

                if (property != null && property.name == "m_DynamicBatching")
                {
                    property.boolValue = enable;
                }
            }
        }

        playerSettingsSerializedObject.ApplyModifiedProperties();
    }

    public static void SetVRSDKs(BuildTargetGroup buildTargetGroup, string[] sdkNames)
    {
        VRC.Core.Logger.Log("Setting virtual reality SDKs in PlayerSettings: ", VRC.Core.DebugLevel.All);
        if (sdkNames != null)
        {
            foreach (string s in sdkNames)
                VRC.Core.Logger.Log("- " + s, VRC.Core.DebugLevel.All);
        }

        PlayerSettings.SetVirtualRealitySDKs(buildTargetGroup, sdkNames);
    }

    public static bool CheckForFirstInit()
    {
        bool firstLaunch = UnityEditor.SessionState.GetBool("EnvConfigFirstLaunch", true);
        if (firstLaunch)
            UnityEditor.SessionState.SetBool("EnvConfigFirstLaunch", false);

        return firstLaunch;
    }

    static void SetDefaultGraphicsAPIs()
    {
        VRC.Core.Logger.Log("Setting Graphics APIs", VRC.Core.DebugLevel.All);
        foreach (BuildTarget target in relevantBuildTargets)
        {
            var apis = allowedGraphicsAPIs[target];
            if (apis == null)
                SetGraphicsAPIs(target, true);
            else
                SetGraphicsAPIs(target, false, apis);
        }
    }

    static void SetGraphicsAPIs(BuildTarget platform, bool auto, UnityEngine.Rendering.GraphicsDeviceType[] allowedTypes = null)
    {
        try
        {
            if (auto != PlayerSettings.GetUseDefaultGraphicsAPIs(platform))
                PlayerSettings.SetUseDefaultGraphicsAPIs(platform, auto);
        }
        catch { }

        try
        {
            UnityEngine.Rendering.GraphicsDeviceType[] graphicsAPIs = PlayerSettings.GetGraphicsAPIs(platform);
            if (((allowedTypes == null || allowedTypes.Length == 0) &&
                 (graphicsAPIs != null || graphicsAPIs.Length != 0))
                || !allowedTypes.SequenceEqual(graphicsAPIs))
            {
                if (allowedTypes == null)
                {
                    allowedTypes =  PlayerSettings.GetGraphicsAPIs(platform);
                }
                PlayerSettings.SetGraphicsAPIs(platform, allowedTypes);   
            }
        }
        catch { }
    }

    static void SetGraphicsSettings()
    {
        VRC.Core.Logger.Log("Setting Graphics Settings", VRC.Core.DebugLevel.All);

        const string GraphicsSettingsAssetPath = "ProjectSettings/GraphicsSettings.asset";
        SerializedObject graphicsManager = new SerializedObject(UnityEditor.AssetDatabase.LoadAllAssetsAtPath(GraphicsSettingsAssetPath)[0]);

        SerializedProperty deferred = graphicsManager.FindProperty("m_Deferred.m_Mode");
        deferred.enumValueIndex = 1;

        SerializedProperty deferredReflections = graphicsManager.FindProperty("m_DeferredReflections.m_Mode");
        deferredReflections.enumValueIndex = 1;

        SerializedProperty screenSpaceShadows = graphicsManager.FindProperty("m_ScreenSpaceShadows.m_Mode");
        screenSpaceShadows.enumValueIndex = 1;

        SerializedProperty legacyDeferred = graphicsManager.FindProperty("m_LegacyDeferred.m_Mode");
        legacyDeferred.enumValueIndex = 1;

        SerializedProperty depthNormals = graphicsManager.FindProperty("m_DepthNormals.m_Mode");
        depthNormals.enumValueIndex = 1;

        SerializedProperty motionVectors = graphicsManager.FindProperty("m_MotionVectors.m_Mode");
        motionVectors.enumValueIndex = 1;

        SerializedProperty lightHalo = graphicsManager.FindProperty("m_LightHalo.m_Mode");
        lightHalo.enumValueIndex = 1;

        SerializedProperty lensFlare = graphicsManager.FindProperty("m_LensFlare.m_Mode");
        lensFlare.enumValueIndex = 1;

#if ENV_SET_INCLUDED_SHADERS && VRC_CLIENT
        SerializedProperty alwaysIncluded = graphicsManager.FindProperty("m_AlwaysIncludedShaders");
        alwaysIncluded.arraySize = 0;   // clear GraphicsSettings->Always Included Shaders - these cause a +5s app startup time increase on Quest.
                                        // include Shader objects as resources instead

#if ENV_SEARCH_FOR_SHADERS
        Resources.LoadAll("", typeof(Shader));
        System.Collections.Generic.List<Shader> foundShaders = Resources.FindObjectsOfTypeAll<Shader>()
            .Where(s => { string name = s.name.ToLower(); return 0 == (s.hideFlags & HideFlags.DontSave); })
            .GroupBy(s => s.name)
            .Select(g => g.First())
            .ToList();
#else
        System.Collections.Generic.List<Shader> foundShaders = new System.Collections.Generic.List<Shader>();
#endif

        for (int shaderIdx = 0; shaderIdx < ensureTheseShadersAreAvailable.Length; ++shaderIdx)
        {
            if (foundShaders.Any(s => s.name == ensureTheseShadersAreAvailable[shaderIdx]))
                continue;
            Shader namedShader = Shader.Find(ensureTheseShadersAreAvailable[shaderIdx]);
            if (namedShader != null)
                foundShaders.Add(namedShader);
        }

        foundShaders.Sort((s1, s2) => s1.name.CompareTo(s2.name));

        // populate Resources list of "always included shaders"
        ShaderAssetList alwaysIncludedShaders = AssetDatabase.LoadAssetAtPath<ShaderAssetList>("Assets/Resources/AlwaysIncludedShaders.asset");
        alwaysIncludedShaders.Shaders = new Shader[foundShaders.Count];
        for (int shaderIdx = 0; shaderIdx < foundShaders.Count; ++shaderIdx)
            alwaysIncludedShaders.Shaders[shaderIdx] = foundShaders[shaderIdx];
#endif

        SerializedProperty preloaded = graphicsManager.FindProperty("m_PreloadedShaders");
        preloaded.ClearArray();
        preloaded.arraySize = 0;

        SerializedProperty spritesDefaultMaterial = graphicsManager.FindProperty("m_SpritesDefaultMaterial");
        spritesDefaultMaterial.objectReferenceValue = Shader.Find("Sprites/Default");

        SerializedProperty renderPipeline = graphicsManager.FindProperty("m_CustomRenderPipeline");
        renderPipeline.objectReferenceValue = null;

        SerializedProperty transparencySortMode = graphicsManager.FindProperty("m_TransparencySortMode");
        transparencySortMode.enumValueIndex = 0;

        SerializedProperty transparencySortAxis = graphicsManager.FindProperty("m_TransparencySortAxis");
        transparencySortAxis.vector3Value = Vector3.forward;

        SerializedProperty defaultRenderingPath = graphicsManager.FindProperty("m_DefaultRenderingPath");
        defaultRenderingPath.intValue = 1;

        SerializedProperty defaultMobileRenderingPath = graphicsManager.FindProperty("m_DefaultMobileRenderingPath");
        defaultMobileRenderingPath.intValue = 1;

        SerializedProperty tierSettings = graphicsManager.FindProperty("m_TierSettings");
        tierSettings.ClearArray();
        tierSettings.arraySize = 0;

#if ENV_SET_LIGHTMAP
        SerializedProperty lightmapStripping = graphicsManager.FindProperty("m_LightmapStripping");
        lightmapStripping.enumValueIndex = 1;

        SerializedProperty instancingStripping = graphicsManager.FindProperty("m_InstancingStripping");
        instancingStripping.enumValueIndex = 2;

        SerializedProperty lightmapKeepPlain = graphicsManager.FindProperty("m_LightmapKeepPlain");
        lightmapKeepPlain.boolValue = true;

        SerializedProperty lightmapKeepDirCombined = graphicsManager.FindProperty("m_LightmapKeepDirCombined");
        lightmapKeepDirCombined.boolValue = true;

        SerializedProperty lightmapKeepDynamicPlain = graphicsManager.FindProperty("m_LightmapKeepDynamicPlain");
        lightmapKeepDynamicPlain.boolValue = true;

        SerializedProperty lightmapKeepDynamicDirCombined = graphicsManager.FindProperty("m_LightmapKeepDynamicDirCombined");
        lightmapKeepDynamicDirCombined.boolValue = true;

        SerializedProperty lightmapKeepShadowMask = graphicsManager.FindProperty("m_LightmapKeepShadowMask");
        lightmapKeepShadowMask.boolValue = true;

        SerializedProperty lightmapKeepSubtractive = graphicsManager.FindProperty("m_LightmapKeepSubtractive");
        lightmapKeepSubtractive.boolValue = true;
#endif

        SerializedProperty albedoSwatchInfos = graphicsManager.FindProperty("m_AlbedoSwatchInfos");
        albedoSwatchInfos.ClearArray();
        albedoSwatchInfos.arraySize = 0;

        SerializedProperty lightsUseLinearIntensity = graphicsManager.FindProperty("m_LightsUseLinearIntensity");
        lightsUseLinearIntensity.boolValue = true;

        SerializedProperty lightsUseColorTemperature = graphicsManager.FindProperty("m_LightsUseColorTemperature");
        lightsUseColorTemperature.boolValue = true;

        graphicsManager.ApplyModifiedProperties();
    }

    public static FogSettings GetFogSettings()
    {
        VRC.Core.Logger.Log("Force-enabling Fog", VRC.Core.DebugLevel.All);

        const string graphicsSettingsAssetPath = "ProjectSettings/GraphicsSettings.asset";
        SerializedObject graphicsManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath(graphicsSettingsAssetPath)[0]);


        SerializedProperty fogStrippingSerializedProperty = graphicsManager.FindProperty("m_FogStripping");
        FogSettings.FogStrippingMode fogStripping = (FogSettings.FogStrippingMode)fogStrippingSerializedProperty.enumValueIndex;

        SerializedProperty fogKeepLinearSerializedProperty = graphicsManager.FindProperty("m_FogKeepLinear");
        bool keepLinear = fogKeepLinearSerializedProperty.boolValue;

        SerializedProperty fogKeepExpSerializedProperty = graphicsManager.FindProperty("m_FogKeepExp");
        bool keepExp = fogKeepExpSerializedProperty.boolValue;

        SerializedProperty fogKeepExp2SerializedProperty = graphicsManager.FindProperty("m_FogKeepExp2");
        bool keepExp2 = fogKeepExp2SerializedProperty.boolValue;

        FogSettings fogSettings = new FogSettings(fogStripping, keepLinear, keepExp, keepExp2);
        return fogSettings;
    }

    public static void SetFogSettings(FogSettings fogSettings)
    {
        VRC.Core.Logger.Log("Force-enabling Fog", VRC.Core.DebugLevel.All);

        const string graphicsSettingsAssetPath = "ProjectSettings/GraphicsSettings.asset";
        SerializedObject graphicsManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath(graphicsSettingsAssetPath)[0]);

        SerializedProperty fogStripping = graphicsManager.FindProperty("m_FogStripping");
        fogStripping.enumValueIndex = (int)fogSettings.fogStrippingMode;

        SerializedProperty fogKeepLinear = graphicsManager.FindProperty("m_FogKeepLinear");
        fogKeepLinear.boolValue = fogSettings.keepLinear;

        SerializedProperty fogKeepExp = graphicsManager.FindProperty("m_FogKeepExp");
        fogKeepExp.boolValue = fogSettings.keepExp;

        SerializedProperty fogKeepExp2 = graphicsManager.FindProperty("m_FogKeepExp2");
        fogKeepExp2.boolValue = fogSettings.keepExp2;

        graphicsManager.ApplyModifiedProperties();
    }

    static void SetAudioSettings()
    {
        var config = AudioSettings.GetConfiguration();
        config.dspBufferSize = 0; // use default
        config.speakerMode = AudioSpeakerMode.Stereo; // use default
        config.sampleRate = 48000; // forcing 48k seems to avoid sample rate conversion problems
        if (EditorUserBuildSettings.selectedBuildTargetGroup == BuildTargetGroup.Android)
        {
            config.numRealVoices = 24;
            config.numVirtualVoices = 32;
        }
        else
        {
            config.numRealVoices = 32;
            config.numVirtualVoices = 64;
        }
        AudioSettings.Reset(config);
    }

    static void SetPlayerSettings()
    {
        // asset bundles MUST be built with settings that are compatible with VRC client
        #if VRC_OVERRIDE_COLORSPACE_GAMMA
        PlayerSettings.colorSpace = ColorSpace.Gamma;
        #else
        PlayerSettings.colorSpace = ColorSpace.Linear;
        #endif

        #if !VRC_CLIENT // In client rely on platform-switcher
        PlayerSettings.SetVirtualRealitySupported(EditorUserBuildSettings.selectedBuildTargetGroup, true);
        #endif

        PlayerSettings.graphicsJobs = true;

        PlayerSettings.gpuSkinning = true;

        PlayerSettings.stereoRenderingPath = StereoRenderingPath.SinglePass;

        #if UNITY_2018_4_OR_NEWER
        PlayerSettings.scriptingRuntimeVersion = ScriptingRuntimeVersion.Latest;
        #endif

        #if UNITY_ANDROID
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;

        if(PlayerSettings.Android.targetArchitectures.HasFlag(AndroidArchitecture.ARM64))
        {
            // Since we need different IL2CPP args we can't build ARM64 with other Architectures.
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.SetAdditionalIl2CppArgs("");
        }
        else
        {
            PlayerSettings.SetAdditionalIl2CppArgs("--linker-flags=\"-long-plt\"");
        }
        #else
        PlayerSettings.SetAdditionalIl2CppArgs("");
        #endif

        SetActiveSDKDefines();

        EnableBatching(true);
    }

    public static void SetActiveSDKDefines()
    {
        bool definesChanged = false;
        BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
        List<string> defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup).Split(';').ToList();

        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        if (assemblies.Any(assembly => assembly.GetType("VRC.Udon.UdonBehaviour") != null))
        {
            if (!defines.Contains("UDON", StringComparer.OrdinalIgnoreCase))
            {
                defines.Add("UDON");
                definesChanged = true;
            }
        }
        else if (defines.Contains("UDON"))
            defines.Remove("UDON");

        if (VRCSdk3Analysis.IsSdkDllActive(VRCSdk3Analysis.SdkVersion.VRCSDK2))
        {
            if (!defines.Contains("VRC_SDK_VRCSDK2", StringComparer.OrdinalIgnoreCase))
            {
                defines.Add("VRC_SDK_VRCSDK2");
                definesChanged = true;
            }
        }
        else if (defines.Contains("VRC_SDK_VRCSDK2"))
            defines.Remove("VRC_SDK_VRCSDK2");

        if (VRCSdk3Analysis.IsSdkDllActive(VRCSdk3Analysis.SdkVersion.VRCSDK3))
        {
            if (!defines.Contains("VRC_SDK_VRCSDK3", StringComparer.OrdinalIgnoreCase))
            {
                defines.Add("VRC_SDK_VRCSDK3");
                definesChanged = true;
            }
        }
        else if (defines.Contains("VRC_SDK_VRCSDK3"))
            defines.Remove("VRC_SDK_VRCSDK3");

        if (definesChanged)
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, string.Join(";", defines.ToArray()));
    }

    static void SetBuildTarget()
    {
#if !VRC_CLIENT
        VRC.Core.Logger.Log("Setting build target", VRC.Core.DebugLevel.All);

        BuildTarget target = UnityEditor.EditorUserBuildSettings.activeBuildTarget;

        if (!allowedBuildtargets.Contains(target))
        {
            Debug.LogError("Target not supported, switching to one that is.");
            target = allowedBuildtargets[0];
#pragma warning disable CS0618 // Type or member is obsolete
            EditorUserBuildSettings.SwitchActiveBuildTarget(target);
#pragma warning restore CS0618 // Type or member is obsolete
        }
#endif
    }

    private static void LoadEditorResources()
    {
        AvatarPerformanceStats.Initialize();
    }

    public struct FogSettings
    {
        public enum FogStrippingMode
        {
            Automatic,
            Custom
        }

        public readonly FogStrippingMode fogStrippingMode;
        public readonly bool keepLinear;
        public readonly bool keepExp;
        public readonly bool keepExp2;

        public FogSettings(FogStrippingMode fogStrippingMode)
        {
            this.fogStrippingMode = fogStrippingMode;
            keepLinear = true;
            keepExp = true;
            keepExp2 = true;
        }

        public FogSettings(FogStrippingMode fogStrippingMode, bool keepLinear, bool keepExp, bool keepExp2)
        {
            this.fogStrippingMode = fogStrippingMode;
            this.keepLinear = keepLinear;
            this.keepExp = keepExp;
            this.keepExp2 = keepExp2;
        }
    }
}
