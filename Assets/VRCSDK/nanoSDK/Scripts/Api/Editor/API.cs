﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using UnityEditor;
using SystemInfo = UnityEngine.SystemInfo;

namespace nanoSDK
{
    internal class App
    {
        public static string GrabVariable(string name)
        {
            try
            {
                if (User.ID != null || User.HWID != null || User.IP != null || !Constants.Breached)
                {
                    return Variables[name];
                }
                else
                {
                    Constants.Breached = true;
                    return "User is not logged in, possible breach detected!";
                }
            }
            catch
            {
                return "N/A";
            }
        }
        public static string Error = null;
        public static Dictionary<string, string> Variables = new Dictionary<string, string>();
    }
    internal class Constants
    {
        public static string Token { get; set; }

        public static string Date { get; set; }

        public static string APIENCRYPTKEY { get; set; }

        public static string APIENCRYPTSALT { get; set; }

        public static bool Breached = false;

        public static bool Started = false;

        public static string IV = null;

        public static string Key = null;

        public static string ApiUrl = "https://api.auth.gg/csharp/";

        public static bool Initialized = false;

        public static Random random = new Random();

        public static string RandomString(int length)
        {
            return new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string HWID()
        {
            return SystemInfo.deviceUniqueIdentifier;
        }
    }
    internal class User
    {
        public static string ID { get; set; }

        public static string Username { get; set; }

        public static string Password { get; set; }

        public static string Email { get; set; }

        public static string HWID { get; set; }

        public static string IP { get; set; }

        public static string UserVariable { get; set; }

        public static string Rank { get; set; }

        public static string Expiry { get; set; }

        public static string LastLogin { get; set; }

        public static string RegisterDate { get; set; }
    }
    internal class ApplicationSettings
    {
        public static bool Status { get; set; }

        public static bool DeveloperMode { get; set; }

        public static string Hash { get; set; }

        public static string Version { get; set; }

        public static string Update_Link { get; set; }

        public static bool Freemode { get; set; }

        public static bool Login { get; set; }

        public static string Name { get; set; }

        public static bool Register { get; set; }
    }

    internal class OnProgramStart
    {
        public static string AID = null;

        public static string Secret = null;

        public static string Version = null;

        public static string Name = null;

        public static string Salt = null;

