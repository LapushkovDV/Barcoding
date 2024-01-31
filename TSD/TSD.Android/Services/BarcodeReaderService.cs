using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Honeywell.AIDC.CrossPlatform;
using System;
using System.Linq;
using System.Threading.Tasks;
using TSD.Model.User;
using TSD.Services;
using Xamarin.Forms;
using ResourcesTerminal = TSD.Resources;

namespace TSD.Droid.Services.Dependency
{
    [Service(Label = "ScannerActive")]
    public class BarcodeReaderService : Service
    {
        private BarcodeReader _reader;
        private string _data;
        private bool _isStartService = false;

        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            _isStartService = true;

            Device.BeginInvokeOnMainThread(async () =>
            {
                if (await GetReaderDefault())
                {
                    Device.StartTimer(TimeSpan.FromMilliseconds(0), () =>
                    {
                        if (_data != null && _data != string.Empty)
                        {
                            try
                            {
                                if (!UserAccount.IsBlock)
                                {
                                    UserAccount.IsBlock = true;
                                    MessagingCenter.Send(new MessageClass<string>(_data), ResourcesTerminal.MsgCenterTagSendBarcode);
                                }

                                _data = string.Empty;
                            }
                            catch (Exception ex)
                            {
                                throw new Exception("проблема", ex);
                            }
                        }

                        return _isStartService;
                    });
                }
                else
                {
                    MessagingCenter.Send(new MessageClass<string>(ResourcesTerminal.ConstCancel), ResourcesTerminal.MsgCenterTagScanner);
                }
            });
            return StartCommandResult.Sticky;
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override void OnDestroy()
        {
            if (_isStartService)
            {
                _isStartService = false;
                MainActivity.Instance.TypeSource = default;

                StopSelf();
                base.OnDestroy();
            }
        }

        private async Task<bool> GetReaderDefault()
        {
            var readers = await BarcodeReader.GetConnectedBarcodeReaders();

            if (readers.Count != 0)
            {
                if (_reader == null)
                {
                    var name = readers.ToList().FirstOrDefault().ScannerName;
                    _reader = new BarcodeReader(name);
                    _reader.BarcodeDataReady += Reader_BarcodeDataReady;

                    MessagingCenter.Send(new MessageClass<bool>(true), ResourcesTerminal.MsgCenterTagIsExistBarcode);
                }
            }
            else
            {
                MessagingCenter.Send(new MessageClass<bool>(true), ResourcesTerminal.MsgCenterTagIsExistBarcode);
            }

            if (_reader != null)
            {
                if (!_reader.IsReaderOpened)
                {
                    var result = await _reader.OpenAsync();

                    return result.Code == BarcodeReaderBase.Result.Codes.SUCCESS;

                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        private void Reader_BarcodeDataReady(object sender, BarcodeDataArgs e)
        {
            if (e.Data != string.Empty)
            {
                _data = e.Data;
            }
        }
    }
}