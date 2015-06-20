using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content.PM;

namespace Demo.Xamarin.Android
{
    // the ConfigurationChanges flags set here keep the EGL context
    // from being destroyed whenever the device is rotated or the
    // keyboard is shown (highly recommended for all GL apps)
    [Activity(Label = "Demo.Xamarin.Android",
				#if __ANDROID_11__
				HardwareAccelerated=false,
				#endif
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden,
        MainLauncher = true,
        Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            DDDirector.Instance.OnCreate(this, bundle, ScreenOrientation.Portrait, 30,
                new DDRenderer(), () => new Demo.Shared.MyScene());
        }

        protected override void OnPause()
        {
            base.OnPause();
            DDDirector.Instance.OnPause(this);
        }

        protected override void OnResume()
        {
            base.OnResume();
            DDDirector.Instance.OnResume(this);
        }
    }
}


