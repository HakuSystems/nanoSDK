using UnityEngine;
using System.IO;
using System;
using UnityEditor;
using System.Net.Http;
using System.Net;
using System.ComponentModel;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace nanoSDK
{
    public class NanoSDK_AutomaticUpdateAndInstall : MonoBehaviour
    {
        public static string apiDownloadUrl = NanoApiManager.SERVERURL;

        //GetVersion
        public static string currentVersion = File.ReadAllText("Assets/VRCSDK/version.txt");


        //select where to be imported (sdk)
        public static string assetPath = "Assets\\";
        //Custom name for downloaded unitypackage
        public static string assetName = "latest.unitypackage";
        //gets VRCSDK Directory Path
        public static string vrcsdkPath = "Assets\\VRCSDK\\";

        public async static void AutomaticSDKInstaller()
        {
            try
            {
                await DownloadnanoSDK();
            }
            catch (Exception ex)
            {
                Debug.LogError("[nanoSDK] AssetDownloadManager:" + ex.Message);
            }
            
        }

        public static async Task DownloadnanoSDK()
        {
            NanoLog("Asking for Approval..");
            if (EditorUtility.DisplayDialog("nanoSDK Updater", "Your Version (V" + currentVersion.ToString() + ") is Outdated!" + " do you want to Download and Import the Newest Version?", "Yes", "No"))
            {
                //starting deletion of old sdk
                await DeleteAndDownloadAsync();
            }
            else
            {
                //canceling the whole process
                NanoLog("You pressed no.");
            }
        }

        public static async Task DeleteAndDownloadAsync()
        {
            try
            {
                if (EditorUtility.DisplayDialog("nanoSDK_Automatic_DownloadAndInstall", "The Old SDK will Be Deleted and the New SDK Will be imported!", "Okay"))
                {
                    try
                    {
                        //gets every file in VRCSDK folder
                        string[] vrcsdkDir = Directory.GetFiles(vrcsdkPath, "*.*");

                        try
                        {
                            //Deletes All Files in VRCSDK folder
                            await Task.Run(() =>
                            {
                                foreach (string f in vrcsdkDir)
                                {
                                    NanoLog($"{f} - Deleted");
                                    File.Delete(f);
                                }
                            });
                        }
                        catch (Exception ex)
                        {
                            EditorUtility.DisplayDialog("Error Deleting VRCSDK", ex.Message, "Okay");
                        }
                    }
                    catch //catch nothing
                    {
                    }
                }
            }
            catch (DirectoryNotFoundException)
            {
                EditorUtility.DisplayDialog("Error Deleting Files", "Error wihle trying to find VRCSDK Folder.", "Ignore");
            }
            //Checks if Directory still exists
            if (Directory.Exists(vrcsdkPath))
            {
                NanoLog($"{vrcsdkPath} - Deleted");
                //Delete Folder
                Directory.Delete(vrcsdkPath, true);
            }
            //Refresh
            AssetDatabase.Refresh();


            WebClient w = new WebClient();
            w.Headers.Set(HttpRequestHeader.UserAgent, "Webkit Gecko wHTTPS (Keep Alive 55)");
            w.DownloadFileCompleted += new AsyncCompletedEventHandler(FileDownloadComplete);
            w.DownloadProgressChanged += FileDownloadProgress;
            try
            {
                string url = apiDownloadUrl;
                w.DownloadFileAsync(new Uri(url), Path.GetTempPath() + "\\" + assetName);
            }
            catch (Exception ex)
            {
                NanoLog("Download failed!");
                if (EditorUtility.DisplayDialog("nanoSDK_Automatic_DownloadAndInstall", "nanoSDK Failed Download: " + ex.Message, "Join Discord for help", "Cancel"))
                {
                    Application.OpenURL("https://nanosdk.net/discord");
                }
            }
        }

        private static void FileDownloadProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            //Creates A ProgressBar
            var progress = e.ProgressPercentage;
            if (progress < 0) return;
            if (progress >= 100)
            {
                EditorUtility.ClearProgressBar();
            }
            else
            {
                EditorUtility.DisplayProgressBar("Download of " + assetName,
                    "Downloading " + assetName + " " + progress + "%",
                    (progress / 100F));
            }
        }

        private static void FileDownloadComplete(object sender, AsyncCompletedEventArgs e)
        {
            //Checks if Download is complete
            try
            {
                if (e.Error == null)
                {
                    string temp = Path.GetTempPath();
                    Process.Start(temp + "\\" + assetName);
                }
            }
            catch (Exception ex)
            {
                NanoLog("Download failed!");
                if (EditorUtility.DisplayDialog("nanoSDK_Automatic_DownloadAndInstall", "nanoSDK Failed Download: " + ex.Message, "Join Discord for help", "Cancel"))
                {
                    Application.OpenURL("https://nanosdk.net/discord");
                }
            }
        }

        private static void NanoLog(string message)
        {
            //Our Logger
            Debug.Log("[nanoSDK] AssetDownloadManager: " + message);
        }
    }
}
