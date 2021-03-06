﻿using Acr.UserDialogs;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Plugin.Permissions;
using Xamarin.Forms;

namespace Poc.Luis.Xamarin.Droid
{
    [Activity(Label = "Cognitive Test",
              Icon = "@drawable/ic_launcher",
              Theme = "@style/MyTheme",
              MainLauncher = true,
              ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            UserDialogs.Init(() => (Activity)Forms.Context);

            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}