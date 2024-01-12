using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using System;
using Xamarin.Forms;

namespace TSD.Droid.Services
{
    [Service(Label = "UnBlockPositionService")]
    public class UnBlockPositionService : Service
    {
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            Device.StartTimer(TimeSpan.FromSeconds(5), () => { return true; });

            return StartCommandResult.Sticky;
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override void OnDestroy()
        {

            StopSelf();
            base.OnDestroy();
        }

    }
}