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
using System.Collections.Generic;

namespace nanoSDK
{
    public class NanoSDK_AutomaticUpdateAndInstall : MonoBehaviour
    { //api features in here bc files will be delted when process is being made
        private static readonly HttpClient HttpClient = new HttpClient();

        private const string _BASE_URL = "https://api.nanosdk.net"; 
        private static readonly Uri _SdkVersionUri = new Uri(_BASE_URL + "/public/sdk/version/list");

        public static string CurrentVersion { get; set; } = File.ReadAllText("Assets/VRCSDK/version.txt").Replace(" ", "").Replace("\n", "");
        public static List<SdkVersionBaseINTERNDATA> SERVERVERSIONLIST {get; set;}
        

        //select where to be imported (sdk)
        public static string assetPath = "Assets\\";
        //Custom name for downloaded unitypackage
        public static string assetName = "unitypackage";
        //gets VRCSDK Directory Path
        public static string vrcsdkPath = "Assets\\VRCSDK\\";


        //[MenuItem("nanoSDK/Update Test", false, 500)]
        public static async void CheckServerVersionINTERN()
        {
            CurrentVersion = File.ReadAllText("Assets/VRCSDK/version.txt").Replace(" ", "").Replace("\n", "");

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = _SdkVersionUri
            };

            using (var response = await HttpClient.SendAsync(request))
            {
                string result = await response.Content.ReadAsStringAsync();
                var SERVERCHECKproperties = JsonConvert.DeserializeObject<SdkVersionBaseINTERN<List<SdkVersionBaseINTERNDATA>>>(result);
                SERVERVERSIONLIST = SERVERCHECKproperties.Data;
            } //without AuthKey Sending



            // foreach(SdkVersionBaseINTERNDATA idata in SERVERVERSIONLIST) {
            //     NanoLog(idata.Version);
            // }
            //Debug.Log($"!{SERVERVERSIONLIST[0].Version}!{CurrentVersion}!");
            if (!CurrentVersion.Equals(SERVERVERSIONLIST[0].Version))
            {
                await DownloadnanoSDK("latest");
            }
            else
            {
                EditorUtility.DisplayDialog("You are up to date",
                    "Current nanoSDK version: V" + CurrentVersion,
                    "Okay"
                    );
            }
        }

        public static async Task DownloadnanoSDK(string version)
        {
            if (EditorUtility.DisplayDialog("nanoSDK", "This is Still indevelopment we will announce it when its finished.", "okay"))
            {
                NanoLog("Cancel");
            }
            return; //Still in Developement
            NanoLog("Asking user for update Approval..");
            if (EditorUtility.DisplayDialog("nanoSDK Updater", "Your Version (V" + CurrentVersion.ToString() + ") is Outdated!" + " do you want to Download and Import the Newest Version?", "Yes", "No"))
            {
                //starting deletion of old sdk
                await DeleteAndDownloadAsync("latest");
            }
            else
            {
                //canceling the whole process
                NanoLog("User declined update");
            }
        }

        public static async Task DeleteAndDownloadAsync(string version = "latest")
        {
            if (EditorUtility.DisplayDialog("nanoSDK", "This is Still indevelopment we will announce it when its finished.", "okay"))
            {
                NanoLog("Cancel");

            }
            return;

            WebClient w = new WebClient();
            w.Headers.Set(HttpRequestHeader.UserAgent, "Webkit Gecko wHTTPS (Keep Alive 55)");
            //w.DownloadFileCompleted += new AsyncCompletedEventHandler(FileDownloadComplete);
            w.DownloadProgressChanged += FileDownloadProgress;
            try
            {
                string url = GetUrlFromVersion(version);
                if (url == null) throw new Exception("Invalid version");
                await w.DownloadFileTaskAsync(new Uri(url), Path.GetTempPath() + "\\" + $"{version}.{assetName}");
            }
            catch (Exception ex)
            {
                NanoLog("Download failed!");
                if (EditorUtility.DisplayDialog("nanoSDK_Automatic_DownloadAndInstall", "nanoSDK Failed Download: " + ex.Message, "Join Discord for help", "Cancel"))
                {
                    Application.OpenURL("https://nanosdk.net/discord");
                }
                return;
            }
           
            NanoLog("Download Complete");
            
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
            NanoLog("sffsdfsdfsdfsdfdsf");
            try {
                Process.Start(Path.GetTempPath() + "\\" + $"{version}.{assetName}");

            } catch (Exception ex) {

                NanoLog("Download failed!");
                if (EditorUtility.DisplayDialog("nanoSDK_Automatic_DownloadAndInstall", "nanoSDK Failed Download: " + ex.Message, "Join Discord for help", "Cancel"))
                {
                    Application.OpenURL("https://nanosdk.net/discord");
                }
                return;
            }
            AssetDatabase.Refresh();


        }

        private static string GetUrlFromVersion(string version)
        {
            string url = null;
            if (version.Equals("latest")) url = SERVERVERSIONLIST[0].Url;
            else if (version.Equals("beta")) url = SERVERVERSIONLIST[SERVERVERSIONLIST.Count - 1].Url; 

            for (int i = 0; i < SERVERVERSIONLIST.Count; i++)
            {
                if(version.Equals(SERVERVERSIONLIST[i].Version)) url = SERVERVERSIONLIST[i].Url;
            }
            return url;
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

    public class SdkVersionBaseINTERNDATA
    {
        public string Url { get; set; }
        public string Version { get; set; }
        public ReleaseType Type { get; set; }

        public BranchType Branch { get; set; }

        public enum ReleaseType
        {
            Avatar = 0,
            World = 1
        }

        public enum BranchType
        {
            Release = 0,
            Beta = 1
        }
    }

    public class SdkVersionBaseINTERN<T>
    {
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
