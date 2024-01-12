using System.ComponentModel;
using TSD.Services;
using TSD.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TSD.Views.FlyoutPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [DesignTimeVisible(true)]
    public partial class AppPageFlyout : ContentPage
    {
        public ListView ListView;

        public AppPageFlyout()
        {
            BindingContext = App.GetViewModel<AppPageViewModel>();
            InitializeComponent();

            ListView = MenuItemsListView;
        }
    }
}