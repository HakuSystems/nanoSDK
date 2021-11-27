using UnityEngine;
using UnityEditor;

namespace nanoSDK
{
    [InitializeOnLoad]
    public class NanoSDK_Info : EditorWindow
    {

        static NanoSDK_Info()
        {
            EditorApplication.update -= DoSplashScreen;
            EditorApplication.update += DoSplashScreen;
        }

        private static void DoSplashScreen()
        {
            EditorApplication.update -= DoSplashScreen;
            if (EditorApplication.isPlaying)
                return;

            if (EditorPrefs.GetBool("nanoSDK_ShowInfoPanel", true))
                OpenSplashScreen();
        }

        private static Vector2 changeLogScroll;
        private static GUIStyle nanoSdkHeader;
        private static readonly int _sizeX = 400;
        private static readonly int _sizeY = 700;
        private static GUIStyle nanoSdkBottomHeader;
        private static GUIStyle nanoHeaderLearnMoreButton;
        private static GUIStyle nanoBottomHeaderLearnMoreButton;
        [MenuItem("nanoSDK/Info", false, 500)]
        public static void OpenSplashScreen()
        {
            
            //nanoSDK_AutomaticUpdateAndInstall.apiCheckFileExists();
            GetWindow<NanoSDK_Info>(true);
        }

        public static void Open()
        {
            OpenSplashScreen();
        }
        public void OnEnable()
        {
            titleContent = new GUIContent("nanoSDK Info");

            maxSize = new Vector2(_sizeX, _sizeY);
            minSize = maxSize;

            nanoSdkHeader = new GUIStyle
            {
                normal =
                    {
                    //Top
                       background = Resources.Load("nanosdkInfo") as Texture2D,
                       textColor = Color.white
                    },
                fixedHeight = 250
            };

            nanoSdkBottomHeader = new GUIStyle
            {
                normal =
                {
                    //Bottom
                       background = Resources.Load("nanoSdkBanner") as Texture2D,
                       textColor = Color.white
                },
                fixedHeight = 60
            };

        }

