using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using System;
using TSD.Droid.Services.Dependency;
using TSD.Services;
using Xamarin.Forms;
using ResourcesTerminal = TSD.Resources;

namespace TSD.Droid.Services
{
    [Service(Label = "WaitAuthService")]
    public class WaitAuthService : Service
    {
        readonly AccountManagerDroid _droidAccount = new AccountManagerDroid();
        bool _workTimer;

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            _workTimer = true;

            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                if (_droidAccount.IsValue(ResourcesTerminal.RestKeyToken))
                {
                    var date = DateTime.Parse(_droidAccount.GetValue(ResourcesTerminal.RestKeyExpire));
                    if (_droidAccount.IsValue(ResourcesTerminal.RestKeyExpire) && (DateTime.Parse(_droidAccount.GetValue(ResourcesTerminal.RestKeyExpire)) < DateTime.UtcNow))
                    {
                        MessagingCenter.Send(new MessageClass<object>(), ResourcesTerminal.MsgCenterTagWaitAuth);

                        _workTimer = false;
                    }
                }

                return _workTimer;
            });

            return StartCommandResult.NotSticky;
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