        public static void Initialize(string name, string aid, string secret, string version)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(aid) || string.IsNullOrWhiteSpace(secret) || string.IsNullOrWhiteSpace(version))
            {
                EditorUtility.DisplayDialog("nanoSDK Api", "Invalid application information!", "Okay");
                //MessageBox.Show("Invalid application information!", Name, MessageBoxButton.OK, MessageBoxImage.Error);
                Process.GetCurrentProcess().Kill();
            }
            AID = aid;
            Secret = secret;
            Version = version;
            Name = name;
            string[] response = new string[] { };
            using (WebClient wc = new WebClient())
            {

                try
                {
                    wc.Proxy = null;
                    Security.Start();
                    response = (Encryption.DecryptService(Encoding.Default.GetString(wc.UploadValues(Constants.ApiUrl, new NameValueCollection
                    {
                        ["token"] = Encryption.EncryptService(Constants.Token),
                        ["timestamp"] = Encryption.EncryptService(DateTime.Now.ToString()),
                        ["aid"] = Encryption.APIService(AID),
                        ["session_id"] = Constants.IV,
                        ["api_id"] = Constants.APIENCRYPTSALT,
                        ["api_key"] = Constants.APIENCRYPTKEY,
                        ["session_key"] = Constants.Key,
                        ["secret"] = Encryption.APIService(Secret),
                        ["type"] = Encryption.APIService("start")

                    }))).Split("|".ToCharArray()));
                    if (Security.MaliciousCheck(response[1]))
                    {
                        EditorUtility.DisplayDialog("nanoSDK Api", "Possible malicious activity detected!", "Ignore");
                        //MessageBox.Show("Possible malicious activity detected!", OnProgramStart.Name, MessageBoxButton.OK, MessageBoxImage.Warning);
                        //Process.GetCurrentProcess().Kill();
                    }
                    if (Constants.Breached)
                    {
                        EditorUtility.DisplayDialog("nanoSDK Api", "Possible malicious activity detected!", "Ignore");
                        //MessageBox.Show("Possible malicious activity detected!", OnProgramStart.Name, MessageBoxButton.OK, MessageBoxImage.Warning);
                        //Process.GetCurrentProcess().Kill();
                    }
                    if (response[0] != Constants.Token)
                    {
                        EditorUtility.DisplayDialog("nanoSDK Api", "Security error has been triggered!", "Okay");
                        //MessageBox.Show("Security error has been triggered!", Name, MessageBoxButton.OK, MessageBoxImage.Error);
                        Process.GetCurrentProcess().Kill();
                    }
                    switch (response[2])
                    {
                        case "success":
                            Constants.Initialized = true;
                            if (response[3] == "Enabled")
                                ApplicationSettings.Status = true;
                            if (response[4] == "Enabled")
                                ApplicationSettings.DeveloperMode = true;
                            ApplicationSettings.Hash = response[5];
                            ApplicationSettings.Version = response[6];
                            ApplicationSettings.Update_Link = response[7];
                            if (response[8] == "Enabled")
                                ApplicationSettings.Freemode = true;
                            if (response[9] == "Enabled")
                                ApplicationSettings.Login = true;
                            ApplicationSettings.Name = response[10];
                            if (response[11] == "Enabled")
                                ApplicationSettings.Register = true;
                            if (ApplicationSettings.DeveloperMode)
                            {
                                EditorUtility.DisplayDialog("nanoSDK Api", "Application is in Developer Mode, bypassing integrity and update check!", "Okay");
                                //MessageBox.Show("Application is in Developer Mode, bypassing integrity and update check!", Name, MessageBoxButton.OK, MessageBoxImage.Warning);
                                File.Create(Environment.CurrentDirectory + "/integrity.log").Close();
                                string hash = Security.Integrity(Process.GetCurrentProcess().MainModule.FileName);
                                File.WriteAllText(Environment.CurrentDirectory + "/integrity.log", hash);
                                EditorUtility.DisplayDialog("nanoSDK Api", "Your applications hash has been saved to integrity.txt, please refer to this when your application is ready for release!", "Okay");
                                //MessageBox.Show("Your applications hash has been saved to integrity.txt, please refer to this when your application is ready for release!", Name, MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                if (response[12] == "Enabled")
                                {
                                    if (ApplicationSettings.Hash != Security.Integrity(Process.GetCurrentProcess().MainModule.FileName))
                                    {
                                        EditorUtility.DisplayDialog("nanoSDK Api", "File has been tampered with, couldn't verify integrity!", "Okay");
                                        //MessageBox.Show($"File has been tampered with, couldn't verify integrity!", Name, MessageBoxButton.OK, MessageBoxImage.Error);
                                        Process.GetCurrentProcess().Kill();
                                    }
                                }
                                if (ApplicationSettings.Version != Version)
                                {
                                    EditorUtility.DisplayDialog("nanoSDK Api", $"Update {ApplicationSettings.Version} available, redirecting to update!", "Okay");
                                    //MessageBox.Show($"Update {ApplicationSettings.Version} available, redirecting to update!", Name, MessageBoxButton.OK, MessageBoxImage.Error);
                                    Process.Start(ApplicationSettings.Update_Link);
                                    Process.GetCurrentProcess().Kill();
                                }

                            }
                            if (ApplicationSettings.Status == false)
                            {
                                EditorUtility.DisplayDialog("nanoSDK Api", "Looks like this application is disabled, please try again later!", "Okay");
                                //MessageBox.Show("Looks like this application is disabled, please try again later!", Name, MessageBoxButton.OK, MessageBoxImage.Error);
                                Process.GetCurrentProcess().Kill();
                            }
                            break;
                        case "binderror":
                            EditorUtility.DisplayDialog("nanoSDK Api", Encryption.Decode("RmFpbGVkIHRvIGJpbmQgdG8gc2VydmVyLCBjaGVjayB5b3VyIEFJRCAmIFNlY3JldCBpbiB5b3VyIGNvZGUh"), "Okay");
                            Process.GetCurrentProcess().Kill();
                            return;
                        case "banned":
                            EditorUtility.DisplayDialog("nanoSDK Api", "This application has been banned for violating the TOS" + Environment.NewLine + "Contact us at support@auth.gg", "Okay");
                            Process.GetCurrentProcess().Kill();
                            return;
                    }
                    Security.End();
                }
                catch (Exception ex)
                {
                    EditorUtility.DisplayDialog("nanoSDK Api", ex.Message, "Okay");
                    Process.GetCurrentProcess().Kill();
                }
            }
        }
    }

    internal class API
    {
        public static void Log(string username, string action)
        {
            if (!Constants.Initialized)
            {
                EditorUtility.DisplayDialog("nanoSDK Api", "Please initialize your application first!", "Okay");
                Process.GetCurrentProcess().Kill();
            }
            if (string.IsNullOrWhiteSpace(action))
            {
                EditorUtility.DisplayDialog("nanoSDK Api", "Missing log information!", "Okay");
                Process.GetCurrentProcess().Kill();
            }
            string[] response = new string[] { };
            using (WebClient wc = new WebClient())
            {
                try
                {
                    Security.Start();
                    wc.Proxy = null;
                    response = (Encryption.DecryptService(Encoding.Default.GetString(wc.UploadValues(Constants.ApiUrl, new NameValueCollection
                    {
                        ["token"] = Encryption.EncryptService(Constants.Token),
                        ["aid"] = Encryption.APIService(OnProgramStart.AID),
                        ["username"] = Encryption.APIService(username),
                        ["pcuser"] = Encryption.APIService(Environment.UserName),
                        ["session_id"] = Constants.IV,
                        ["api_id"] = Constants.APIENCRYPTSALT,
                        ["api_key"] = Constants.APIENCRYPTKEY,
                        ["data"] = Encryption.APIService(action),
                        ["session_key"] = Constants.Key,
                        ["secret"] = Encryption.APIService(OnProgramStart.Secret),
                        ["type"] = Encryption.APIService("log")
                    }))).Split("|".ToCharArray()));
                    Security.End();
                }
                catch (Exception ex)
                {
                    EditorUtility.DisplayDialog("nanoSDK Api", ex.Message, "Okay");
                    Process.GetCurrentProcess().Kill();
                }
            }
        }
        public static bool AIO(string AIO)
        {
            if (AIOLogin(AIO))
            {
                return true;
            }
            else
            {
                if (AIORegister(AIO))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public static bool AIOLogin(string AIO)
        {
            if (!Constants.Initialized)
            {
                EditorUtility.DisplayDialog("nanoSDK Api", "Please initialize your application first!", "Okay");
                Process.GetCurrentProcess().Kill();
            }
            if (string.IsNullOrWhiteSpace(AIO))
            {
                EditorUtility.DisplayDialog("nanoSDK Api", "Missing user login information!", "Okay");
                Process.GetCurrentProcess().Kill();
            }
            string[] response = new string[] { };
            using (WebClient wc = new WebClient())
            {
                try
                {
                    Security.Start();
                    wc.Proxy = null;
                    response = (Encryption.DecryptService(Encoding.Default.GetString(wc.UploadValues(Constants.ApiUrl, new NameValueCollection
                    {
                        ["token"] = Encryption.EncryptService(Constants.Token),
                        ["timestamp"] = Encryption.EncryptService(DateTime.Now.ToString()),
                        ["aid"] = Encryption.APIService(OnProgramStart.AID),
                        ["session_id"] = Constants.IV,
                        ["api_id"] = Constants.APIENCRYPTSALT,
                        ["api_key"] = Constants.APIENCRYPTKEY,
                        ["username"] = Encryption.APIService(AIO),
                        ["password"] = Encryption.APIService(AIO),
                        ["hwid"] = Encryption.APIService(Constants.HWID()),
                        ["session_key"] = Constants.Key,
                        ["secret"] = Encryption.APIService(OnProgramStart.Secret),
                        ["type"] = Encryption.APIService("login")

                    }))).Split("|".ToCharArray()));
                    if (response[0] != Constants.Token)
                    {
                        EditorUtility.DisplayDialog("nanoSDK Api", "Security error has been triggered!", "Okay");
                        Process.GetCurrentProcess().Kill();
                    }
                    if (Security.MaliciousCheck(response[1]))
                    {
                        EditorUtility.DisplayDialog("nanoSDK Api", "Possible malicious activity detected!", "Okay");
                        Process.GetCurrentProcess().Kill();
                    }
                    if (Constants.Breached)
                    {
                        EditorUtility.DisplayDialog("nanoSDK Api", "Possible malicious activity detected!", "Okay");
                        Process.GetCurrentProcess().Kill();
                    }
                    switch (response[2])
                    {
                        case "success":
                            Security.End();
                            User.ID = response[3];
                            User.Username = response[4];
                            User.Password = response[5];
                            User.Email = response[6];
                            User.HWID = response[7];
                            User.UserVariable = response[8];
                            User.Rank = response[9];
                            User.IP = response[10];
                            User.Expiry = response[11];
                            User.LastLogin = response[12];
                            User.RegisterDate = response[13];
                            string Variables = response[14];
                            foreach (string var in Variables.Split('~'))
                            {
                                string[] items = var.Split('^');
                                try
                                {
                                    App.Variables.Add(items[0], items[1]);
                                }
                                catch
                                {
                                    //If some are null or not loaded, just ignore.
                                    //Error will be shown when loading the variable anyways
                                }
                            }
                            return true;
                        case "invalid_details":
                            Security.End();
                            return false;
                        case "time_expired":
                            EditorUtility.DisplayDialog("nanoSDK Api", "Your subscription has expired!", "Okay");
                            Security.End();
                            return false;
                        case "hwid_updated":
                            EditorUtility.DisplayDialog("nanoSDK Api", "New machine has been binded, re-open the application!", "Okay");
                            Security.End();
                            return false;
                        case "invalid_hwid":
                            EditorUtility.DisplayDialog("nanoSDK Api", "This user is binded to another computer, please contact support!", "Okay");
                            Security.End();
                            return false;
                    }
                }
                catch (Exception ex)
                {
                    EditorUtility.DisplayDialog("nanoSDK Api", ex.Message, "Okay");
                    Security.End();
                    Process.GetCurrentProcess().Kill();
                }
                return false;

            }
        }
        public static bool AIORegister(string AIO)
        {
            if (!Constants.Initialized)
            {
                EditorUtility.DisplayDialog("nanoSDK Api", "Please initialize your application first!", "Okay");
                Security.End();
                Process.GetCurrentProcess().Kill();
            }
            if (string.IsNullOrWhiteSpace(AIO))
            {
                EditorUtility.DisplayDialog("nanoSDK Api", "Invalid registrar information!", "Okay");
                Process.GetCurrentProcess().Kill();
            }
            string[] response = new string[] { };
            using (WebClient wc = new WebClient())
            {
                try
                {
                    Security.Start();
                    wc.Proxy = null;

                    response = Encryption.DecryptService(Encoding.Default.GetString(wc.UploadValues(Constants.ApiUrl, new NameValueCollection
                    {
                        ["token"] = Encryption.EncryptService(Constants.Token),
                        ["timestamp"] = Encryption.EncryptService(DateTime.Now.ToString()),
                        ["aid"] = Encryption.APIService(OnProgramStart.AID),
                        ["session_id"] = Constants.IV,
                        ["api_id"] = Constants.APIENCRYPTSALT,
                        ["api_key"] = Constants.APIENCRYPTKEY,
                        ["session_key"] = Constants.Key,
                        ["secret"] = Encryption.APIService(OnProgramStart.Secret),
                        ["type"] = Encryption.APIService("register"),
                        ["username"] = Encryption.APIService(AIO),
                        ["password"] = Encryption.APIService(AIO),
                        ["email"] = Encryption.APIService(AIO),
                        ["license"] = Encryption.APIService(AIO),
                        ["hwid"] = Encryption.APIService(Constants.HWID()),

                    }))).Split("|".ToCharArray());
                    if (response[0] != Constants.Token)
                    {
                        EditorUtility.DisplayDialog("nanoSDK Api", "Security error has been triggered!", "Okay");
                        Security.End();
                        Process.GetCurrentProcess().Kill();
                    }
                    if (Security.MaliciousCheck(response[1]))
                    {
                        EditorUtility.DisplayDialog("nanoSDK Api", "Possible malicious activity detected!", "Okay");
                        Process.GetCurrentProcess().Kill();
                    }
                    if (Constants.Breached)
                    {
                        EditorUtility.DisplayDialog("nanoSDK Api", "Possible malicious activity detected!", "Okay");
                        Process.GetCurrentProcess().Kill();
                    }
                    Security.End();
                    switch (response[2])
                    {
                        case "success":
                            return true;
                        case "error":
                            return false;

                    }
                }
                catch (Exception ex)
                {
                    EditorUtility.DisplayDialog("nanoSDK Api", ex.Message, "Okay");
                    Process.GetCurrentProcess().Kill();
                }
                return false;
            }
        }
        public static bool Login(string username, string password)
        {
            if (!Constants.Initialized)
            {
                EditorUtility.DisplayDialog("nanoSDK Api", "Please initialize your application first!", "Okay");
                Process.GetCurrentProcess().Kill();
            }
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                EditorUtility.DisplayDialog("nanoSDK Api", "Missing user login information!", "Okay");
                Process.GetCurrentProcess().Kill();
            }
            string[] response = new string[] { };
            using (WebClient wc = new WebClient())
            {
                try
                {
                    Security.Start();
                    wc.Proxy = null;
                    response = (Encryption.DecryptService(Encoding.Default.GetString(wc.UploadValues(Constants.ApiUrl, new NameValueCollection
                    {
                        ["token"] = Encryption.EncryptService(Constants.Token),
                        ["timestamp"] = Encryption.EncryptService(DateTime.Now.ToString()),
                        ["aid"] = Encryption.APIService(OnProgramStart.AID),
                        ["session_id"] = Constants.IV,
                        ["api_id"] = Constants.APIENCRYPTSALT,
                        ["api_key"] = Constants.APIENCRYPTKEY,
                        ["username"] = Encryption.APIService(username),
                        ["password"] = Encryption.APIService(password),
                        ["hwid"] = Encryption.APIService(Constants.HWID()),
                        ["session_key"] = Constants.Key,
                        ["secret"] = Encryption.APIService(OnProgramStart.Secret),
                        ["type"] = Encryption.APIService("login")

                    }))).Split("|".ToCharArray()));
                    if (response[0] != Constants.Token)
                    {
                        EditorUtility.DisplayDialog("nanoSDK Api", "Security error has been triggered!", "Okay");
                        Process.GetCurrentProcess().Kill();
                    }
                    if (Security.MaliciousCheck(response[1]))
                    {
                        EditorUtility.DisplayDialog("nanoSDK Api", "Possible malicious activity detected!", "Okay");
                        Process.GetCurrentProcess().Kill();
                    }
                    if (Constants.Breached)
                    {
                        EditorUtility.DisplayDialog("nanoSDK Api", "Possible malicious activity detected!", "Okay");
                        Process.GetCurrentProcess().Kill();
                    }
                    switch (response[2])
                    {
                        case "success":
                            User.ID = response[3];
                            User.Username = response[4];
                            User.Password = response[5];
                            User.Email = response[6];
                            User.HWID = response[7];
                            User.UserVariable = response[8];
                            User.Rank = response[9];
                            User.IP = response[10];
                            User.Expiry = response[11];
                            User.LastLogin = response[12];
                            User.RegisterDate = response[13];
                            string Variables = response[14];
                            foreach (string var in Variables.Split('~'))
                            {
                                string[] items = var.Split('^');
                                try
                                {
                                    App.Variables.Add(items[0], items[1]);
                                }
                                catch
                                {
                                    //If some are null or not loaded, just ignore.
                                    //Error will be shown when loading the variable anyways
                                }
                            }
                            Security.End();
                            return true;
                        case "invalid_details":
                            EditorUtility.DisplayDialog("nanoSDK Api", "Sorry, your username/password does not match!", "Okay");
                            Security.End();
                            return false;
                        case "time_expired":
                            EditorUtility.DisplayDialog("nanoSDK Api", "Your subscription has expired!", "Okay");
                            Security.End();
                            return false;
                        case "hwid_updated":
                            EditorUtility.DisplayDialog("nanoSDK Api", "New machine has been binded, re-open the application!", "Okay");
                            Security.End();
                            return false;
                        case "invalid_hwid":
                            EditorUtility.DisplayDialog("nanoSDK Api", "This user is binded to another computer, please contact support!", "Okay");
                            Security.End();
                            return false;
                    }
                }
                catch (Exception ex)
                {
                    EditorUtility.DisplayDialog("nanoSDK Api", ex.Message, "Okay");
                    Security.End();
                    Process.GetCurrentProcess().Kill();
                }
                return false;

            }
        }
        public static bool Register(string username, string password, string email, string license)
        {
            if (!Constants.Initialized)
            {
                EditorUtility.DisplayDialog("nanoSDK Api", "Please initialize your application first!", "Okay");
                Security.End();
                Process.GetCurrentProcess().Kill();
            }
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(license))
            {
                EditorUtility.DisplayDialog("nanoSDK Api", "Invalid registrar information!", "Okay");
                Process.GetCurrentProcess().Kill();
            }
            string[] response = new string[] { };
            using (WebClient wc = new WebClient())
            {
                try
                {
                    Security.Start();
                    wc.Proxy = null;

                    response = Encryption.DecryptService(Encoding.Default.GetString(wc.UploadValues(Constants.ApiUrl, new NameValueCollection
                    {
                        ["token"] = Encryption.EncryptService(Constants.Token),
                        ["timestamp"] = Encryption.EncryptService(DateTime.Now.ToString()),
                        ["aid"] = Encryption.APIService(OnProgramStart.AID),
                        ["session_id"] = Constants.IV,
                        ["api_id"] = Constants.APIENCRYPTSALT,
                        ["api_key"] = Constants.APIENCRYPTKEY,
                        ["session_key"] = Constants.Key,
                        ["secret"] = Encryption.APIService(OnProgramStart.Secret),
                        ["type"] = Encryption.APIService("register"),
                        ["username"] = Encryption.APIService(username),
                        ["password"] = Encryption.APIService(password),
                        ["email"] = Encryption.APIService(email),
                        ["license"] = Encryption.APIService(license),
                        ["hwid"] = Encryption.APIService(Constants.HWID()),

                    }))).Split("|".ToCharArray());
                    if (response[0] != Constants.Token)
                    {
                        EditorUtility.DisplayDialog("nanoSDK Api", "Security error has been triggered!", "Okay");
                        Security.End();
                        Process.GetCurrentProcess().Kill();
                    }
                    if (Security.MaliciousCheck(response[1]))
                    {
                        EditorUtility.DisplayDialog("nanoSDK Api", "Possible malicious activity detected!", "Okay");
                        Process.GetCurrentProcess().Kill();
                    }
                    if (Constants.Breached)
                    {
                        EditorUtility.DisplayDialog("nanoSDK Api", "Possible malicious activity detected!", "Okay");
                        Process.GetCurrentProcess().Kill();
                    }
                    switch (response[2])
                    {
                        case "success":
                            Security.End();
                            return true;
                        case "invalid_license":
                            EditorUtility.DisplayDialog("nanoSDK Api", "License does not exist!", "Okay");
                            Security.End();
                            return false;
                        case "email_used":
                            EditorUtility.DisplayDialog("nanoSDK Api", "Email has already been used!", "Okay");
                            Security.End();
                            return false;
                        case "invalid_username":
                            EditorUtility.DisplayDialog("nanoSDK Api", "You entered an invalid/used username!", "Okay");
                            Security.End();
                            return false;
                    }
                }
                catch (Exception ex)
                {
                    EditorUtility.DisplayDialog("nanoSDK Api", ex.Message, "Okay");
                    Process.GetCurrentProcess().Kill();
                }
                return false;
            }
        }
        public static bool ExtendSubscription(string username, string password, string license)
        {
            if (!Constants.Initialized)
            {
                EditorUtility.DisplayDialog("nanoSDK Api", "Please initialize your application first!", "Okay");
                Security.End();
                Process.GetCurrentProcess().Kill();
            }
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(license))
            {
                EditorUtility.DisplayDialog("nanoSDK Api", "Invalid registrar information!", "Okay");
                Process.GetCurrentProcess().Kill();
            }
            string[] response = new string[] { };
            using (WebClient wc = new WebClient())
            {
                try
                {
                    Security.Start();
                    wc.Proxy = null;
                    response = Encryption.DecryptService(Encoding.Default.GetString(wc.UploadValues(Constants.ApiUrl, new NameValueCollection
                    {
                        ["token"] = Encryption.EncryptService(Constants.Token),
                        ["timestamp"] = Encryption.EncryptService(DateTime.Now.ToString()),
                        ["aid"] = Encryption.APIService(OnProgramStart.AID),
                        ["session_id"] = Constants.IV,
                        ["api_id"] = Constants.APIENCRYPTSALT,
                        ["api_key"] = Constants.APIENCRYPTKEY,
                        ["session_key"] = Constants.Key,
                        ["secret"] = Encryption.APIService(OnProgramStart.Secret),
                        ["type"] = Encryption.APIService("extend"),
                        ["username"] = Encryption.APIService(username),
                        ["password"] = Encryption.APIService(password),
                        ["license"] = Encryption.APIService(license),

                    }))).Split("|".ToCharArray());
                    if (response[0] != Constants.Token)
                    {
                        EditorUtility.DisplayDialog("nanoSDK Api", "Security error has been triggered!", "Okay");
                        Security.End();
                        Process.GetCurrentProcess().Kill();
                    }
                    if (Security.MaliciousCheck(response[1]))
                    {
                        EditorUtility.DisplayDialog("nanoSDK Api", "Possible malicious activity detected!", "Okay");
                        Process.GetCurrentProcess().Kill();
                    }
                    if (Constants.Breached)
                    {
                        EditorUtility.DisplayDialog("nanoSDK Api", "Possible malicious activity detected!", "Okay");
                        Process.GetCurrentProcess().Kill();
                    }
                    switch (response[2])
                    {
                        case "success":
                            Security.End();
                            return true;
                        case "invalid_token":
                            EditorUtility.DisplayDialog("nanoSDK Api", "Token does not exist!", "Okay");
                            Security.End();
                            return false;
                        case "invalid_details":
                            EditorUtility.DisplayDialog("nanoSDK Api", "Your user details are invalid!", "Okay");
                            Security.End();
                            return false;
                    }
                }
                catch (Exception ex)
                {
                    EditorUtility.DisplayDialog("nanoSDK Api", ex.Message, "Okay");
                    Process.GetCurrentProcess().Kill();
                }
                return false;
            }
        }
    }
    internal class Security
    {
        public static string Signature(string value)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] input = Encoding.UTF8.GetBytes(value);
                byte[] hash = md5.ComputeHash(input);
                return BitConverter.ToString(hash).Replace("-", "");
            }
        }
        private static string Session(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, length)
             .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static string Obfuscate(int length)
        {
            Random random = new Random();
            const string chars = "gd8JQ57nxXzLLMPrLylVhxoGnWGCFjO4knKTfRE6mVvdjug2NF/4aptAsZcdIGbAPmcx0O+ftU/KvMIjcfUnH3j+IMdhAW5OpoX3MrjQdf5AAP97tTB5g1wdDSAqKpq9gw06t3VaqMWZHKtPSuAXy0kkZRsc+DicpcY8E9+vWMHXa3jMdbPx4YES0p66GzhqLd/heA2zMvX8iWv4wK7S3QKIW/a9dD4ALZJpmcr9OOE=";
            return new string(Enumerable.Repeat(chars, length)
             .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static void Start()
        {
            string drive = Path.GetPathRoot(Environment.SystemDirectory);
            if (Constants.Started)
            {
                EditorUtility.DisplayDialog("nanoSDK Api", "A session has already been started, please end the previous one!", "Okay");
                Process.GetCurrentProcess().Kill();
            }
            else
            {
                using (StreamReader sr = new StreamReader($@"{drive}Windows\System32\drivers\etc\hosts"))
                {
                    string contents = sr.ReadToEnd();
                    if (contents.Contains("api.auth.gg"))
                    {
                        Constants.Breached = true;
                        EditorUtility.DisplayDialog("nanoSDK Api", "DNS redirecting has been detected!", "Okay");
                        Process.GetCurrentProcess().Kill();
                    }
                }
                InfoManager infoManager = new InfoManager();
                infoManager.StartListener();
                Constants.Token = Guid.NewGuid().ToString();
                ServicePointManager.ServerCertificateValidationCallback += PinPublicKey;
                Constants.APIENCRYPTKEY = Convert.ToBase64String(Encoding.Default.GetBytes(Session(32)));
                Constants.APIENCRYPTSALT = Convert.ToBase64String(Encoding.Default.GetBytes(Session(16)));
                Constants.IV = Convert.ToBase64String(Encoding.Default.GetBytes(Constants.RandomString(16)));
                Constants.Key = Convert.ToBase64String(Encoding.Default.GetBytes(Constants.RandomString(32)));
                Constants.Started = true;
            }
        }
        public static void End()
        {
            if (!Constants.Started)
            {
                EditorUtility.DisplayDialog("nanoSDK Api", "No session has been started, closing for security reasons!", "Okay");
                Process.GetCurrentProcess().Kill();
            }
            else
            {
                Constants.Token = null;
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                Constants.APIENCRYPTKEY = null;
                Constants.APIENCRYPTSALT = null;
                Constants.IV = null;
                Constants.Key = null;
                Constants.Started = false;
            }
        }
        private static bool PinPublicKey(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return certificate != null && certificate.GetPublicKeyString() == _key;
        }
        private const string _key = "04E32E295F50051CD5A5AF5B9B19DFAAB514806DDDEEAEBB38AFCC8AB7D9F1BE5C8E7A782E377DC198E62A1D091A2ADD63F4AC0A320BC4341AD980E34B47C08DB6";
        public static string Integrity(string filename)
        {
            string result;
            using (MD5 md = MD5.Create())
            {
                using (FileStream fileStream = File.OpenRead(filename))
                {
                    byte[] value = md.ComputeHash(fileStream);
                    result = BitConverter.ToString(value).Replace("-", "").ToLowerInvariant();
                }
            }
            return result;
        }
        public static bool MaliciousCheck(string date)
        {
            DateTime dt1 = DateTime.Parse(date); //time sent
            DateTime dt2 = DateTime.Now; //time received
            TimeSpan d3 = dt1 - dt2;
            if (Convert.ToInt32(d3.Seconds.ToString().Replace("-", "")) >= 5 || Convert.ToInt32(d3.Minutes.ToString().Replace("-", "")) >= 1)
            {
                Constants.Breached = true;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    internal class Encryption
    {
        public static string APIService(string value)
        {
            string message = value;
            string password = Encoding.Default.GetString(Convert.FromBase64String(Constants.APIENCRYPTKEY));
            SHA256 mySHA256 = SHA256Managed.Create();
            byte[] key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(password));
            byte[] iv = Encoding.ASCII.GetBytes(Encoding.Default.GetString(Convert.FromBase64String(Constants.APIENCRYPTSALT)));
            string encrypted = EncryptString(message, key, iv);
            return encrypted;
        }
        public static string EncryptService(string value)
        {
            string message = value;
            string password = Encoding.Default.GetString(Convert.FromBase64String(Constants.APIENCRYPTKEY));
            SHA256 mySHA256 = SHA256Managed.Create();
            byte[] key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(password));
            byte[] iv = Encoding.ASCII.GetBytes(Encoding.Default.GetString(Convert.FromBase64String(Constants.APIENCRYPTSALT)));
            string encrypted = EncryptString(message, key, iv);
            int property = Int32.Parse((OnProgramStart.AID.Substring(0, 2)));
            string final = encrypted + Security.Obfuscate(property);
            return final;
        }
        public static string DecryptService(string value)
        {
            string message = value;
            string password = Encoding.Default.GetString(Convert.FromBase64String(Constants.APIENCRYPTKEY));
            SHA256 mySHA256 = SHA256Managed.Create();
            byte[] key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(password));
            byte[] iv = Encoding.ASCII.GetBytes(Encoding.Default.GetString(Convert.FromBase64String(Constants.APIENCRYPTSALT)));
            string decrypted = DecryptString(message, key, iv);
            return decrypted;
        }
        public static string EncryptString(string plainText, byte[] key, byte[] iv)
        {
            Aes encryptor = Aes.Create();
            encryptor.Mode = CipherMode.CBC;
            encryptor.Key = key;
            encryptor.IV = iv;
            MemoryStream memoryStream = new MemoryStream();
            ICryptoTransform aesEncryptor = encryptor.CreateEncryptor();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, aesEncryptor, CryptoStreamMode.Write);
            byte[] plainBytes = Encoding.ASCII.GetBytes(plainText);
            cryptoStream.Write(plainBytes, 0, plainBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] cipherBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            string cipherText = Convert.ToBase64String(cipherBytes, 0, cipherBytes.Length);
            return cipherText;
        }

        public static string DecryptString(string cipherText, byte[] key, byte[] iv)
        {
            Aes encryptor = Aes.Create();
            encryptor.Mode = CipherMode.CBC;
            encryptor.Key = key;
            encryptor.IV = iv;
            MemoryStream memoryStream = new MemoryStream();
            ICryptoTransform aesDecryptor = encryptor.CreateDecryptor();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, aesDecryptor, CryptoStreamMode.Write);
            string plainText = String.Empty;
            try
            {
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                cryptoStream.Write(cipherBytes, 0, cipherBytes.Length);
                cryptoStream.FlushFinalBlock();
                byte[] plainBytes = memoryStream.ToArray();
                plainText = Encoding.ASCII.GetString(plainBytes, 0, plainBytes.Length);
            }
            finally
            {
                memoryStream.Close();
                cryptoStream.Close();
            }
            return plainText;
        }
        public static string Decode(string text)
        {
            text = text.Replace('_', '/').Replace('-', '+');
            switch (text.Length % 4)
            {
                case 2:
                    text += "==";
                    break;
                case 3:
                    text += "=";
                    break;
            }
            return Encoding.UTF8.GetString(Convert.FromBase64String(text));
        }

    }
    class InfoManager
    {
        private System.Threading.Timer timer;
        private string lastGateway;

        public InfoManager()
        {
            lastGateway = GetGatewayMAC();
        }

        public void StartListener()
        {
            timer = new System.Threading.Timer(_ => OnCallBack(), null, 5000, Timeout.Infinite);
        }

        private void OnCallBack()
        {
            timer.Dispose();
            if (!(GetGatewayMAC() == lastGateway))
            {
                Constants.Breached = true;
                EditorUtility.DisplayDialog("nanoSDK Api", "ARP Cache poisoning has been detected!", "Okay");
                Process.GetCurrentProcess().Kill();
            }
            else
            {
                lastGateway = GetGatewayMAC();
            }
            timer = new Timer(_ => OnCallBack(), null, 5000, Timeout.Infinite);
        }

        public static IPAddress GetDefaultGateway()
        {
            return NetworkInterface
                .GetAllNetworkInterfaces()
                .Where(n => n.OperationalStatus == OperationalStatus.Up)
                .Where(n => n.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .SelectMany(n => n.GetIPProperties()?.GatewayAddresses)
                .Select(g => g?.Address)
                .Where(a => a != null)
                .FirstOrDefault();
        }

        private string GetArpTable()
        {
            string drive = Path.GetPathRoot(Environment.SystemDirectory);
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = $@"{drive}Windows\System32\arp.exe";
            start.Arguments = "-a";
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            start.CreateNoWindow = true;

            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private string GetGatewayMAC()
        {
            string routerIP = GetDefaultGateway().ToString();
            string regx = String.Format(@"({0} [\W]*) ([a-z0-9-]*)", routerIP);
            Regex regex = new Regex(@regx);
            Match matches = regex.Match(GetArpTable());
            return matches.Groups[2].ToString();
        }
    }
}