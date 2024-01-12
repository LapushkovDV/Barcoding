using MyApp.Droid;
using TSD.Services.Interfaces;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidMethods))]
namespace MyApp.Droid
{
    public class AndroidMethods : IAndroidMethods
    {
        public void CloseApp()
        {
            Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
        }
    }
}