using Android.Content;
using Android.Text.Method;
using TSD.Droid.Services.ElementsRenderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Editor), typeof(MyEditorRenderer))]
namespace TSD.Droid.Services.ElementsRenderer
{
    public class MyEditorRenderer : EditorRenderer
    {
        public MyEditorRenderer(Context ctx) : base(ctx) { }
        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
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