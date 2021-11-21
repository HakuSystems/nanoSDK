using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace nanoSDK
{
    [InitializeOnLoad]
    public class NanoSDK_StartApi
    {
        
        static NanoSDK_StartApi()
        {
            StartProgram();
        }
        [InitializeOnLoadMethod]
        public static void StartProgram()
        {
            StartLoginWindow();
        }

        private static void StartLoginWindow()
        {
            const string k_ProjectOpened = "ProjectOpened";
            if (!SessionState.GetBool(k_ProjectOpened, false) && EditorApplication.isPlayingOrWillChangePlaymode == false)
            {
                SessionState.SetBool(k_ProjectOpened, true);
                EditorApplication.delayCall += () =>
                {
                    EditorWindow.GetWindow(typeof(NanoSDK_Login), true, "nanoSDKApi");
                };
            }
        }
    }
    public class NanoSDK_Login : EditorWindow
    {
        public static readonly HttpClient nanoHttpclient = new HttpClient(new HttpClientHandler { UseCookies = false});

        //gets this window
        public static NanoSDK_Login Instance { get; private set; }
        //public bool if window is open
        public static bool IsOpen => Instance != null;

        private static readonly int _sizeX = 500; //500
        private static readonly int _sizeY = 250; //250
        //Login
        public string UserinputText;
        public string PassinputText;
        //register
        public string regUserinputText;
        public string regPassinputText;
        public string regEmailInputText;

        public void OnEnable()
        {
            NanoLog("ENABLED");
            Instance = this;
            maxSize = new Vector2(_sizeX, _sizeY);
            minSize = maxSize;
        }
        /* After cookie thing is finished then this dont make like "ugh we cant use this bla" lol
        private void OnLostFocus()
        {
            EditorUtility.DisplayDialog("nanoSDK Api", "Window lost Focus", "Okay");
            NanoLog("LOST FOCUS");
            NanoLog("INVALID CONNECTION");
            GetUserLoggedIn("https://api.nanosdk.net/user/self");
        }

        private void OnDestroy()
        {
            EditorUtility.DisplayDialog("nanoSDK Api", "Window got Destroyed", "Okay");
            NanoLog("DESTROYED");
            NanoLog("INVALID CONNECTION");
            GetUserLoggedIn("https://api.nanosdk.net/user/self");
        }
        */

        public async void GetUserLoggedIn(string path)
        {
            string nanoCheckURL = path;
            nanoHttpclient.DefaultRequestHeaders.Add("Auth-Key", PlayerPrefs.GetString("nanoAuthKey"));
            var response = await nanoHttpclient.GetAsync(nanoCheckURL);
            string result = await response.Content.ReadAsStringAsync();

            var properties = JsonConvert.DeserializeObject<NanoUserData>(result);
            EditorUtility.DisplayDialog("server", properties.Username, "Okay");
            EditorUtility.DisplayDialog("unity", PlayerPrefs.GetString("nanoUsername"), "Okay");

            //irgendwie irgendwas falsch
            LoginnanoUser(PlayerPrefs.GetString("nanoUsername"), PlayerPrefs.GetString("nanoPassword"));
            if (properties.IsVerified)
            {
                NanoLog("VALID LICENSE");
                //do nothing
            }
            else
            {
                NanoLog("INVALID LICENSE");
                NanoSDK_License nanoSDK_License = (NanoSDK_License)ScriptableObject.CreateInstance(typeof(NanoSDK_License));
                nanoSDK_License.Show();
            }

        }

        void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Login");

            UserinputText = EditorGUILayout.TextField("Username", UserinputText);
            PassinputText = EditorGUILayout.PasswordField("Password", PassinputText);
            if (GUILayout.Button("Login"))
            {
                NanoLog("TRYING TO LOGIN");
                if (string.IsNullOrWhiteSpace(UserinputText) || string.IsNullOrWhiteSpace(PassinputText))
                {
                    EditorUtility.DisplayDialog("nanoSDK Api", "Credentials Cant be Empty!", "Okay");
                }
                else
                {
                    NanoLog("SAVING DATA");
                    LoginnanoUser(UserinputText, PassinputText);
                }
            }

            EditorGUILayout.LabelField("Register an Account");

            regUserinputText = EditorGUILayout.TextField("Username", regUserinputText);
            regPassinputText = EditorGUILayout.PasswordField("Password", regPassinputText);
            regEmailInputText = EditorGUILayout.TextField("Email", regEmailInputText);


            if (GUILayout.Button("Register"))
            {
                NanoLog("TRYING TO REGISTER");
                if (string.IsNullOrEmpty(regUserinputText) || string.IsNullOrEmpty(regPassinputText) || string.IsNullOrEmpty(regEmailInputText))
                {
                    EditorUtility.DisplayDialog("nanoSDK Api", "Credentials Cant be Empty!", "Okay");
                }
                else
                {
                    RegisternanoUser(regUserinputText, regPassinputText, regEmailInputText);
                }
            }
            EditorGUILayout.EndVertical();


        }

        private async void RegisternanoUser(string username, string password, string email)
        {

            NanoLog("WAITING FOR SERVER TO REPLY");
            var content = new StringContent(JsonConvert.SerializeObject(new APIRegisterData
            {
                Username = username,
                Password = password,
                Email = email
            }));
            string nanoRegisterURL = "https://api.nanosdk.net/user/signup";
            var response = await nanoHttpclient.PostAsync(nanoRegisterURL, content);
            NanoLog("GETTING SERVER DATA");
            string result = await response.Content.ReadAsStringAsync();
            var properties = JsonConvert.DeserializeObject<RegisterBase<SanityCheckResponse>>(result);

            if (properties.Message.Contains("Successfully executed sign up."))
            {
                NanoLog("SIGN UP COMPLETE");
                EditorUtility.DisplayDialog("nanoSDK Api", "Successfully Registered, autoLogin is being processed when pressed Okay", "Okay");
                LoginnanoUser(username, password);
            }
            else if (properties.Message.Contains("Sanity checks have failed."))
            {
                NanoLog("SOMETHING WENT WRONG ON SIGN UP");

                var sb = new StringBuilder();
                string usernameArray = null;
                string passwordArray = null;
                string emailArray = null;
                foreach (var item in properties.Data.UsernameSanityCheck)
                {
                    sb.AppendLine(item.Value);
                    usernameArray = sb.ToString();
                    
                }
                sb.Clear();
                foreach (var item in properties.Data.PasswordSanityCheck)
                {
                    sb.AppendLine(item.Value);
                    passwordArray = sb.ToString();
                }
                sb.Clear();
                foreach (var item in properties.Data.EmailSanityCheck)
                {
                    sb.AppendLine(item.Value);
                    emailArray = sb.ToString();
                }
                sb.Clear();
                if (string.IsNullOrEmpty(usernameArray)) { usernameArray = "Valid"; }
                if (string.IsNullOrEmpty(passwordArray)) { passwordArray = "Valid"; }
                if (string.IsNullOrEmpty(emailArray)) { emailArray = "Valid"; }

                if (EditorUtility.DisplayDialog("nanoSDK Api", "Username: " + Environment.NewLine + usernameArray.ToString() + Environment.NewLine + "Password: " + Environment.NewLine + passwordArray.ToString() + Environment.NewLine + "Email: " + Environment.NewLine + emailArray.ToString(), "Back"))
                {
                    FocusWindowIfItsOpen(typeof(NanoSDK_Login));
                    NanoSDK_Login window = (NanoSDK_Login)EditorWindow.GetWindow(typeof(NanoSDK_Login));
                    NanoLog("OPENING");
                    window.Show();
                }
            }
        }

        private async void LoginnanoUser(string username, string password)
        {
            var content = new StringContent(JsonConvert.SerializeObject(new APILoginData
            {
                Username = username,
                Password = password
            }));
            string nanoLoginURL = "https://api.nanosdk.net/user/login";
            var response = await nanoHttpclient.PostAsync(nanoLoginURL, content);
            string result = await response.Content.ReadAsStringAsync();

            var properties = JsonConvert.DeserializeObject<APIAuthLoginBase<APIAuthLoginAuthResponse>>(result);
            if (properties.message.Contains("Successfully executed login. Method used: Cookie. Cookie was returned"))
            {
                PlayerPrefs.SetString("nanoAuthKey", properties.Data.AuthKey);
                PlayerPrefs.SetString("nanoUsername", UserinputText);
                PlayerPrefs.SetString("nanoPassword", PassinputText);
                NanoLog("LOGIN FINISHED");
                try
                {
                    NanoLog("CLOSING");
                    Close();
                }
                catch (NullReferenceException)
                {
                    //NOTHING TO CATCH
                }
            }
            else
            {
                try
                {
                    LoginnanoUser(PlayerPrefs.GetString("nanoUsername"), PlayerPrefs.GetString("nanoPassword"));
                }
                catch (Exception ex)
                {
                    NanoLog("LOGIN WENT WRONG");
                    if (ex is NullReferenceException || ex is InvalidOperationException)
                    {
                        NanoSDK_Login window = (NanoSDK_Login)EditorWindow.GetWindow(typeof(NanoSDK_Login));
                        NanoLog("OPENING");
                        window.Show();
                    }
                }
            }
        }


        private void NanoLog(string msg)
        {
            Debug.Log("[nanoSDK API]: " + msg);
        }
    }

    public class NanoUserData
    {
        public string ID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public int Permission { get; set; }
        public bool IsVerified { get; set; }
        public bool IsPremium { get; set; }

    }

    public class APIRegisterData
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }

    public class APILoginData
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class RegisterBase<T>
    {
        public string Message { get; set; }
        public T Data { get; set; }
    }

    public class SanityCheckResponse
    {
        public Dictionary<string, string> UsernameSanityCheck { get; set; }
        public Dictionary<string, string> PasswordSanityCheck { get; set; }
        public Dictionary<string, string> EmailSanityCheck { get; set; }
    }

    public class APIAuthLoginBase<T>
    {
        public string message { get; set; }
        public T Data { get; set; }
    }
    public class APIAuthLoginAuthResponse
    {
        public string AuthKey { get; set; }
    }


    public class NanoSDK_License : EditorWindow
    {
        //gets this window
        public static NanoSDK_License Instance { get; private set; }
        //public bool if window is open
        public static bool IsOpen => Instance != null;

        private static readonly int _sizeX = 500; //500
        private static readonly int _sizeY = 80; //250
        //License
        public string LicenseinputText;


        public void OnEnable()
        {
            NanoLog("ENABLED LICENSE WINDOW");
            Instance = this;
            maxSize = new Vector2(_sizeX, _sizeY);
            minSize = maxSize;
        }
        /* ignore for development thing
        private void OnLostFocus()
        {
            EditorUtility.DisplayDialog("nanoSDK Api", "Window lost Focus", "Okay");
            NanoLog("LOST FOCUS");
        }
        private void OnDestroy()
        {
            EditorUtility.DisplayDialog("nanoSDK Api", "Window got Destroyed", "Okay");
            NanoLog("DESTROYED");
        }
        */
        void OnGUI()
        {
            NanoLog("REQUESTING DATA");

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("LICENSE KEY");
            LicenseinputText = EditorGUILayout.PasswordField("KEY", LicenseinputText);
            if (GUILayout.Button("CHECK VALID"))
            {
                NanoLog("CHECKING LICENSE KEY");
                if (string.IsNullOrWhiteSpace(LicenseinputText))
                {
                    EditorUtility.DisplayDialog("nanoSDK Api", "LicenseKey Cant be Empty!", "Okay");
                }
                else
                {
                    CheckIfLicenseKeyValidAsync(LicenseinputText);
                    NanoLog("SENDING DATA");
                }
            }
            EditorGUILayout.EndVertical();
        }

        private async void CheckIfLicenseKeyValidAsync(string licenseinputText)
        {
            var content = new StringContent(JsonConvert.SerializeObject(new APILicenseData
            {
                Key = licenseinputText
            }));
            string nanoLicenseURL = "https://api.nanosdk.net/user/redeemables/redeem";
            var response = await NanoSDK_Login.nanoHttpclient.PostAsync(nanoLicenseURL, content);
            string result = await response.Content.ReadAsStringAsync();

            var properties = JsonConvert.DeserializeObject<APILicenseData>(result);

            if (properties.Message.Contains("Redeem was successful"))
            {
                Close();
            }
            else
            {
                EditorUtility.DisplayDialog("nanoSDK Api", properties.Message, "Okay");
            }
        }

        private void NanoLog(string msg)
        {
            Debug.Log("[nanoSDK API - License]: " + msg);
        }

        public class APILicenseData
        {
            public string Message { get; set; }
            public string Key { get; set; }
        }
    }
}