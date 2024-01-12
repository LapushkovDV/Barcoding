using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using AndroidX.Core.Content;
using TSD.Droid.Services.ElementsRenderer;
using TSD.Services.DependencyProperties;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ImageEntry), typeof(ImageEntryRenderer))]
namespace TSD.Droid.Services.ElementsRenderer
{
    public class ImageEntryRenderer : EntryRenderer
    {
        ImageEntry element;

        public ImageEntryRenderer(Context context) : base(context) { }

        [System.Obsolete]
#pragma warning disable CS0809 // Устаревший член переопределяет неустаревший член
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
#pragma warning restore CS0809 // Устаревший член переопределяет неустаревший член
        {
            base.OnElementChanged(e);
            if (e.OldElement != null || e.NewElement == null)
                return;

            element = (ImageEntry)Element;

            var editText = Control;

            editText.SetSelectAllOnFocus(true);

            if (!string.IsNullOrEmpty(element.Image))
            {
                switch (element.ImageAlignment)
                {
                    case ImageAlignment.Left:
                        editText.SetCompoundDrawablesWithIntrinsicBounds(GetDrawable(element.Image), null, null, null);
                        break;
                    case ImageAlignment.Right:
                        editText.SetCompoundDrawablesWithIntrinsicBounds(null, null, GetDrawable(element.Image), null);
                        break;
                }
            }
            editText.CompoundDrawablePadding = 25;

            if (Build.VERSION.SdkInt < BuildVersionCodes.Q)
            {
                Control.Background.SetColorFilter(element.LineColor.ToAndroid(), PorterDuff.Mode.SrcAtop);
            }
            else
            {
                Control.Background.SetColorFilter(new BlendModeColorFilter(element.LineColor.ToAndroid(), BlendMode.SrcAtop));
            }
        }

        private BitmapDrawable GetDrawable(string imageEntryImage)
        {
            var resID = Resources.GetIdentifier(imageEntryImage, "drawable", Context.PackageName);
            var drawable = ContextCompat.GetDrawable(Context, resID);
            var bitmap = ((BitmapDrawable)drawable).Bitmap;
            var width = Build.VERSION.SdkInt < BuildVersionCodes.P ? element.ImageWidth * 2 : element.ImageWidth / 2;
            var height = Build.VERSION.SdkInt < BuildVersionCodes.P ? element.ImageHeight * 2 : element.ImageHeight / 2;

            return new BitmapDrawable(Resources, Bitmap.CreateScaledBitmap(bitmap, width, height, true));
        }
    }
}