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

namespace nanoSDK
{
    public class nanoSDK_AutomaticUpdateAndInstall : MonoBehaviour
    {

        //get server version
        public static string versionURL = "https://nanosdk.net/download/Version/version.txt";
        //get download url
        public static string unitypackageUrl = "https://nanosdk.net/download/newest/"; //sdk

        //GetVersion
        public static string currentVersion = File.ReadAllText("Assets/VRCSDK/version.txt");


        //select where to be imported (sdk)
        public static string assetPath = "Assets\\";
        //Custom name for downloaded unitypackage
        public static string assetName = "latest nanoSDK.unitypackage";
        //gets VRCSDK Directory Path
        public static string vrcsdkPath = "Assets\\VRCSDK\\";
        public async static void AutomaticSDKInstaller()
        {
            //Starting Browser
            HttpClient httpClient = new HttpClient();
            //Reading Version data
            var result = await httpClient.GetAsync(versionURL);
            var strServerVersion = await result.Content.ReadAsStringAsync();
            var serverVersion = strServerVersion;

            var thisVersion = currentVersion;

            try
            {
                //Checking if Uptodate or not
                if (serverVersion == thisVersion)
                {
                    //up to date
                    nanoLog("you are using the newest version of nanoSDK!");
                    EditorUtility.DisplayDialog("You are up to date",
                        "Current nanoSDK version: " + currentVersion,
                        "Okay"
                        );
                }
                else
                {
                    //not up to date
                    nanoLog("There is an Update Available");
                    //start download
                    await DownloadnanoSDK();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("[nanoSDK] AssetDownloadManager:" + ex.Message);
            }
        }

        public static async Task DownloadnanoSDK()
        {
            nanoLog("Asking for Approval..");
            if (EditorUtility.DisplayDialog("nanoSDK Updater", "Your Version (V" + currentVersion.ToString() + ") is Outdated!" + " do you want to Download and Import the Newest Version?", "Yes", "No"))
            {
                //starting deletion of old sdk
                await DeleteAndDownloadAsync();
            }
            else
            {
                //canceling the whole process
                nanoLog("You pressed no.");
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
                                    nanoLog($"{f} - Deleted");
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
                nanoLog($"{vrcsdkPath} - Deleted");
                //Delete Folder
                Directory.Delete(vrcsdkPath, true);
            }
            //Refresh
            AssetDatabase.Refresh();


            if (EditorUtility.DisplayDialog("nanoSDK_Automatic_DownloadAndInstall", "The New SDK Will be imported now!", "Nice!"))
            {
                //Creates WebClient to Download latest .unitypackage
                WebClient w = new WebClient();
                w.Headers.Set(HttpRequestHeader.UserAgent, "Webkit Gecko wHTTPS (Keep Alive 55)");
                w.DownloadFileCompleted += new AsyncCompletedEventHandler(fileDownloadComplete);
                w.DownloadProgressChanged += fileDownloadProgress;
                string url = unitypackageUrl;
                w.DownloadFileAsync(new Uri(url), assetName);
            }
        }

        private static void fileDownloadProgress(object sender, DownloadProgressChangedEventArgs e)
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

        private static void fileDownloadComplete(object sender, AsyncCompletedEventArgs e)
        {
            //Checks if Download is complete
            if (e.Error == null)
            {
                nanoLog("Download completed!");
                //Opens .unitypackage
                Process.Start(assetName);
            }
            else
            {
                //Asks to open Download Page Manually
                nanoLog("Download failed!");
                if (EditorUtility.DisplayDialog("nanoSDK_Automatic_DownloadAndInstall", "nanoSDK Failed to Download to latest Version", "Open URL instead", "Cancel"))
                {
                    Application.OpenURL(unitypackageUrl);
                }
            }
        }

        private static void nanoLog(string message)
        {
            //Our Logger
            Debug.Log("[nanoSDK] AssetDownloadManager: " + message);
        }
    }
}
