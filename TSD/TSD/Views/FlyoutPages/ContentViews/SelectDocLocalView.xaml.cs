using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TSD.Views.FlyoutPages.ContentViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectDocLocalView : ContentView
    {
        public SelectDocLocalView()
        {
            InitializeComponent();
        }

        private void ListViewColumns_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e == null) return;

            ((ListView)sender).SelectedItem = null;
        }
    }
}