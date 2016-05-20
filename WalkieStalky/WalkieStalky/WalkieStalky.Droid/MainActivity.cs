﻿using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using WalkieStalky.Services;
using Xamarin.Auth;

namespace WalkieStalky.Droid
{
    [Activity(Label = "WalkieStalky.Droid", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity, ILoginService
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App(new Services.Services { LoginService = this }));
        }

        public void LogIn()
        {
            var auth = new OAuth2Authenticator(
                Constants.ClientId,
                Constants.Scope,
                new Uri(Constants.AuthorizeUrl),
                new Uri(Constants.RedirectUrl));
            auth.Completed += Auth_Completed; ;
            var intent = auth.GetUI(this);
            StartActivity(intent);
        }

        private void Auth_Completed(object sender, AuthenticatorCompletedEventArgs e)
        {
            OnLogin?.Invoke(sender, new OnLoginEventArgs { Account = e.Account });
        }

        public event OnLoginEvent OnLogin;
    }


    internal class Constants
    {
        public static string ClientId = "612161925597862";
        public static string ClientSecret = "6f87a5247c5131dda0699375e44ff4ba";
        public static string Scope = "";
        public static string AuthorizeUrl = "https://m.facebook.com/dialog/oauth/";
        public static string RedirectUrl = "http://www.facebook.com/connect/login_success.html";
        //public static string RedirectUrl = "";
        public static string AccessTokenUrl;
    }
}

