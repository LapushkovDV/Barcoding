using Android.Content;
using System.Text.RegularExpressions;
using TSD.Model.User;
using TSD.Services;
using Xamarin.Forms;

namespace TSD.Droid.Services
{
    [BroadcastReceiver(Enabled = true)]
    public class ReceiverData : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent receivedIntent)
        {
            if (UserAccount.IsBlock) return;

            UserAccount.IsBlock = true;

            var barcode = receivedIntent.GetStringExtra("EXTRA_BARCODE_DECODING_DATA");
            var result = Regex.Replace(barcode, @"\t|\n|\r", string.Empty);

            MessagingCenter.Send(new MessageClass<string>(result), Resources.MsgCenterTagSendBarcode);
        }
    }
}