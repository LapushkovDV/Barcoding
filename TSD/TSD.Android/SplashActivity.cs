using Android.App;
using Android.OS;

namespace TSD.Droid
{
    [Activity(Label = "ТСД", Icon = "@mipmap/icon", Theme = "@style/splashscreen", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            StartActivity(typeof(MainActivity));
        }
    }
}