        public async void OnGUI()
        {
            GUILayout.Box("", nanoSdkHeader);

            nanoHeaderLearnMoreButton = EditorStyles.miniButton;
            nanoHeaderLearnMoreButton.normal.textColor = Color.black;
            nanoHeaderLearnMoreButton.fontSize = 12;
            nanoHeaderLearnMoreButton.border = new RectOffset(10, 10, 10, 10);
            Texture2D texture = AssetDatabase.GetBuiltinExtraResource<Texture2D>("UI/Skin/UISprite.psd");
            nanoHeaderLearnMoreButton.normal.background = texture;
            nanoHeaderLearnMoreButton.active.background = texture;

            GUILayout.Space(4);
            GUILayout.BeginHorizontal();
            GUI.backgroundColor = Color.gray;
            if (GUILayout.Button("nanoSDK Discord"))
            {
                Application.OpenURL("https://nanosdk.net/discord");
            }
            if (GUILayout.Button("nanoSDK Website"))
            {
                Application.OpenURL("https://nanoSDK.net/");
            }

            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();
            GUILayout.Space(4);
            GUILayout.BeginHorizontal();
            GUI.backgroundColor = Color.gray;
            if (GUILayout.Button("Check for Updates"))
            {
                NanoSDK_AutomaticUpdateAndInstall.AutomaticSDKInstaller();
            }
            if (GUILayout.Button("Reinstall SDK"))
            {
                await NanoSDK_AutomaticUpdateAndInstall.DeleteAndDownloadAsync();
            }
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();
            GUILayout.Space(4);
            GUILayout.Space(2);
            changeLogScroll = GUILayout.BeginScrollView(changeLogScroll, GUILayout.Width(395));

            GUILayout.Label(

    @"Changelog:
== V1.5.2 ==
VRCSDK3-AVATAR-2021.08.04.11.23 (Used as basis, Its the Unity 2019 Vers)
    ┠ bypassed
        ┠ Station limit
        ┠ Quest shader
        ┠ Audio source
        ┠ Unsuported scripts/component
        ┠ VRCExpressionParameters count limit
        ┖ Texture size limit
    ┠ nanoSDKAPIClient
        ┠ Fixed
            ┖ All in one Login

== V1.5.1 ==
VRCSDK3-AVATAR-2021.08.04.11.23 (Used as basis, Its the Unity 2019 Vers)
    ┠ Changed
        ┖ Updater Method
    ┠ Removed
        ┖ SDK 2.0 Version of nanoSDK    
    ┠ nanoSDKAPIClient
        ┠ Added
            ┠ Register and Login
            ┠ Discord
            ┠ MaterialDesign
            ┠ Animations
            ┠ Custom MessageBoxes
            ┠ All in One Login
            ┠ Auto Register Login
            ┠ Updater (Automatic)
            ┖ nanoSDK Installer

== V1.5.0 ==
VRCSDK3-AVATAR-2021.08.04.11.23 (Used as basis, Its the Unity 2019 Vers)
    ┠ Removed
        ┠ login System and API ( until lyze has fixed versions)


== V1.4.9 ==
[COMPLETE REWORK]
VRCSDK3-AVATAR-2021.08.04.11.23 (Used as basis, Its the Unity 2019 Vers)
    ┠ Added
        ┠ Login Welcoming
        ┠ New Unity Version 2019
        ┠ nanoSDK
        ┠ Reinstall SDK Button
        ┠ Delete Missing Scripts to UploadPanel
    ┠ Changed
        ┠ allowUnsafeCode set to true
        ┠ Security License (Requires Update)
    ┠ Removed
        ┠ Old Unity Version
        ┠ VRCsdkControlPanelHelp.cs
        ┠ vrchat sdkUpdater
        ┠ shaderkeywordsutility 
        ┠ HDRColorFixerUtility
        ┠ Force Configure Player Settings
        ┠ clear cache and playerprefs
        ┠ vrc Splash Screen
        ┠ 
    ┠ Bypassed
        ┠ quest upload limit
        ┠ max parameter count
        ┠ max expression count

== V1.4.8 ==
VRCSDK3-AVATAR-2021.04.21.11.58 (Used as basis)
    ┠ Fixed
        ┠ Possible malicious activity detected
        ┖ HWID Login on Multi Accounts

== V1.4.7 ==
VRCSDK3-AVATAR-2021.04.21.11.58 (Used as basis)
    ┠ Added
        ┠ Security Warning
        ┖ Login System
    ┠ Removed
        ┖ nanoSDK_FindMissingScripts it gets too many ERRs will implement with next update maybe again.

== V1.4.6 ==
VRCSDK3-AVATAR-2021.04.21.11.58 (Used as basis)
    ┠ Added
        ┖ LocomotionFix contoller (by edward7)
    ┠ Moved
        ┖ nanoSDK_FindMissingScripts is now on Upload panel.


== V1.4.5 ==
VRCSDK3-AVATAR-2021.04.21.11.58 (Used as basis)
    ┠ Fixed
        ┖ Window Alignments
    ┠ Added
        ┠ UNIVERSE DESIGN (By lost edward7)
        ┠ Thanks for usign nanosdk Debug logs
        ┖ Security
    ┠ Removed
        ┖ Gradient Scroll (again) bc its dumb.

== V1.4.4 ==
VRCSDK3-AVATAR-2021.04.21.11.58 (Used as basis)
    ┠ Fixed
        ┖ NullReferenceException in GradientScroll Script
    ┠ Added
        ┖ V1.4.3.Beta Features (except unity 2019Version) Read More in Commit.

== V1.4.3.Beta ==
U2019-VRCSDK3-AVATAR-2021.07.15.13.44_Public (Used as basis)
    ┠ Forked
        ┖ forked Version V1.4.3 to Unity U2019-VRCSDK3 Beta Version

== V1.4.3 ==
VRCSDK3-AVATAR-2021.04.21.11.58 (Used as basis)
    ┠ Fixed
        ┖ discordRPC wont update after unity Closing

GithubCommitLink: 

== V1.4.2 ==
VRCSDK3-AVATAR-2021.04.21.11.58 (Used as basis)
    ┠ Added
        ┖ Added a Error when Credenials are Null
    ┠ Changed
        ┠ playmode warning message
        ┖ Replaced some nanoSDK text to VRCSdk because it sounds better.

GithubCommitLink: https://github.com/HakuSystems/nanoSDK/commit/eb2b322cdcf20d808e883c2828039d67ca0727d6

== V1.4.1 ==
VRCSDK3-AVATAR-2021.04.21.11.58 (Used as basis)
    ┠ Moved
        ┠ hideConsoleLogsScript (hide errors in console)
            ┖ Moved to Settings - made it turn off and on-able
    ┠ Added
        ┖ Upload Panel Toggle GradientScroll on or off
    ┠ Fixed
        ┖ Upload pannel resolution won't change with window size

== V1.4.0 ==
VRCSDK3-AVATAR-2021.04.21.11.58 (Used as basis)
    ┠ Removals
        ┠ Developer faq
        ┠ VRChat discord
        ┠ Avatar optimization tips
        ┠ Avatar rig requirements
        ┠ VRChat splash screen
        ┠ Avatar Shader Keywords Utility
        ┠ Force Configure Player Settings
        ┠ Clear Cache and PlayerPrefs
        ┠ Avatar optimization tips on Builder
        ┠ Future proof checkbox
        ┖ Builder update checking method
    ┠ Renamed
        ┠ SDK Version Date to SDK Version since our version is now listed on settings tab
        ┠ Builder to Upload text
        ┠ Control panel to Upload Panel
        ┖ Content manager to Uploaded Content
    ┠ Added
        ┠ A little Easter egg :)
        ┠ Runtime Update Checker       
        ┠ nanoSDK_HideConsoleLogs
            ┖ Hides Errors in PlayMode.
        ┠ Discord RPC restart window
        ┠ Version numbers to import panel
        ┠ Updater
            ┖ Async auto Downloader for faster Download of Newest version
        ┠ Old Upload Screen
            ┠ Background Video is now available again
            ┠ Gradient scroll text is now available again
            ┖ Thubmnail image is now available again (custom profile for upload - vrccam)
        ┠ Old HardwareID(HWID) Spoofer
            ┖ nanoSDK_HWIDHelper to VRCCore - Editor.dll
        ┠ Bypassed
            ┠ Audio source limit
            ┠ Station Count limit
            ┠ Unsupported Component/Script limit
            ┠ ExpressionParameters Limit
            ┠ Quest shader limit
            ┠ Model high limit
            ┖ Image size limit
        ┠ Deleted
            ┖ License.txt
        ┠ Changed
            ┠ nanoSDK_info placement on top
            ┠ Future proof default status to false
            ┠ Version check method
            ┠ Banner of control panel
            ┠ SDK Version Date to SDK Version since our version is now listed on settings tab
            ┠ Builder to Upload text
            ┠ Autoupdate version.txt path
            ┖ Api.cs DeviceID method to get ID from nanoSDK_HWIDHelper
                ┠ This spoofes your HWID completely from VRChat
                ┖ I do recommend to add this to your hosts file 0.0.0.0 api.amplitude.com so amplitude cant access their API(Spoofes you more)
        ┠ Updated
            ┠ Doppelganger MetallicFX to newest version
            ┠ PoiyomiToon to newest version
            ┠ LeviantUbershader to newest version
            ┖ MuscleAnimator to the newest version
        ┠ Moved
            ┠ Control panel to nanoSDK tab
            ┖ Version.txt since i moved it to the wrong place
        ┠ Fixed
           ┠ import panel not working
           ┖ Plugin bug in import panel

== V1.3.9 ==
- Fixed nanosdk HWID Spoofer not working.
  ┠ - Bug Reported by Zell (friendly nanosdk user)

== V1.3.8 ==
- Changed Domain to nanosdk.net
- Added Automatic Installer for Newest SDK
  ┠ - this means when u click on check for updates the sdk will check for an update and
  ┠ -if a new version
  ┠ -is available the old version will get Deleted for u and the newest sdk will be
  ┖ -installed and imported.
- Fixed Some Spelling issues
- Removed old Updater
- Moved DiscordRPC
- Moved(nanosdk)Plugins

== V1.3.7 ==
- Changed info panel
- Bug fixes again

== V1.3.6 ==
- Critical bug fixes
- Removed VRCAnimatorRemeasureAvatarEditor

== V1.3.5 ==
Panels:
- Fixed info panel window size
- Fixed import panel alignment
- Updated Check for updates window
Limitation bypass:
- Changed some parameter count display
- Removed controller limitation
- Removed audio source adjusted to safe limits
- Removed script removal
- Removed quest shaders limitation
- Removed VRC Station limitation

- Update made by Dark_Lazer#9308

== V1.3.4 ==
- Bypassed Expression Parameter Count (Bypassed by Dark_Lazer)

== V1.3.3 ==
- Updated to the latest VRChat SDK
- Removed Background Video since its not used in V1.3.2

== V1.3.2 ==
- Fixed Import panel Allingment lol
- Added older changelogs to InfoPanel

== V1.3.1 ==
- Fixed 10MB file Size for Quest


== V1.3 ==
Info panel:
- Added Check for Updates Button
Import panel:
- Added Check for Updates Button
Settings panel:
- Added Check for Updates Button
Tab Section
- Added Check for Updates Button
- Added DelteMissingScripts Function
Quest Users
- You now should be able to Upload Anything to Quest (if not, tell me).

Overall:
- Fixes
- Added check SDK Update Function
    ┖ You can now Click `Check for Sdk Updates` so nanoSDK will be downloaded without opening Discord!
- Removed Useless VRChat Scripts
- Added DelteMissingScripts Function
    ┖ You now can Find Missing Scripts (in the Hierachy) and Delte them with one Click.
- Changed Future Proof Default Enabled to Disabled

== V1.2 ==

Overall:
- Updated to newest VRCHAT SDK
- Fixed Quest Upload not working

== V1.1 ==

Overall:
- Updated to newest VRCHAT SDK (Supports Avatar 3.0 now)
- Removed useless Stuff
- Made upload a little faster
- Avatar Performance is now always Perfect

== V1.0 ==

┠ Updated to newest VRCSDK
┠ Import panel
    ┠ Import panel reworked. Assets are now hosted on the nanoSDK Server
      to make sure you always download the newest version.
    ┠ Besides that you can now look for an update of the asset list.
      It also looks for updates automatically from time to time.
      This means we have the ability to ship new assets without
      requiring an update of the SDK.
    ┠ Added Settings options. Defines the asset download folder.
      This Setting is for every project.
    ┖ Added a download progressbar.
┖ Overall
    ┠ Fixes
    ┖ Optimization

== V0.9 ==

┠ Quick build panel
    ┠ Added Quick build panel to menu tab
    ┖ More compact upload panel
┠ Upload flow
    ┖ Changed compression mode to ChunkBaseCompression (Smaller file = faster upload / load)
┖ Updated Upload Scene/Prefab (World)
    ┖ Made it fit the avatar upload panel

Server File :  PoiyomiToonShader
Updated to : Poiyomi Toon Shader 4.0.2 [Newest]
Original Shader Update: 4h 46m ago


U can easily download the newest Poiyomi shader via nanosdk V0.8.1 [Current] without downloading anything else

== V0.8 ==

┖ Ported everything to the new VRCSDK version 2019.08.20.13.28

== V0.7 ==

┠ Settings panel
    ┖ Added DiscordRPC button
┖ Upload panel
    ┠ Added random background selection
    ┖ Changed colors

== V0.6 == 

┠ Import panel
    ┠ Added nanoSDK_ImportManager
        ┠ Downloads some of the big assets from our server so the sdk it self doesnt get to big
        ┖ For further information, check the menu tab Info
    ┠ Added Downloadable FinalIK 1.7
    ┖ Added Downloadable Poiyomi Toon Shader
┖ Avatar upload panel
    ┖ Added a custom background (background suggestions would be nice btw.)

== V0.5 ==

┠ Menu Item Info
    ┠ Added version info
    ┖ Added instruction text
        ┖ Visemes auto assign instructions
┠ Added Thumbnail selector
    ┠ Select an image to be your avatar thumbnail
    ┖ Used by selecting and image through the thumbnail script that will be added to the VRCam once you are in upload 'screen'

== V0.4 ==

┠ Update nanoSDK_GradientScrollScript
    ┖ Fixed name for MonoBehaviour
┠ Deleted old DLLMaker.dll, forgot it, made some errors
┠ Added Visemes Auto Assign Script
    ┖ Added to CONTEXT Menu of Avatar descriptor (Right-click on VRC_AvatarDiscriptor to set blendshapes)
┖ Website is now under construction. Domain will be show once its finished.

== V0.3.1 == 

┖ Import Panel
    ┖ Moved script into Editor Folder, prevented uploading (Editor Window)

== V0.3 == (Oldest)

VRCSDK-2019.07.31.13.58 (Used as basis)
┠ Updated Splash Screen
    ┠ Moved to nanoSDK menu tab
    ┠ Renamed to Info
    ┠ Changed image, buttons, text
    ┠ Changed buttons to discord, website links
    ┖Changed EditorPrefs key
┠ Updated Settings
    ┠ Moved to nanoSDK menu tab
    ┠ Renamed to Login
    ┠ Removed -Future proof- upload function, set it to FALSE
    ┖ Remove unnecessary code
┠ Updated Build Control Panel
    ┠ Moved to nanoSDK menu tab
    ┠ Renamed to Build and Upload
    ┠ Removed ALL LIMITS
    ┖ Removed unnecessary code
┠ Updated Upload Scene / Prefab
    ┠ Changed some text(provide solutions with error messages)
    ┠ Added GradientScrollScript to cycle trough gradient
    ┠ Added GradientScrollScript to background color
    ┖ Changed button texture
┠ Updated Manage Uploaded Content
    ┠ Moved to nanoSDK menu tab
    ┖ Renamed to Uploaded content
┠ Updated Force Configure Player Settings
    ┖ Moved to nanoSDK menu tab
┠ Updated Clear Cache and PlayerPrefs
    ┖ Moved to nanoSDK menu tab
┠ Added Import panel
    ┠ Added to nanoSDK menu tab
    ┠ Added Info header
    ┠ Added Editor tools
        ┠ QHierarchy
        ┠ Bit Animator
        ┠ Rero Editor Scripts
        ┖ Muscle Animator v2.2
    ┠ Added Plugins
        ┖ Dynamic Bones
    ┖ Added Prefabs
        ┠ nanoSDK World Audio
        ┠ nanoSDK Particle Shader Sphere(Menu overrender)
        ┖ IK World Fixed Joint(FinalIK 1.7 required!)
┠ Added HardwareID(HWID) Spoofer
    ┠ Added nanoSDK_HWIDHelper to VRCCore - Editor.dll
    ┖ Changed Api.cs DeviceID method to get ID from nanoSDK_HWIDHelper
        ┠ This spoofes your HWID completely from VRChat
        ┖ I do recommend to add this to your hosts file -0.0.0.0 api.amplitude.com- so amplitude cant access their API(Spoofes you more)
┖ Website
    ┠ Waiting for domain to register, will be announced once available
    ┠ Set up Webserver
    ┠ Set fancy coming soon as default for now
    ┖ Set up download

");
            GUILayout.EndScrollView();

            GUILayout.Space(4);

            GUILayout.Box("",nanoSdkBottomHeader);
            nanoBottomHeaderLearnMoreButton = EditorStyles.miniButton;
            nanoBottomHeaderLearnMoreButton.normal.textColor = Color.black;
            nanoBottomHeaderLearnMoreButton.fontSize = 10;
            nanoBottomHeaderLearnMoreButton.border = new RectOffset(10, 10, 10, 10);
            nanoBottomHeaderLearnMoreButton.normal.background = texture;
            nanoBottomHeaderLearnMoreButton.active.background = texture;

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();
            EditorPrefs.SetBool("nanoSDK_ShowInfoPanel", GUILayout.Toggle(EditorPrefs.GetBool("nanoSDK_ShowInfoPanel"), "Show at startup"));

            GUILayout.EndHorizontal();

        }
    }
}