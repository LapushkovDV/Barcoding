using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TSD.Views.FlyoutPages.ContentViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActionsView : ContentView
    {
        public ActionsView() => InitializeComponent();

        private void List_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e == null) return;

            ((ListView)sender).SelectedItem = null;
        }
    }

}