using Android.Content;
using System;
using TSD.Droid.Services.Dependency;
using TSD.Model;
using TSD.Services.Interfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(StartServiceAndroid))]
namespace TSD.Droid.Services.Dependency
{

    public class StartServiceAndroid : IWorkService
    {
        public void StartForegroundServiceCompact(ServiceEnums service = ServiceEnums.Sync, Type sourceCall = default)
        {
            SwitchService(service, true, sourceCall);
        }

        public void StopForegroundServiceCompact(ServiceEnums service = ServiceEnums.Sync)
        {
            SwitchService(service, false);
        }

        private void SwitchService(ServiceEnums service, bool isStart = true, Type sourceCall = default)
        {
            switch (service)
            {
                case ServiceEnums.Sync:
                    {
                        if (isStart)
                            MainActivity.Instance.StartService(new Intent(MainActivity.Instance, typeof(SyncSendService)));
                        else
                            MainActivity.Instance.StopService(new Intent(MainActivity.Instance, typeof(SyncSendService)));

                        break;
                    }
                case ServiceEnums.WaitLogin:
                    {
                        if (isStart)
                            MainActivity.Instance.StartService(new Intent(MainActivity.Instance, typeof(WaitAuthService)));
                        else
                            MainActivity.Instance.StopService(new Intent(MainActivity.Instance, typeof(WaitAuthService)));

                        break;
                    }
                case ServiceEnums.Scanner:
                    {
                        if (isStart)
                        {
                            var activity = MainActivity.Instance;
                            activity.TypeSource = sourceCall;

                            MainActivity.Instance.StartService(new Intent(activity, typeof(BarcodeReaderService)));
                        }
                        else
                            MainActivity.Instance.StopService(new Intent(MainActivity.Instance, typeof(BarcodeReaderService)));


                        break;
                    }
            }
        }
    }
}