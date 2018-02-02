using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Plugin.Permissions;
using ImageCircle.Forms.Plugin.Droid;
using System.Reflection;
using Xamarin.Forms;

namespace BjjAcademy.Droid
{
    [Activity(Label = "BjjAcademy", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            /*MethodInfo dynMethod = typeof(MessagingCenter).GetMethod("ClearSubscribers", BindingFlags.NonPublic | BindingFlags.Static);
            dynMethod.Invoke(null, null);
            */
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            Xamarin.Forms.Forms.Init(this, bundle);
            ImageCircleRenderer.Init();

            LoadApplication(new App());
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

