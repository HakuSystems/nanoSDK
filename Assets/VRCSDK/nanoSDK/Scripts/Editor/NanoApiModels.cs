using System.Collections.Generic;

namespace nanoSDK
{
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

    public class BaseResponse<T>
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

    public class LoginResponse
    {
        public string AuthKey { get; set; }
    }

    public class LicenseData
    {
        public string Key { get; set; }
    }
}