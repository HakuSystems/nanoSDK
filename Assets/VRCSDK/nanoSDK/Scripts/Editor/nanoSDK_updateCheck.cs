using nanoSDK;
using System.IO;
using System.Net.Http;
using UnityEditor;
using UnityEngine;

public class NanoSDK_updateCheck : MonoBehaviour
{
    [InitializeOnLoad]
    public class Startup
    {
        public static string versionURL = "https://nanosdk.net/download/Version/version.txt";
        public static string currentVersion = File.ReadAllText("Assets/VRCSDK/version.txt");
        static Startup()
        {
            Check();
        }
        public async static void Check()
        {
            HttpClient httpClient = new HttpClient();
            var result = await httpClient.GetAsync(versionURL);
            var strServerVersion = await result.Content.ReadAsStringAsync();
            var serverVersion = strServerVersion;

            var thisVersion = currentVersion;
            
            if (serverVersion != thisVersion)
            {
                NanoSDK_AutomaticUpdateAndInstall.AutomaticSDKInstaller();
            }
        }
    }
}
