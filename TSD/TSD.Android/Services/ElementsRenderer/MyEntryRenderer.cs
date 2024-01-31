using Android.Content;
using Android.Widget;
using TSD.Droid.Services.ElementsRenderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Entry), typeof(MyEntryRenderer))]
namespace TSD.Droid.Services.ElementsRenderer
{
    public class MyEntryRenderer : EntryRenderer
    {
        public MyEntryRenderer(Context ctx) : base(ctx) { }
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            
            if (e.OldElement == null)
            {
                var nativeEditText = Control;
                nativeEditText.SetSelectAllOnFocus(true);
            }
        }
    }
}