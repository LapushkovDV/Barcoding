using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Microsoft.AppCenter.Distribute;
using Microsoft.Extensions.DependencyInjection;
using System;
using TSD.Droid.Services;
using TSD.Views;
using ResourcesTerminal = TSD.Resources;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace TSD.Droid
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, Label = "ТСД", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        internal static int REQUEST_PERMISSION_PHONE_STATE => 1;
        internal static MainActivity Instance { get; private set; }
        internal static string CHANNEL_ID => "my_notification_channel";
        internal static int NOTIFICATION_ID => 100;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Instance = this;
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            Forms.Init(this, savedInstanceState);
            Distribute.SetEnabledForDebuggableBuild(true);

            LoadApplication(new App(AddServices));

            var receiver = new ReceiverData();
            var intentFilter = new IntentFilter();
            
            intentFilter.AddAction("com.xcheng.scanner.action.BARCODE_DECODING_BROADCAST");
            intentFilter.AddAction("BARCODE_BROADCAST");

            RegisterReceiver(receiver, intentFilter);

        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public Type TypeSource { get; set; } = default;

        static void AddServices(IServiceCollection services)
        {
    
        }
    }
}