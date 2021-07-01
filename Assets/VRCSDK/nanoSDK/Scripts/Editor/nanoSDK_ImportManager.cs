using UnityEngine;
using System.IO;
using System.Net;
using System;
using System.ComponentModel;
using UnityEditor;

namespace nanoSDK
{
    public class nanoSDK_ImportManager
    {
        public static string configName = "importConfig.json";
        public static string serverUrl = "https://cdn.nanoSDK.net/assets/";
        public static string internalServerUrl = "https://cdn.nanoSDK.net/assets/";

        public static void downloadAndImportAssetFromServer(string assetName)
        {
            if (File.Exists(nanoSDK_Settings.getAssetPath() + assetName))
            {
                nanoLog(assetName + " exists. Importing it..");
                importDownloadedAsset(assetName);
            }
            else
            {
                nanoLog(assetName + " does not exist. Starting download..");
                downloadFile(assetName);
            }
        }

        private static void downloadFile(string assetName)
        {
            WebClient w = new WebClient();
            w.Headers.Set(HttpRequestHeader.UserAgent, "Webkit Gecko wHTTPS (Keep Alive 55)");
            w.QueryString.Add("assetName", assetName);
            w.DownloadFileCompleted += fileDownloadCompleted;
            w.DownloadProgressChanged += fileDownloadProgress;
            string url = serverUrl + assetName;
            w.DownloadFileAsync(new Uri(url), nanoSDK_Settings.getAssetPath() + assetName);
        }

        public static void deleteAsset(string assetName)
        {
            File.Delete(nanoSDK_Settings.getAssetPath() + assetName);
        }

        public static void updateConfig()
        {
            WebClient w = new WebClient();
            w.Headers.Set(HttpRequestHeader.UserAgent, "Webkit Gecko wHTTPS (Keep Alive 55)");
            w.DownloadFileCompleted += configDownloadCompleted;
            w.DownloadProgressChanged += fileDownloadProgress;
            string url = internalServerUrl + configName;
            w.DownloadFileAsync(new Uri(url), nanoSDK_Settings.projectConfigPath + "update_" + configName);
        }

        private static void configDownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                //var updateFile = File.ReadAllText(nanoSDK_Settings.projectConfigPath + "update_" + configName);
                File.Delete(nanoSDK_Settings.projectConfigPath + configName);
                File.Move(nanoSDK_Settings.projectConfigPath + "update_" + configName,
                    nanoSDK_Settings.projectConfigPath + configName);
                nanoSDK_ImportPanel.LoadJson();

                EditorPrefs.SetInt("nanoSDK_configImportLastUpdated", (int) DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                nanoLog("Import Config has been updated!");
            }
            else
            {
                nanoLog("Import Config could not be updated!");
            }
        }

        private static void fileDownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            string assetName = ((WebClient) sender).QueryString["assetName"];
            if (e.Error == null)
            {
                nanoLog("Download of file " + assetName + " completed!");
            }
            else
            {
                deleteAsset(assetName);
                nanoLog("Download of file " + assetName + " failed!");
            }
        }

        private static void fileDownloadProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            var progress = e.ProgressPercentage;
            var assetName = ((WebClient) sender).QueryString["assetName"];
            if (progress < 0) return;
            if (progress >= 100)
            {
                EditorUtility.ClearProgressBar();
            }
            else
            {
                EditorUtility.DisplayProgressBar("Download of " + assetName,
                    "Downloading " + assetName + ". Currently at: " + progress + "%",
                    (progress / 100F));
            }
        }

        public static void checkForConfigUpdate()
        {
            if (EditorPrefs.HasKey("nanoSDK_configImportLastUpdated"))
            {
                var lastUpdated = EditorPrefs.GetInt("nanoSDK_configImportLastUpdated");
                var currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                if (currentTime - lastUpdated < 3600)
                {
                    Debug.Log("Not updating config: " + (currentTime - lastUpdated));
                    return;
                }
            }
            nanoLog("Updating import config");
            updateConfig();
        }

        private static void nanoLog(string message)
        {
            Debug.Log("[nanoSDK] AssetDownloadManager: " + message);
        }

        public static void importDownloadedAsset(string assetName)
        {
            AssetDatabase.ImportPackage(nanoSDK_Settings.getAssetPath() + assetName, true);
        }
    }
}